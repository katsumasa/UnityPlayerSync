using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncGameObject : SyncUnityObject
    {
        protected SyncBool m_ActiveInHierarchy;
        protected SyncBool m_ActiveSelf;
        protected SyncBool m_IsStatic;
        protected SyncInt m_Layer;        
        protected SyncString m_Tag;
        protected SyncComponent[] m_Components;

        public bool activeInHierarchy
        {
            get { return m_ActiveInHierarchy.value; }
            set { m_ActiveInHierarchy.value = value; }
        }

        public bool activeSelf
        {
            get { return m_ActiveSelf.value; }
            set { m_ActiveSelf.value = value; }
        }



        public SyncGameObject(GameObject gameObject):base(gameObject)
        {
            if(gameObject != null)
            {
                m_ActiveInHierarchy = new SyncBool(gameObject.activeInHierarchy);
                m_ActiveSelf = new SyncBool(gameObject.activeSelf);
                m_IsStatic = new SyncBool(gameObject.isStatic);
                m_Layer = new SyncInt(gameObject.layer);
                m_Tag = new SyncString(gameObject.tag);


                var components = gameObject.GetComponents<Component>();

                m_Components = new SyncComponent[components.Length];
                for(var i= 0; i < components.Length; i++)
                {
                    m_Components[i] = new SyncComponent(components[i]);
                }

            }
        }
            
        
    }

}