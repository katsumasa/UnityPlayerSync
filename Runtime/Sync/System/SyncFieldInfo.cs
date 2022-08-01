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
            var hash = m_FieldType.GetHashCode();
            binaryWriter.Write(hash);
            if(SyncTypeTree.Instances.ContainsKey(hash) == false)
            {
                SyncTypeTree.Instances.Add(hash, m_FieldType);
            }
            else
            {
                m_FieldType = SyncTypeTree.Instances[hash];
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_IsNotSerialized = binaryReader.ReadBoolean();
            m_IsPrivate = binaryReader.ReadBoolean();
            m_IsPublic = binaryReader.ReadBoolean();
            m_IsStatic = binaryReader.ReadBoolean();
            var hash = binaryReader.ReadInt32();                                    
            m_FieldType = SyncTypeTree.Instances[hash];
        }

        public override void Dispose()
        {            
            base.Dispose();
            m_FieldType = null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyncFieldInfo;
            if(other == null)
            {
                return false;
            }
            if(base.Equals(obj) == false)
            {
                return false;
            }
            if (m_IsNotSerialized.Equals(other.m_IsNotSerialized) == false)
            {
                return false;
            }
            if(m_IsPrivate.Equals(other.m_IsPrivate) == false)
            {
                return false;
            }
            if(m_IsPublic.Equals(other.m_IsPublic) == false)
            {
                return false;
            }
            if(m_IsStatic.Equals(other.m_IsStatic) == false)
            {
                return false;
            }
            if(m_FieldType.Equals(other.m_FieldType) == false)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = (hash * 397) ^ m_IsNotSerialized.GetHashCode();
            hash = (hash * 397) ^ m_IsPrivate.GetHashCode();
            hash = (hash * 397) ^ m_IsPublic.GetHashCode();
            hash = (hash * 397) ^ m_IsStatic.GetHashCode();
            hash = (hash * 397) ^ m_FieldType.GetHashCode();
            return hash;
        }        
    }
}