using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncPropertyInfo : SyncMemberInfo
    {
        protected bool m_CanRead;
        protected bool m_CanWrite;
        protected SyncType m_PropertyType;
        


        public bool CanRead
        {
            get { return m_CanRead; }
        }


        public bool CanWrite
        {
            get { return m_CanWrite; }
        }


        public SyncType PropertyType
        {
            get { return m_PropertyType; }
        }


        public SyncPropertyInfo() : base(typeof(PropertyInfo)) { }


        public SyncPropertyInfo(PropertyInfo propInfo) : base(propInfo)
        {
            m_CanRead = propInfo.CanRead;
            m_CanWrite = propInfo.CanWrite;
            m_PropertyType = new SyncType(propInfo.PropertyType);
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_CanRead);
            binaryWriter.Write(m_CanWrite);
            m_PropertyType.Serialize(binaryWriter);            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_CanRead = binaryReader.ReadBoolean();
            m_CanWrite = binaryReader.ReadBoolean();
            m_PropertyType = new SyncType();
            m_PropertyType.Deserialize(binaryReader);
        }
    }
}