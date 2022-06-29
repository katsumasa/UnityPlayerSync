using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncFieldInfo : SyncMemberInfo
    {
        protected bool m_IsNotSerialized;
        protected bool m_IsPrivate;
        protected bool m_IsPublic;
        protected bool m_IsStatic;
        protected SyncType m_FieldType;


        public bool IsNotSerialized
        {
            get { return m_IsNotSerialized; }
        }

        public bool IsPrivate
        {
            get { return m_IsPrivate; }
        }

        public bool IsPublic
        {
            get { return m_IsPublic; }
        }

        public bool IsStatic
        {
            get { return m_IsStatic; }
        }

        public SyncType FieldType
        {
            get { return m_FieldType; }
        }

        public SyncFieldInfo() : base(typeof(FieldInfo)){ }

        public SyncFieldInfo(FieldInfo fieldInfo) : base(fieldInfo)
        {
            m_IsNotSerialized = fieldInfo.IsNotSerialized;
            m_IsPrivate = fieldInfo.IsPrivate;
            m_IsPublic = fieldInfo.IsPublic;
            m_IsStatic = fieldInfo.IsStatic;
            m_FieldType = new SyncType(fieldInfo.FieldType);
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_IsNotSerialized);
            binaryWriter.Write(m_IsPrivate);
            binaryWriter.Write(m_IsPublic);
            binaryWriter.Write(m_IsStatic);
            m_FieldType.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_IsNotSerialized = binaryReader.ReadBoolean();
            m_IsPrivate = binaryReader.ReadBoolean();
            m_IsPublic = binaryReader.ReadBoolean();
            m_IsStatic = binaryReader.ReadBoolean();
            m_FieldType = new SyncType();
            m_FieldType.Deserialize(binaryReader);
        }
    }
}