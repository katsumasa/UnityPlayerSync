//
// Programed by Katsumasa Kimura
//
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime 
{
    /// <summary>
    /// Player側の中核
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



                            // Player側の情報でEditor側に送信し直す
                            foreach (var sync in syncs)
                            {
                                sync.Reset();
                            }
                            binaryWriter.Write((int)MessageID.SyncGameObject);
                            binaryWriter.Write(count);
                            foreach (var sync in syncs)
                            {
                                binaryWriter.Write(sync.GetInstanceEditorID());
                                sync.Serialize(binaryWriter);

                            }
#if false
                            var root = syncs[0].gameObject;
                            count = 0;
                            GetGameObjectCount(root, ref count);
                            binaryWriter.Write((int)MessageID.SyncGameObject);
                            binaryWriter.Write(count);
                            SerializeGameObject(root, binaryWriter);
#endif
                            SendRemoteMessage(writerMemory.ToArray());

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


        private static void GetGameObjectCount(GameObject go, ref int count)
        {
            count++;
            for (var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                GetGameObjectCount(child.gameObject, ref count);
            }
        }

        private static void SerializeGameObject(GameObject go, BinaryWriter binaryWriter)
        {
            var sync = SyncGameObject.Find(go);
            if (sync != null)
            {
                sync.Reset();
            }
            else
            {
                sync = new SyncGameObject(go);
            }
            binaryWriter.Write(sync.GetInstanceID());
            sync.Serialize(binaryWriter);

            for (var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                SerializeGameObject(child.gameObject, binaryWriter);
            }
        }
    }
}