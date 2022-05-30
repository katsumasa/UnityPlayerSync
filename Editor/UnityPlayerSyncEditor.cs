using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UTJ.RemoteConnect.Editor;
using UTJ.UnityPlayerSyncEngine;

namespace UTJ.UnityPlayerSyncEditor
{
               
    public class UnityPlayerSyncEditorWindow : RemoteConnectEditorWindow
    {
        [MenuItem("Window/UTJ/UnityPlayerSync/Open")]
        static void OpenWindow()
        {
            var window = (UnityPlayerSyncEditorWindow)EditorWindow.GetWindow(typeof(UnityPlayerSyncEditorWindow));
        }

        protected override void OnEnable()
        {            
            kMsgSendEditorToPlayer = UnityPlayerSyncRuntime.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncRuntime.kMsgSendPlayerToEditor;
            remoteMessageCB = MessageReciveCB;
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

        private void OnGUI()
        {
            ConnectionTargetSelectionDropdown();

            if (GUILayout.Button("SYNC"))
            {
                var message = new RemoteConnect.Message(0);
                

                SendRemoteMessage(RemoteConnect.Message.Serialize(message));
            }

        }
    }            
}