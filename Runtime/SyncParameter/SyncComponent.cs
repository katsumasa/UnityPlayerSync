using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncComponent : SyncUnityObject
    {
        protected SyncInt m_GameObjectInstanceID;
        protected SyncString m_Tag;


        public SyncComponent(Component component) : base(component)
        {
            if(component != null)
            {
                m_GameObjectInstanceID = new SyncInt(component.gameObject.GetInstanceID());
                m_Tag = new SyncString(component.tag);
                
            }
        }

        public virtual void WriteBack(Component component)
        {
            if (m_Tag.hasChanged)
            {
                component.tag = m_Tag.value;
            }
        }
    }
}
