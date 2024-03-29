﻿//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;
using UnityEditor;
using UTJ.UnityPlayerSync.Runtime;


namespace UTJ.UnityPlayerSync.Editor
{
    /// <summary>
    /// Hierarchy Windowでのコンテキストメニューを処理するクラス
    /// </summary>
    public static class SyncHierarchyScript
    {
        [MenuItem("GameObject/Sync",priority = 100)]
        private static void ContextMenuSync(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;
            if(go == null)
            {
                // Sceneの同期を実行する(from Player to Editor)
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
                // GameObjectの同期を実行する(from Editor To Player)                
                if(go.transform.parent != null)
                {
                    var sync = SyncGameObject.Find(go.transform.parent.gameObject);
                    if(sync == null || sync.GetInstanceID() == SyncUnityEngineObject.InstanceID_None)
                    {
                        Debug.LogWarning($"{go.name} is not synced. Berore you should sync parent gameObject.");
                        return;
                    }
                }
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                try
                {
                    SyncTypeTree.Instances.Clear();                    
                    bw.Write((int)MessageID.SyncGameObject);
                    var ofsetPos = bw.BaseStream.Position;
                    bw.Write((long)0);    // TreeTypeへのオフセットのリザーブ                
                    int count = 0;
                    GetGameObjectCount(go, ref count);
                    bw.Write(count);
                    SerializeGameObject(go, bw);

                    var typeTreePos = bw.BaseStream.Position;
                    SyncTypeTree.Serialize(bw);
                    bw.BaseStream.Seek(ofsetPos, SeekOrigin.Begin);
                    bw.Write(typeTreePos);

                    UnityEditorSyncWindow.SendMessage(ms.ToArray());
                }
                finally
                {                    
                }                
            }
        }

        [MenuItem("GameObject/Sync Delete", priority = 101)]
        private static void ContextMenuDelete(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;            
            if(go != null)
            {
                var sync = SyncGameObject.Find(go);
                if(sync != null)
                {
                    using var ms = new MemoryStream();
                    using var bw = new BinaryWriter(ms);
                    try
                    {
                        bw.Write((int)MessageID.SyncDelete);
                        bw.Write(sync.GetInstanceID());
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
                    }
                    finally
                    {                        
                    }
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


   


    // Componentのコンテキストメニューを処理するクラス
    [CustomEditor(typeof(Component))]

    public class SyncComponentScript : UnityEditor.Editor
    {
        /// <summary>
        /// コンテキストメニューからSyncを選択された時の処理
        /// </summary>
        /// <param name="menuCommand">選択されたcomponent</param>
        [MenuItem("CONTEXT/Component/Sync")]
        private static void ContextMenu(MenuCommand menuCommand)
        {
            // MenuCommand.contextにコンポーネントのインスタンスが入ってくる
            var component = menuCommand.context as Component;


            // Transformのみ特別処理
            if (component is Transform)
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                try
                {
                    var sync = SyncTransform.Find(component);
                    if (sync != null)
                    {
                        SyncTypeTree.Instances.Clear();
                        bw.Write((int)MessageID.SyncTransform);
                        var ofst1 = bw.BaseStream.Position;
                        bw.Write((long)0);
                        bw.Write(sync.GetInstanceID());
                        sync.Serialize(bw);
                        var ofst2 = bw.BaseStream.Position;
                        SyncTypeTree.Serialize(bw);
                        bw.BaseStream.Seek(ofst1, SeekOrigin.Begin);
                        bw.Write(ofst2);
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
                    }
                    else
                    {
                        Debug.LogWarning($"Transform({component.name}) is not found.");
                    }
                }
                finally
                {                    
                }
            }
            else
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                try
                {
                    var sync = SyncComponent.Find(component);
                    if (sync != null)
                    {
                        SyncTypeTree.Instances.Clear();
                        bw.Write((int)MessageID.SyncComponent);
                        var ofst1 = bw.BaseStream.Position;
                        bw.Write((long)0);
                        bw.Write(sync.GetInstanceID());
                        sync.Init();
                        sync.Serialize(bw);
                        var ofst2 = bw.BaseStream.Position;
                        SyncTypeTree.Serialize(bw);
                        bw.BaseStream.Seek(ofst1, SeekOrigin.Begin);
                        bw.Write(ofst2);
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
                    }
                    else
                    {
                        Debug.LogWarning($"Component({component.name}) is not found.");
                    }
                }
                finally
                {
                    SyncTypeTree.Instances.Clear();
                }
            }
        }
    }
}