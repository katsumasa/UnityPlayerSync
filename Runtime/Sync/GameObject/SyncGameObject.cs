using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncGameObject : SyncUnityEngineObject
    {
        static List<SyncGameObject> m_Caches;

        private static List<SyncGameObject> Caches
        {
            get
            {
                if(m_Caches == null)
                {
                    m_Caches = new List<SyncGameObject>();
                }
                return m_Caches;
            }
        }


        public static void ClearList()
        {
            Caches.Clear();            
        }

        public static SyncGameObject Find(GameObject gameObject)
        {
            foreach (var sync in Caches)
            {
                if (GameObject.Equals(sync.gameObject, gameObject))
                {
                    return sync;
                }                    
            }
            return null;
        }

        public static SyncGameObject Find(int instanceID)
        {
            foreach (var sync in Caches)
            {
                if(sync.GetInstanceID() == instanceID)
                {
                    return sync;
                }
            }
            return null;
        }


        public static UnityEngine.Object FintObject(int instanceID)
        {
            foreach(var syncGameObject in Caches)
            {
                if(syncGameObject.GetInstanceID() == instanceID)
                {
                    return (UnityEngine.Object)syncGameObject.m_object;
                }
                if(syncGameObject.m_Transform.GetInstanceID() == instanceID)
                {
                    return syncGameObject.m_Transform.GetTransform();
                }
                var component = syncGameObject.GetComponent(instanceID);
                if(component != null)
                {
                    return component;
                }                
            }
            return null;
        }

        public GameObject gameObject
        {
            get { return (GameObject)m_object; }
        }


        protected SyncTransform m_Transform;
        // Transformとそれ以外のコンポーネントで分けます        
        protected int[] m_ComponentInstancIDs;
        protected SyncType[] m_ComponentTypes;
        protected SyncComponent[] m_Components;

        public Component GetComponent(int instanceID)
        {
            for(var i = 0; i < m_ComponentInstancIDs.Length; i++)
            {
                if(m_ComponentInstancIDs[i] == instanceID)
                {
                    return m_Components[i].GetComponent();
                }
            }
            return null;
        }

                            
        public SyncGameObject(object obj):base(obj)
        {
#if !UNITY_EDITOR
            var gameObject = (GameObject)m_object;
            m_Transform = new SyncTransform(gameObject.transform);
            var components = gameObject.GetComponents<Component>();
            m_ComponentInstancIDs = new int[components.Length - 1];
            m_ComponentTypes = new SyncType[components.Length - 1];
            m_Components = new SyncComponent[components.Length - 1];
            
            // 0番目はTransformである為、1番目から処理する
            for(var i= 1; i < components.Length; i++)
            {
                m_ComponentInstancIDs[i - 1] = components[i].GetInstanceID();
                m_ComponentTypes[i-1] = new SyncType(components[i].GetType());
                m_Components[i-1] = new SyncComponent(components[i]);
            }
#endif       
            Caches.Add(this);
        }

        ~SyncGameObject()
        {
            Caches.Remove(this);
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            var gameObject = (GameObject)m_object;

            base.Serialize(binaryWriter);            
            binaryWriter.Write(gameObject.activeSelf);
            binaryWriter.Write(gameObject.isStatic);
            binaryWriter.Write(gameObject.layer);
            binaryWriter.Write(gameObject.tag);            
            m_Transform.Serialize(binaryWriter);
            
            var len = m_Components.Length;
            binaryWriter.Write(len);
            for (var i = 0; i < len; i++)
            {
                binaryWriter.Write(m_ComponentInstancIDs[i]);
            }
            for(var i = 0; i < len; i++)
            {
                m_ComponentTypes[i].Serialize(binaryWriter);                
            }
            for (var i = 0; i < len; i++)
            {
                m_Components[i].Serialize(binaryWriter);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {            
            base.Deserialize(binaryReader);            
            gameObject.SetActive(binaryReader.ReadBoolean());
            gameObject.isStatic = binaryReader.ReadBoolean();
            gameObject.layer = binaryReader.ReadInt32();
            gameObject.tag = binaryReader.ReadString();            
            m_Transform = new SyncTransform(gameObject.transform);
            m_Transform.Deserialize(binaryReader);
            
            
            var len = binaryReader.ReadInt32();
            m_ComponentInstancIDs = new int[len];
            m_ComponentTypes = new SyncType[len];
            m_Components = new SyncComponent[len];
            for(var i = 0; i < len; i++)
            {
                m_ComponentInstancIDs[i] = binaryReader.ReadInt32();
            }                              
            
            for (var i = 0; i < len; i++)
            {                        
                m_ComponentTypes[i] = new SyncType();
                m_ComponentTypes[i].Deserialize(binaryReader);
            }                

            // Componentのデシリアライズ
            var components = gameObject.GetComponents<Component>();
            for (var i = 0; i < len; i++)
            {                                                
                var componentType = SyncType.GetType(m_ComponentTypes[i]);
                var component = gameObject.GetComponent(componentType);
                if (component == null)
                {
                    component = gameObject.AddComponent(componentType);
                }
                m_Components[i] = new SyncComponent(component,false);
                m_Components[i].Deserialize(binaryReader);
            }
        }

        public void WriteBack()
        {       
            for (var i = 0; i < m_Components.Length; i++)
            {
                m_Components[i].WriteBack();
            }
        }

        public void Reset()
        {
            foreach(var component in m_Components)
            {
                component.Reset();
            }
        }


        Component GetComponent(Component[] components,int instanceID)
        {
            foreach(var c in components)
            {
                if(c.GetInstanceID() == instanceID)
                {
                    return c;
                }
            }
            return null;
        }

    }

}