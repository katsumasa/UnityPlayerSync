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
                var syncSceneManager = new SyncSceneManager(true);
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

    }
}