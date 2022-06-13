using System;
using System.IO;


namespace UTJ.UnityPlayerSyncEngine
{        
    public class SyncObject : Sync
    {
        protected bool m_HasChanded;
        protected SyncType m_Type;


        public bool hasChanged
        {
            get { return m_HasChanded; }
            set { m_HasChanded = value; }
        }


        public SyncType Type
        {
            get { return m_Type; }
        }                


        public SyncObject(Type type)
        {            
            m_Type = new SyncType(type);
            m_HasChanded = true;
        }

        public SyncObject()
        {
            m_Type = new SyncType();
            m_HasChanded = true;
        }


        public SyncObject(object o)
        {
            if (o == null)
            {
                m_Type = new SyncType();
            }
            else
            {
                var t = o.GetType();
                m_Type = new SyncType(t);
            }
            m_HasChanded = true;
        }
                


        public override void Serialize(BinaryWriter binaryWriter)
        {
            m_Type.Serialize(binaryWriter);
            binaryWriter.Write(m_HasChanded);
        }


        public override void Deserialize(BinaryReader binaryReader)
        {            
            m_Type.Deserialize(binaryReader);            
            m_HasChanded = binaryReader.ReadBoolean();
        }

        public virtual object GetValue()
        {
            return null;
        }
    }
}
