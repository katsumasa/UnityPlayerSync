using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncUnityObject : SyncObject
    {
        protected SyncInt m_HideFlags;
        protected SyncInt m_InstanceID;
        protected SyncString m_ObjectName;

        public HideFlags hideFlags
        {
            get
            {
                return (HideFlags)m_HideFlags.value;
            }

            set
            {
                m_HideFlags.value = (int)value;
                m_HasChanded = true;
            }
                    
        }

        public override string name { get => m_ObjectName.name; set { m_ObjectName.name = value; } }



        public SyncUnityObject(UnityEngine.Object @object):base(@object)
        {
            if (@object != null)
            {
                m_HideFlags = new SyncInt((int)@object.hideFlags);
                m_InstanceID = new SyncInt(@object.GetInstanceID());
                m_ObjectName = new SyncString(@object.name);
            }
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_HideFlags.Serialize(binaryWriter);
            m_InstanceID.Serialize(binaryWriter);

        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_HideFlags.Deserialize(binaryReader);
            m_InstanceID.Deserialize(binaryReader);
        }
    }
}