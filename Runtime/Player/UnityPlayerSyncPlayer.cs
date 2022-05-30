using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine {

    public class UnityPlayerSyncPlayer : UTJ.RemoteConnect.Player
    {



        protected override void OnEnable()
        {
            kMsgSendEditorToPlayer = UnityPlayerSyncRuntime.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncRuntime.kMsgSendPlayerToEditor;
            remoteMessageCB = MessageReciveCB;
            base.OnEnable();
        }


        void MessageReciveCB(UTJ.RemoteConnect.Message remoteMessageBase)
        {
            Debug.Log(remoteMessageBase.messageId);
            var message = new RemoteConnect.Message(1);
            SendRemoteMessage(RemoteConnect.Message.Serialize(message));
        }


    }
}