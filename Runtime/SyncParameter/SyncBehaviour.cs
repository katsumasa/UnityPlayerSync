using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncBehaviour : SyncComponent
    {
        protected SyncBool m_Enabled;

        public bool enabled
        {
            get { return m_Enabled.value; }
            set { m_Enabled.value = value; m_HasChanded = true; }
        }

        public SyncBehaviour(Behaviour behaviour):base(behaviour)        
        {
            if (behaviour != null)
            {
                m_Enabled = new SyncBool(behaviour.enabled);
            }
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_Enabled.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Enabled.Deserialize(binaryReader);
        }

        public override void WriteBack(Component component)
        {
            base.WriteBack(component);
            var behabiour = component as Behaviour;

            if (m_Enabled.hasChanged)
            {
                behabiour.enabled = m_Enabled.value;
            }
        }

    }
}
