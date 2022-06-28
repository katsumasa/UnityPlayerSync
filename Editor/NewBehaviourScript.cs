using System.IO;
using UnityEngine;
using UnityEditor;
using UTJ.UnityPlayerSyncEngine;
using UTJ.RemoteConnect.Editor;


namespace UTJ.UnityPlayerSyncEditor
{
    public static class SyncSceneScript
    {
        [MenuItem("GameObject/Sync")]
        private static void ContextMenu(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;
            if(go == null)
            {
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
        }
    }






    [CustomEditor(typeof(Component))]

    public class SyncComponentScript : Editor
    {
        [MenuItem("CONTEXT/Component/Sync")]
        private static void ContextMenu(MenuCommand menuCommand)
        {
            // MenuCommand.contextにコンポーネントのインスタンスが入ってくる
            var component = menuCommand.context as Component;

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
                        sync.Serialize(bw);
                        UnityEditorSyncWindow.SendMessage(ms.ToArray());
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