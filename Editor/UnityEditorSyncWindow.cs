//
// Programed by Katsumasa Kimura
//
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UTJ.RemoteConnect.Editor;
using UTJ.UnityPlayerSync;
using UTJ.UnityPlayerSync.Runtime;


namespace UTJ.UnityPlayerSync.Editor
{
    /// <summary>
    /// UnityPlayerSyncのEditor側の中核
    /// </summary>
    public class UnityEditorSyncWindow : RemoteConnectEditorWindow
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        static UnityEditorSyncWindow m_Instance;


        /// <summary>
        /// Player側にデータを送信する
        /// </summary>
        /// <param name="array">ストリームの配列</param>
        public static void SendMessage(byte[] array)
        {
            if(m_Instance == null)
            {
                OpenWindow();
            }
            m_Instance.SendRemoteMessage(array);
        }


        /// <summary>
        /// Open
        /// </summary>
        [MenuItem("Window/UTJ/UnityPlayerSync/Open")]
        static void OpenWindow()
        {
            m_Instance = (UnityEditorSyncWindow)EditorWindow.GetWindow(typeof(UnityEditorSyncWindow));
        }


        protected override void OnEnable()
        {
            kMsgSendEditorToPlayer = UnityPlayerSync.UnityPlayerSyncGuid.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSync.UnityPlayerSyncGuid.kMsgSendPlayerToEditor;
            eventMessageCB = EventMessageReciveCB;            
            base.OnEnable();
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        }

        private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            Debug.Log(menu);
            Debug.Log(property);
        }
        
        void EventMessageReciveCB(byte[] vs)
        {
            var ms = new MemoryStream(vs);
            var binaryReader = new BinaryReader(ms);
            try
            {
                var messageID = (MessageID)binaryReader.ReadInt32();
                //Debug.Log($"Message ID:{id}");
                switch (messageID)
                {
                    case MessageID.SyncScene:
                        {
                            SyncSceneManager.ClearCache();

                            var ofst = binaryReader.ReadInt64();
                            var returnPos = binaryReader.BaseStream.Position;
                            binaryReader.BaseStream.Seek(ofst, SeekOrigin.Begin);

                            SyncTypeTree.Deserialize(binaryReader);
                                                        
                            binaryReader.BaseStream.Seek(returnPos, SeekOrigin.Begin);

                            var sm = new SyncSceneManager(false);
                            sm.Deserialize(binaryReader);
                            sm.WriteBack();

                            UpdateFonts();
                        }
                        break;

                    case MessageID.SyncGameObject:
                        {                            
                            var treetypePos = binaryReader.ReadInt64();
                            var bodyPos = binaryReader.BaseStream.Position;
                            binaryReader.BaseStream.Seek(treetypePos, SeekOrigin.Begin);
                            SyncTypeTree.Deserialize(binaryReader);
                            binaryReader.BaseStream.Seek(bodyPos, SeekOrigin.Begin);

                            var syncs = new List<SyncGameObject>();
                            var count = binaryReader.ReadInt32();
                            for (var i = 0; i < count; i++)
                            {
                                var instanceID = binaryReader.ReadInt32();                                
                                //Debug.Log($"instanceID:{instanceID}");
                                var sync = SyncGameObject.FindEditor(instanceID);                                
                                if (sync == null)
                                {
                                    Debug.LogError($"{instanceID} is not found.");
                                    sync = new SyncGameObject(new GameObject());
                                }
                                sync.Deserialize(binaryReader);
                                syncs.Add(sync);
                            }
                            foreach (var sync in syncs)
                            {
                                sync.WriteBack();
                            }
                        }
                        break;

                    case MessageID.SyncDelete:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            var sync = SyncGameObject.Find(instanceID);
                            if(sync != null)
                            {
                                GameObject.DestroyImmediate(sync.gameObject);
                                sync.Dispose();
                            }
                        }
                        break;
                }
                
            }
            finally
            {
                binaryReader.Close();
                ms.Close();
            }
        }

        private void OnGUI()
        {
            ConnectionTargetSelectionDropdown();

            if (GUILayout.Button("SYNC"))
            {
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    bw.Write((int)MessageID.SyncScene);
                    SendRemoteMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
            if (GUILayout.Button("GC"))
            {
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    bw.Write((int)MessageID.GC);
                    SendRemoteMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
            if(GUILayout.Button("UnLoadUnUsedAsset"))
            {
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                try
                {
                    bw.Write((int)MessageID.UnLoadUnUsedAsset);
                    SendRemoteMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }
        }


        private void UpdateFonts()
        {
            // TextMeshPro
            {
                var type = System.Type.GetType("TMPro.TextMeshProUGUI,Unity.TextMeshPro");
                if (type != null)
                {
                    var method = type.GetMethod("UpdateFontAsset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (method != null)
                    {
                        var objects = Resources.FindObjectsOfTypeAll(type);
                        foreach (var obj in objects)
                        {
                            method.Invoke(obj, null);
                        }
                    }

                }
            }
            // UI.Text
            {
                var objects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text[];
                foreach (var obj in objects)
                {
                    obj.FontTextureChanged();
                }
            }
        }
    }            
}