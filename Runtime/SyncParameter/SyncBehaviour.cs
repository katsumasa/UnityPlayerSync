using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncBehaviour : SyncComponent
    {
        protected SyncBool m_Enabled;
        private Behaviour m_Behaviour;

        public bool enabled
        {
            get { return m_Enabled.value; }
            set { m_Enabled.value = value; m_HasChanded = true; }
        }

        public SyncBehaviour(Behaviour behaviour):base(behaviour)        
        {
            m_Behaviour = behaviour;
            m_Enabled = new SyncBool(behaviour.enabled);
            
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

        public override void WriteBack()
        {
            base.WriteBack();            
            if (m_Enabled.hasChanged)
            {
                m_Behaviour.enabled = m_Enabled.value;
            }
        }

    }
}
