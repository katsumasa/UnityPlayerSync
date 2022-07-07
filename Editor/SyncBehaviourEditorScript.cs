//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;
using UnityEditor;
using UTJ.UnityPlayerSync.Runtime;



namespace UTJ.UnityPlayerSync.Editor
{
    /// <summary>
    /// Hierarchy Window�ł̃R���e�L�X�g���j���[����������N���X
    /// </summary>
    public static class SyncHierarchyScript
    {
        [MenuItem("GameObject/Sync")]
        private static void ContextMenu(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;
            if(go == null)
            {
                // Scene�̓��������s����(from Player to Editor)
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    bw.Write((int)MessageID.SyncScene);
                    UnityEditorSyncWindow.SendMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
            else
            {
                // GameObject�̓��������s����(from Editor To Player)                
                
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    int count = 0;
                    GetGameObjectCount(go, ref count);

                    Debug.Log($"count : {count}");
                    
                    bw.Write((int)MessageID.SyncGameObject);
                    bw.Write(count);
                    SerializeGameObject(go, bw);                    
                    UnityEditorSyncWindow.SendMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
                
            }
        }

        private static void GetGameObjectCount(GameObject go, ref int count)
        {
            count++;
            for (var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                GetGameObjectCount(child.gameObject, ref count);
            }
        }

        private static void SerializeGameObject(GameObject go,BinaryWriter binaryWriter)
        {
            var sync = SyncGameObject.Find(go);
            if(sync == null)
            {
                sync = new SyncGameObject(go);
            }
            binaryWriter.Write(sync.GetInstanceID());
            sync.Serialize(binaryWriter);

            for(var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                SerializeGameObject(child.gameObject, binaryWriter);
            }
        }
    }


   


    // Component�̃R���e�L�X�g���j���[����������N���X
    [CustomEditor(typeof(Component))]

    public class SyncComponentScript : UnityEditor.Editor
    {
        /// <summary>
        /// �R���e�L�X�g���j���[����Sync��I�����ꂽ���̏���
        /// </summary>
        /// <param name="menuCommand">�I�����ꂽcomponent</param>
        [MenuItem("CONTEXT/Component/Sync")]
        private static void ContextMenu(MenuCommand menuCommand)
        {
            // MenuCommand.context�ɃR���|�[�l���g�̃C���X�^���X�������Ă���
            var component = menuCommand.context as Component;


            // Transform�̂ݓ��ʏ���
            if (component is Transform)
            {
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    var sync = SyncTransform.Find(component);
                    if (sync != null)
                    {                        
                        bw.Write((int)MessageID.SyncTransform);
                        bw.Write(sync.GetInstanceID());
                        sync.Serialize(bw);
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
                    }
                    else
                    {
                        Debug.LogWarning($"Transform({component.name}) is not found.");
                    }
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
            else
            {
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    var sync = SyncComponent.Find(component);
                    if (sync != null)
                    {                        
                        bw.Write((int)MessageID.SyncComponent);
                        bw.Write(sync.GetInstanceID());
                        sync.Reset();
                        sync.Serialize(bw);
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
                    }
                    else
                    {
                        Debug.LogWarning($"Component({component.name}) is not found.");
                    }
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
        }
    }
}