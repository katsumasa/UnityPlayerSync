using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTJ.UnityPlayerSyncEngine {

    public class UnityPlayerSyncPlayer : UTJ.RemoteConnect.Player
    {



        protected override void OnEnable()
        {
            kMsgSendEditorToPlayer = UnityPlayerSyncRuntime.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncRuntime.kMsgSendPlayerToEditor;
            //remoteMessageCB = MessageReciveCB;
            messageEventCB = MessageReciveEventCB;
            base.OnEnable();
        }
        

        void MessageReciveEventCB(byte[] bytes)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            try
            {
                bw.Write(0);
                var syncSceneManager = new SyncSceneManager();
                syncSceneManager.Serialize(bw);
                var vs = ms.ToArray();

                SendRemoteMessage(vs);
            }

            finally
            {
                bw.Close();
                ms.Close();
            }

        }


        void MessageReciveCB(UTJ.RemoteConnect.Message remoteMessageBase)
        {
            Debug.Log(remoteMessageBase.messageId);
            var message = new RemoteConnect.Message(1);

            var syncSceneManager = new SyncSceneManager();
            var memory = new MemoryStream();
            var writer = new BinaryWriter(memory);
            byte[] bytes;

            syncSceneManager.Serialize(writer);
            bytes = memory.ToArray();
            writer.Close();
            memory.Close();

            SendRemoteMessage(RemoteConnect.Message.Serialize(message));
        }


    }
}