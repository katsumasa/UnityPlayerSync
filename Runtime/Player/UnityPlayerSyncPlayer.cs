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
        [SerializeField] bool m_IsDontDestroyOnLoadEnabled;

        //byte[] m_WriteBuffers = new byte[200 * 1024 * 1024];

        private void Start()
        {
            if (m_IsDontDestroyOnLoadEnabled)
            {
                DontDestroyOnLoad(this.gameObject);
            }                       
        }

        protected override void OnEnable()
        {
            kMsgSendEditorToPlayer = UnityPlayerSyncGuid.kMsgSendEditorToPlayer;
            kMsgSendPlayerToEditor = UnityPlayerSyncGuid.kMsgSendPlayerToEditor;           
            messageEventCB = MessageReciveEventCB;
            base.OnEnable();
        }
        

        void MessageReciveEventCB(byte[] bytes)
        {
            SyncSceneManager.ClearCache();
           
            var readerMemory = new MemoryStream(bytes);
            //var writerMemory = new MemoryStream(m_WriteBuffers,true);            
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
                    case MessageID.GC:
                        {
                            System.GC.Collect();                        
                        }
                        break;

                    case MessageID.UnLoadUnUsedAsset:
                        {
                            Resources.UnloadUnusedAssets();
                        }
                        break;

                    case MessageID.SyncScene:
                        {
                            binaryWriter.Write((int)MessageID.SyncScene);
                            var syncSceneManager = new SyncSceneManager(true);
                            syncSceneManager.Serialize(binaryWriter);
                            var vs = writerMemory.GetBuffer();
                            SendRemoteMessage(vs);
                        }
                        break;

                    case MessageID.SyncGameObject:
                        {
                            var syncs = new List<SyncGameObject>();                            
                            var count = binaryReader.ReadInt32();
                            for (var i = 0; i < count; i++)
                            {
                                var instanceID = binaryReader.ReadInt32();
                                if (m_IsLogEnable)
                                {
                                    Debug.Log($"instanceID:{instanceID}");
                                }

                                var go = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as GameObject;
                                if (go == null)
                                {
                                    if (m_IsLogEnable)
                                    {
                                        Debug.Log($"{instanceID} is new GameObject.");
                                    }
                                    go = new GameObject();
                                }
                                SyncGameObject sync = new SyncGameObject(go);
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
                                // Editor側のInstanceIDを使用
                                Debug.Log($"{sync.GetInstanceID()},{sync.GetInstanceEditorID()}");
                                binaryWriter.Write(sync.GetInstanceEditorID());
                                sync.Serialize(binaryWriter);
                            }

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
                            var transform = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as Transform;
                            if(transform == null)
                            {
                                Debug.LogWarning($"instanceID;{instanceID} is not found.");
                            }
                            else
                            {
                                var sync = new SyncTransform(transform);
                                sync.Deserialize(binaryReader);
                            }                                                           
                        }
                        break;

                    case MessageID.SyncComponent:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            if (m_IsLogEnable)
                            {
                                Debug.Log($"instanceID:{instanceID}");
                            }
                            var component = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as Component;
                            if(component == null)
                            {
                                Debug.LogWarning($"instanceID;{instanceID} is not found.");
                            }
                            else
                            {
                                var sync = new SyncComponent(component);
                                sync.Deserialize(binaryReader);
                                sync.WriteBack();
                            }                                                        
                        }
                        break;

                    case MessageID.SyncDelete:
                        {
                            var instanceID = binaryReader.ReadInt32();
                            if (m_IsLogEnable)
                            {
                                Debug.Log($"instanceID:{instanceID}");
                            }
                            var gameObject = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as GameObject;
                            if(gameObject == null)
                            {
                                Debug.LogWarning($"instanceID;{instanceID} is not found.");
                            }
                            else
                            {
                                GameObject.Destroy(gameObject);
                                binaryWriter.Write((int)MessageID.SyncDelete);
                                binaryWriter.Write(instanceID);
                                SendRemoteMessage(writerMemory.ToArray());
                            }                                                        
                        }
                        break;
                }
            }

            catch(System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {

                Debug.Log($"writerMemorylenght/writerMemory.Position:{writerMemory.Position}/{writerMemory.Length}");

                binaryReader.Close();                
                binaryWriter.Close();
                readerMemory.Close();
                writerMemory.Close();
                readerMemory.Dispose();
                writerMemory.Dispose();
                binaryReader.Dispose();
                binaryWriter.Dispose();

                SyncSceneManager.ClearCache();                
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