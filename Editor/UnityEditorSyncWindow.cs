//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;
using UnityEditor;
using UTJ.RemoteConnect.Editor;
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
            var br = new BinaryReader(ms);
            try
            {
                var id = br.ReadInt32();
                //Debug.Log($"Message ID:{id}");
                var sm = new SyncSceneManager(false);
                sm.Deserialize(br);
                sm.WriteBack();
            }
            finally
            {
                br.Close();
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
        }
    }            
}