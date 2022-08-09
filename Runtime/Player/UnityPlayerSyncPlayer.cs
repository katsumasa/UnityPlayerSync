//
// Programed by Katsumasa Kimura
//
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTJ.UnityPlayerSync.Runtime 
{
#if UNITY_EDITOR
    [CustomEditor(typeof(UnityPlayerSyncPlayer))]
    [CanEditMultipleObjects]
    public class UnityPlayerSyncPlayerEditor :Editor
    {
        static class Styles
        {
            public static readonly GUIContent UseFixedStreamBuffer = new GUIContent("Use Stream Buffer Capacity", "Initializes a new instance of the MemoryStream class with an expandable capacity that is initialized according to the specification.");
            public static readonly GUIContent StreamBufferCapacitySize = new GUIContent("Capacity Size[MB]", "");
        }


        SerializedProperty m_UseFixedStreamBuffer;
        SerializedProperty  m_StreamBufferCapacityMB;
        
        void OnEnable()
        {
            m_UseFixedStreamBuffer = serializedObject.FindProperty("m_UseFixedStreamBuffer");
            m_StreamBufferCapacityMB = serializedObject.FindProperty("m_StreamBufferCapacityMB");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();                        
            EditorGUILayout.PropertyField(m_UseFixedStreamBuffer,Styles.UseFixedStreamBuffer);
            if (m_UseFixedStreamBuffer.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_StreamBufferCapacityMB,Styles.StreamBufferCapacitySize);                
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    /// <summary>
    /// Player側の中核
    /// </summary>
    public class UnityPlayerSyncPlayer : UTJ.RemoteConnect.Player
    {
        [SerializeField,Tooltip("Enable log output.")] bool m_EnableLog;
        [SerializeField,Tooltip("Enable DontDestroyOnLoad")] bool m_EnableDontDestroyOnLoad;
        [HideInInspector,SerializeField] bool m_UseFixedStreamBuffer = false;
        [HideInInspector,SerializeField, Range(1,500)] int m_StreamBufferCapacityMB = 1;
        

        private void Start()
        {
            if (m_EnableDontDestroyOnLoad)
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
            try
            {
                using (var readerMemory = new MemoryStream(bytes))
                {
                    using (var binaryReader = new BinaryReader(readerMemory))
                    {

                        var messageID = (MessageID)binaryReader.ReadInt32();
                        if (m_EnableLog)
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
                                    using (var writerMemory = m_UseFixedStreamBuffer ? new MemoryStream(m_StreamBufferCapacityMB * 1024 * 1024) : new MemoryStream())
                                    {
                                        using (var binaryWriter = new BinaryWriter(writerMemory))
                                        {
                                            binaryWriter.Write((int)MessageID.SyncScene);
                                            var ofsetPos = binaryWriter.BaseStream.Position;
                                            binaryWriter.Write((long)0);
                                            
                                            var syncSceneManager = new SyncSceneManager(true);
                                            syncSceneManager.Serialize(binaryWriter);

                                            var typeTreePos = binaryWriter.BaseStream.Position;
                                            SyncTypeTree.Serialize(binaryWriter);                                            
                                            binaryWriter.BaseStream.Position = ofsetPos;
                                            binaryWriter.Write(typeTreePos);                                                                                           

                                            var vs = writerMemory.GetBuffer();
                                            SendRemoteMessage(vs);
                                        }
                                    }
                                }
                                break;

                            case MessageID.SyncGameObject:
                                {
                                    var ofst = binaryReader.ReadInt64();
                                    var returnPos = binaryReader.BaseStream.Position;
                                    binaryReader.BaseStream.Seek(ofst, SeekOrigin.Begin);
                                    SyncTypeTree.Deserialize(binaryReader);                                                                        
                                    binaryReader.BaseStream.Seek(returnPos, SeekOrigin.Begin);

                                    var syncs = new List<SyncGameObject>();
                                    var count = binaryReader.ReadInt32();
                                    for (var i = 0; i < count; i++)
                                    {
                                        var instanceID = binaryReader.ReadInt32();
                                        if (m_EnableLog)
                                        {
                                            Debug.Log($"instanceID:{instanceID}");
                                        }

                                        var go = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as GameObject;
                                        if (go == null)
                                        {
                                            if (m_EnableLog)
                                            {
                                                Debug.Log($"{instanceID} is new GameObject.");
                                            }
                                            go = new GameObject();
                                        }
                                        SyncGameObject sync = new SyncGameObject(go);
                                        sync.Deserialize(binaryReader);
                                        syncs.Add(sync);
                                    }

                                    foreach (var sync in syncs)
                                    {
                                        sync.WriteBack();
                                    }
                                    // Player側の情報でEditor側に送信し直す
                                    foreach (var sync in syncs)
                                    {
                                        sync.Reset();
                                    }

                                    using (var writerMemory = m_UseFixedStreamBuffer ? new MemoryStream(m_StreamBufferCapacityMB * 1024 * 1024) : new MemoryStream())
                                    {
                                        using (var binaryWriter = new BinaryWriter(writerMemory))
                                        {                                            
                                            binaryWriter.Write((int)MessageID.SyncGameObject);

                                            var ofsetPos = binaryWriter.BaseStream.Position;
                                            binaryWriter.Write((long)0);

                                            binaryWriter.Write(count);
                                            foreach (var sync in syncs)
                                            {
                                                // Editor側のInstanceIDを使用
                                                if (m_EnableLog){
                                                    Debug.Log($"{sync.GetInstanceID()},{sync.GetInstanceEditorID()}");
                                                }
                                                binaryWriter.Write(sync.GetInstanceEditorID());
                                                sync.Serialize(binaryWriter);
                                            }
                                            // TypeTree
                                            var typeTreePos = binaryWriter.BaseStream.Position;
                                            SyncTypeTree.Serialize(binaryWriter);                                            
                                            binaryWriter.BaseStream.Position = ofsetPos;
                                            binaryWriter.Write(typeTreePos);                                                                                           
                                            SendRemoteMessage(writerMemory.ToArray());
                                        }
                                    }
                                }
                                break;

                            case MessageID.SyncTransform:
                                {
                                    {
                                        var ofst1 = binaryReader.ReadInt64();
                                        var ofst2 = binaryReader.BaseStream.Position;
                                        binaryReader.BaseStream.Seek(ofst1, SeekOrigin.Begin);
                                        SyncTypeTree.Deserialize(binaryReader);
                                        binaryReader.BaseStream.Seek(ofst2, SeekOrigin.Begin);
                                    }
                                    var instanceID = binaryReader.ReadInt32();
                                    if (m_EnableLog)
                                    {
                                        Debug.Log($"instanceID:{instanceID}");
                                    }
                                    var transform = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as Transform;
                                    if (transform == null)
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
                                    {
                                        var ofst1 = binaryReader.ReadInt64();
                                        var ofst2 = binaryReader.BaseStream.Position;
                                        binaryReader.BaseStream.Seek(ofst1, SeekOrigin.Begin);
                                        SyncTypeTree.Deserialize(binaryReader);
                                        binaryReader.BaseStream.Seek(ofst2, SeekOrigin.Begin);
                                    }
                                    var instanceID = binaryReader.ReadInt32();
                                    if (m_EnableLog)
                                    {
                                        Debug.Log($"instanceID:{instanceID}");
                                    }
                                    var component = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as Component;
                                    if (component == null)
                                    {
                                        Debug.LogWarning($"instanceID:{instanceID} is not found.");
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
                                    if (m_EnableLog)
                                    {
                                        Debug.Log($"instanceID:{instanceID}");
                                    }
                                    var gameObject = SyncUnityEngineObject.FindObjectFromInstanceID(instanceID) as GameObject;
                                    if (gameObject == null)
                                    {
                                        Debug.LogWarning($"instanceID:{instanceID} is not found.");
                                    }
                                    else
                                    {
                                        using (var writerMemory = m_UseFixedStreamBuffer ? new MemoryStream(m_StreamBufferCapacityMB * 1024 * 1024) : new MemoryStream())
                                        {
                                            using (var binaryWriter = new BinaryWriter(writerMemory))
                                            {
                                                GameObject.Destroy(gameObject);
                                                binaryWriter.Write((int)MessageID.SyncDelete);
                                                binaryWriter.Write(instanceID);
                                                SendRemoteMessage(writerMemory.ToArray());
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
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