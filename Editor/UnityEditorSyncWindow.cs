using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UTJ.RemoteConnect.Editor;
using UTJ.UnityPlayerSyncEngine;

namespace UTJ.UnityPlayerSyncEditor
{
               
    public class UnityEditorSyncWindow : RemoteConnectEditorWindow
    {
        [MenuItem("Window/UTJ/UnityPlayerSync/Open")]
        static void OpenWindow()
        {
            var window = (UnityEditorSyncWindow)EditorWindow.GetWindow(typeof(UnityEditorSyncWindow));
        }

        protected override void OnEnable()
        {            
            kMsgSendEditorToPlayer = UnityPlayerSyncRuntime.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncRuntime.kMsgSendPlayerToEditor;
            eventMessageCB = EventMessageReciveCB;            
            base.OnEnable();

            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        }

        void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            Debug.Log(menu);
            Debug.Log(property);
        }


        void MessageReciveCB(UTJ.RemoteConnect.Message message)
        {
            Debug.Log(message.messageId);
        }


        void EventMessageReciveCB(byte[] vs)
        {
            var ms = new MemoryStream(vs);
            var br = new BinaryReader(ms);
            try
            {
                var id = br.ReadInt32();


                //var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
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
                    bw.Write(0);
                    SendRemoteMessage(ms.ToArray());
                }
                finally
                {
                    bw.Close();
                    ms.Close();
                }
            }

            if(GUILayout.Button("Close"))
            {

            }

        }
    }            
}