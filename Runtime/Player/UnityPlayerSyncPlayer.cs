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
            var readerMemory = new MemoryStream(bytes);
            var writerMemory = new MemoryStream();
            var binaryReader = new BinaryReader(readerMemory);
            var binaryWriter = new BinaryWriter(writerMemory);

            try
            {                
                var messageID = (MessageID)binaryReader.ReadInt32();
                switch (messageID)
                {
                    case MessageID.SyncScene:
                        {
                            binaryWriter.Write((int)MessageID.SyncScene);
                            var syncSceneManager = new SyncSceneManager(true);
                            syncSceneManager.Serialize(binaryWriter);
                            var vs = writerMemory.ToArray();
                            SendRemoteMessage(vs);
                        }
                        break;

                    case MessageID.SyncTransform:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            //Debug.Log($"instanceID:{instanceID}");
                            var sync = SyncTransform.Find(instanceID);
                            if (sync != null)
                            {
                                sync.Deserialize(binaryReader);
                            }
                            else
                            {
                                Debug.LogWarning($"instanceID;{instanceID} is not found.");
                            }
                        }
                        break;

                    case MessageID.SyncComponent:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            var sync = SyncComponent.Find(instanceID);
                            if (sync != null)
                            {
                                Debug.Log($"{sync.GetComponent().name}");

                                sync.Deserialize(binaryReader);
                                sync.WriteBack();
                            }
                            else
                            {
                                Debug.LogWarning($"instanceID;{instanceID} is not found.");
                            }

                        }
                        break;
                }

            }

            finally
            {
                binaryReader.Close();
                binaryWriter.Close();
                readerMemory.Close();
                writerMemory.Close();
            }


            
            
        }        

    }
}