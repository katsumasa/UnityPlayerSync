//
// Programed by Katsumasa Kimura
//
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime 
{
    /// <summary>
    /// Player‘¤‚Ì’†Šj
    /// </summary>
    public class UnityPlayerSyncPlayer : UTJ.RemoteConnect.Player
    {
        [SerializeField] bool m_IsLogEnable;

        protected override void OnEnable()
        {
            kMsgSendEditorToPlayer = UnityPlayerSyncGuid.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncGuid.kMsgSendPlayerToEditor;           
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
                if (m_IsLogEnable)
                {
                    Debug.Log($"messageID:{messageID}");
                }

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

                    case MessageID.SyncGameObject:
                        {
                            var syncs = new List<SyncGameObject>();

                            var count = binaryReader.ReadInt32();
                            for(var i = 0; i < count; i++)
                            {
                                var instanceID = binaryReader.ReadInt32();
                                if (m_IsLogEnable)
                                {
                                    Debug.Log($"instanceID:{instanceID}");
                                }

                                var go = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as GameObject;
                                SyncGameObject sync = null;
                                if (go != null)
                                {
                                    sync = SyncGameObject.Find(go);
                                    if(sync == null)
                                    {
                                        if (m_IsLogEnable)
                                        {
                                            Debug.Log($"{go.name}'s sync is not found.");
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_IsLogEnable)
                                    {
                                        Debug.Log($"{instanceID} is new GameObject.");
                                    }
                                    go = new GameObject();                                    
                                }
                                if(sync == null)
                                {
                                    sync = new SyncGameObject(go);
                                }
                                sync.Deserialize(binaryReader);
                                syncs.Add(sync);
                            }
                            foreach(var sync in syncs)
                            {
                                sync.WriteBack();
                            }
                        }
                        break;

                    case MessageID.SyncTransform:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            if (m_IsLogEnable)
                            {
                                Debug.Log($"instanceID:{instanceID}");
                            }
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
                                if (m_IsLogEnable)
                                {
                                    Debug.Log($"{sync.GetComponent().name}");
                                }
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