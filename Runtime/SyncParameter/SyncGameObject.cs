using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncGameObject : SyncUnityEngineObject
    {        
        protected SyncBool m_ActiveSelf;
        protected SyncBool m_IsStatic;
        protected SyncInt m_Layer;        
        protected SyncString m_Tag;
        protected SyncInt m_TransformInstanceID;
        protected SyncTransform m_Transform;

        // Transformとそれ以外のコンポーネントで分けます        
        protected SyncInt[] m_ComponentInstancIDs;
        protected SyncType[] m_ComponentTypes;
        protected SyncComponent[] m_Components;

        // 
        private GameObject m_GameObject;


     
        public bool activeSelf
        {
            get { return m_ActiveSelf.value; }
            set { m_ActiveSelf.value = value; }
        }

        public bool isStatic
        {
            get { return m_IsStatic.value; }
            set { m_IsStatic.value = value; }
        }

        public int layer
        {
            get { return m_Layer.value; }
            set { m_Layer.value = value; }
        }

        public string tag
        {
            get { return m_Tag.value; }
            set { m_Tag.value = value; }
        }

        public SyncGameObject() :base(typeof(GameObject))
        {                        
        }

        public SyncGameObject(GameObject gameObject):base(gameObject)
        {
            m_GameObject = gameObject;                                     
            m_ActiveSelf = new SyncBool(gameObject.activeSelf);
            m_IsStatic = new SyncBool(gameObject.isStatic);
            m_Layer = new SyncInt(gameObject.layer);
            m_Tag = new SyncString(gameObject.tag);
            m_TransformInstanceID = new SyncInt(gameObject.transform.GetInstanceID());
            m_Transform = new SyncTransform(gameObject.transform);


            var components = gameObject.GetComponents<Component>();
            m_ComponentInstancIDs = new SyncInt[components.Length - 1];
            m_ComponentTypes = new SyncType[components.Length - 1];
            m_Components = new SyncComponent[components.Length - 1];
            
            // 0番目はTransformである為、1番目から処理する
            for(var i= 1; i < components.Length; i++)
            {
                m_ComponentInstancIDs[i - 1] = new SyncInt(components[i].GetInstanceID());
                m_ComponentTypes[i-1] = new SyncType(components[i].GetType());
                m_Components[i-1] = new SyncComponent(components[i]);
            }            
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);            
            m_ActiveSelf.Serialize(binaryWriter);
            m_IsStatic.Serialize(binaryWriter);
            m_Layer.Serialize(binaryWriter);
            m_Tag.Serialize(binaryWriter);
            m_TransformInstanceID.Serialize(binaryWriter);
            m_Transform.Serialize(binaryWriter);
            var len = m_Components.Length;
            binaryWriter.Write(len);            
            for(var i = 0; i < len; i++)
            {
                m_ComponentInstancIDs[i].Serialize(binaryWriter);
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

            m_ActiveSelf = new SyncBool();
            m_ActiveSelf.Deserialize(binaryReader);

            m_IsStatic = new SyncBool();
            m_IsStatic.Deserialize(binaryReader);

            m_Layer = new SyncInt();
            m_Layer.Deserialize(binaryReader);

            m_Tag = new SyncString();
            m_Tag.Deserialize(binaryReader);
            
            m_TransformInstanceID = new SyncInt();
            m_TransformInstanceID.Deserialize(binaryReader);

            if (m_GameObject == null)
            {
                var transform = SyncTransform.GetTransform(m_TransformInstanceID.value);
                if (transform == null)
                {
                    m_GameObject = new GameObject();
                }
                else
                {
                    m_GameObject = transform.gameObject;
                }
            }
            m_Transform = new SyncTransform(m_GameObject.transform);
            m_Transform.Deserialize(binaryReader);
            var len = binaryReader.ReadInt32();
            m_ComponentInstancIDs = new SyncInt[len];
            m_ComponentTypes = new SyncType[len];
            m_Components = new SyncComponent[len];
            for (var i = 0; i < len; i++)
            {
                m_ComponentInstancIDs[i] = new SyncInt();
                m_ComponentInstancIDs[i].Deserialize(binaryReader);
                m_ComponentTypes[i] = new SyncType();
                m_ComponentTypes[i].Deserialize(binaryReader);
            }                

            // Componentのデシリアライズ
            var components = m_GameObject.GetComponents<Component>();
            for (var i = 0; i < m_Components.Length; i++)
            {                
                // ComponentのInstanceIDをキーにして
                var component = GetComponent(components, m_ComponentInstancIDs[i].value);                
                if (component == null)
                {
                    // GameObjectに指定されたComponentが無い場合は、新規に作成する
                    var componentType = SyncType.GetType(m_ComponentTypes[i]);
                    component = m_GameObject.AddComponent(componentType);
                }
                m_Components[i] = new SyncComponent(component);
                m_Components[i].Deserialize(binaryReader);
            }
        }

        public override void WriteBack()
        {
            base.WriteBack();
            if (m_ActiveSelf.hasChanged)
            {
                m_GameObject.SetActive(m_ActiveSelf.value);
            }
            if(m_IsStatic.hasChanged)
            {
                m_GameObject.isStatic = m_IsStatic.value;
            }
            if(m_Layer.hasChanged)
            {
                m_GameObject.hideFlags = (HideFlags)m_HideFlags.value;
            }
            if(m_Tag.hasChanged)
            {
                m_GameObject.tag = m_Tag.value;
            }
            if(m_Transform.hasChanged)
            {
                m_Transform.WriteBack();
            }                                   
            for (var i = 0; i < m_Components.Length; i++)
            {
                m_Components[i].WriteBack();
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