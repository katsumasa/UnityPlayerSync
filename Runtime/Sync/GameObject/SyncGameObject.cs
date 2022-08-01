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


        public static void ClearCache()
        {
            while(Caches.Count > 0)
            {
                Caches[0].Dispose();
            }
            Caches.Clear();
            m_Caches = null;
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
            if(instanceID == SyncUnityEngineObject.InstanceID_None)
            {
                return null;
            }

            foreach (var sync in Caches)
            {
                if(sync.GetInstanceID() == instanceID)
                {
                    return sync;
                }
            }
            return null;
        }

        public static SyncGameObject FindEditor(int instanceEditorID)
        {
            if (instanceEditorID == SyncUnityEngineObject.InstanceID_None)
            {
                return null;
            }
            foreach (var sync in Caches)
            {
                if (sync.GetInstanceEditorID() == instanceEditorID)
                {
                    return sync;
                }
            }
            return null;
        }


        public static UnityEngine.Object FintObject(int instanceID)
        {
            if (instanceID == SyncUnityEngineObject.InstanceID_None)
            {
                return null;
            }

            foreach (var syncGameObject in Caches)
            {
                if((syncGameObject.GetInstanceID() == instanceID)||
                   (syncGameObject.GetInstanceEditorID() == instanceID))
                {
                    return (UnityEngine.Object)syncGameObject.m_object;
                }
                if((syncGameObject.m_Transform.GetInstanceID() == instanceID)||
                    (syncGameObject.m_Transform.GetInstanceEditorID() == instanceID))
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
            if (instanceID == SyncUnityEngineObject.InstanceID_None)
            {
                return null;
            }

            for (var i = 0; i < m_ComponentInstancIDs.Length; i++)
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
            Caches.Add(this);
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {           
            var gameObject = (GameObject)m_object;
            // Transform/RecTransform
            var rectTransform = gameObject.transform as RectTransform;
            if(m_Transform != null)
            {
                m_Transform.Dispose();
            }
            if (rectTransform == null)
            {
                m_Transform = new SyncTransform(gameObject.transform);
            }
            else
            {
                m_Transform = new SyncRectTransform(gameObject.transform as RectTransform);
            }
            
            if(m_Components != null)
            {
                foreach(var component in m_Components)
                {
                    if(component != null)
                    {
                        component.Dispose();
                    }
                }
            }
            // Transformの分を抜いておく
            var components = gameObject.GetComponents<Component>();
            m_ComponentInstancIDs = new int[components.Length - 1];
            m_ComponentTypes = new SyncType[components.Length - 1];
            m_Components = new SyncComponent[components.Length - 1];
            // RequireComponentアトリビュートを考慮してComponentの順番を並び変える
            var componentComparables = new List<ComponentComparable>();
            // 0番目はTransformである為、1番目から処理する            
            for (var i = 1; i < components.Length; i++)
            {
                componentComparables.Add(new ComponentComparable(components[i]));
            }
            componentComparables.Sort();
            for(var i = 0; i < componentComparables.Count; i++)
            {
                var component = componentComparables[i].component;             
                m_ComponentInstancIDs[i] = component.GetInstanceID();                
                m_ComponentTypes[i] = new SyncType(component.GetType());
                m_Components[i] = new SyncComponent(component);                                
            }                        
            base.Serialize(binaryWriter);            
            // GameObject固有
            binaryWriter.Write(gameObject.activeSelf);
            binaryWriter.Write(gameObject.isStatic);
            binaryWriter.Write(gameObject.layer);
            binaryWriter.Write(gameObject.tag); 
            // Transform / RecTransform識別子
            if (rectTransform == null)
            {
                binaryWriter.Write(0);
            }
            else
            {
                binaryWriter.Write(1);
            }
           
            m_Transform.Serialize(binaryWriter);
            
            var len = m_Components.Length;
            binaryWriter.Write(len);
            for (var i = 0; i < len; i++)
            {
                binaryWriter.Write(m_ComponentInstancIDs[i]);
            }
            for(var i = 0; i < len; i++)
            {
                var hash = m_ComponentTypes[i].GetHashCode();
                binaryWriter.Write(hash);
                if(SyncTypeTree.Instances.ContainsKey(hash) == false)
                {
                    SyncTypeTree.Instances.Add(hash, m_ComponentTypes[i]);
                }
                else
                {
                    m_ComponentTypes[i] = SyncTypeTree.Instances[hash];
                }
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
            int rectMode = binaryReader.ReadInt32();
            if (m_Transform != null)
            {
                m_Transform.Dispose();
            }
            if (rectMode == 0)
            {
                var rectTransform = gameObject.transform as RectTransform;
                if (rectTransform != null)
                {
                    gameObject.AddComponent<Transform>();
                }                
                m_Transform = new SyncTransform(gameObject.transform);
            }
            else
            {
                var rectTransform = gameObject.transform as RectTransform;
                if(rectTransform == null)
                {
                    gameObject.AddComponent<RectTransform>();
                }
                m_Transform = new SyncRectTransform(gameObject.transform as RectTransform);
            }
            m_Transform.Deserialize(binaryReader);
            
            // 既存のSyncComponentをDisposeする
            if (m_Components != null)
            {
                for(var i = 0; i < m_Components.Length; i++)
                {
                    if (m_Components[i] != null)
                    {
                        m_Components[i].Dispose();
                    }
                }
            }

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
                var hash = binaryReader.ReadInt32();                
                m_ComponentTypes[i] = SyncTypeTree.Instances[hash];
            }
            for (var i = 0; i < len; i++)
            {
                var componentType = SyncType.GetType(m_ComponentTypes[i]);
                var component = gameObject.GetComponent(componentType);
                if (component == null)
                {
                    component = gameObject.AddComponent(componentType);
                }
                m_Components[i] = new SyncComponent(component, false);
                m_Components[i].Deserialize(binaryReader);
            }            
            
            // 相手側で削除されたコンポーネントをこちらでも削除する
            var components = gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                // Transformは除く
                var t = component.GetType();
                if((t == typeof(Transform)) || (t == typeof(RectTransform)))
                {
                    continue;
                }
                var isDelete = true;
                for (var i = 0; i < len; i++)
                {
                    var componentType = SyncType.GetType(m_ComponentTypes[i]);
                    if(t == componentType)
                    {
                        isDelete = false;
                        break;
                    }
                }
                if (isDelete)
                {
                   // Debug.Log($"(GameObject:{gameObject.name} Component:{t.Name}) is destoyed.");
#if UNITY_EDITOR
                    Object.DestroyImmediate(component);
#else
                    Component.Destroy(component);
#endif
                }
            }
        }

        public void WriteBack()
        {       
            for (var i = 0; i < m_Components.Length; i++)
            {
                m_Components[i].WriteBack();
            }
        }

        public override void Reset()
        {
            base.Reset();
            foreach(var component in m_Components)
            {
                component.Init();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if(m_Transform != null)
            {
                m_Transform.Dispose();
            }
            if (m_Components != null)
            {
                for (var i = 0; i < m_Components.Length; i++)
                {
                    if (m_Components[i] != null)
                    {
                        m_Components[i].Dispose();
                    }
                }
            }
            if (Caches.Contains(this))
            {
                Caches.Remove(this);
            }
        }
    }

    public class ComponentComparable : System.IComparable<ComponentComparable>
    {        
        public Component component
        {
            get { return m_Component; }
        }
        Component m_Component;
        System.Type m_Type;


        public ComponentComparable(Component component)
        {
            m_Component = component;
            m_Type = m_Component.GetType();
        }

        public int CompareTo(ComponentComparable other)
        {           
            if (IsRequireComponent(m_Type, other.m_Type))
            {
                return 1;
            }
            if (other.IsRequireComponent(other.m_Type, m_Type))
            {
                return -1;
            }
            var requireComponents = m_Type.GetCustomAttributes(typeof(RequireComponent), true) as RequireComponent[];
            var otherRequireComponents = other.m_Type.GetCustomAttributes(typeof(RequireComponent), true) as RequireComponent[];
            return requireComponents.Length - otherRequireComponents.Length;
        }

        private bool IsRequireComponent(System.Type type, System.Type otherType)
        {
            var requireComponents = type.GetCustomAttributes(typeof(RequireComponent), true) as RequireComponent[];
            if (requireComponents == null)
            {
                return false;
            }
            while (otherType != null)
            {
                foreach (var requireComponent in requireComponents)
                {
                    if ((requireComponent.m_Type0 == otherType) || (requireComponent.m_Type1 == otherType) || (requireComponent.m_Type2 == otherType))
                    {
                        return true;
                    }
                }
                otherType = otherType.BaseType;
            }
            return false;
        }
    }

}