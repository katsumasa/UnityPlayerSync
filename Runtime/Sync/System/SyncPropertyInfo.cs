using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncPropertyInfo : SyncMemberInfo
    {        
        protected SyncType m_PropertyType;
             

        public SyncType PropertyType
        {
            get { return m_PropertyType; }
        }

        public SyncPropertyInfo() : base(typeof(PropertyInfo)) { }

        public SyncPropertyInfo(PropertyInfo propInfo) : base(propInfo)
        {
            if (propInfo != null)
            {               
                m_PropertyType = new SyncType(propInfo.PropertyType);
            }
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            var hash = m_PropertyType.GetHashCode();
            binaryWriter.Write(hash);
            if(SyncTypeTree.Instances.ContainsKey(hash) == false)
            {
                SyncTypeTree.Instances.Add(hash, m_PropertyType);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var hash = binaryReader.ReadInt32();            
            m_PropertyType = SyncTypeTree.Instances[hash];
        }

        public override void Dispose()
        {            
            base.Dispose();            
            m_PropertyType = null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyncPropertyInfo;
            if(other == null)
            {
                return false;
            }
            if(base.Equals(other) == false)
            {
                return false;
            }
            if(m_PropertyType != other.m_PropertyType)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hash  = base.GetHashCode();
            hash = (hash * 397) ^ m_PropertyType.GetHashCode();
            return hash;
        }
    }
}