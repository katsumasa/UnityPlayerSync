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
                var key = propInfo.PropertyType.FullName;
                if (SyncType.Caches.ContainsKey(key) == false)
                {
                    var syncType = new SyncType(propInfo.PropertyType);
                    SyncType.Caches.Add(syncType.FullName, syncType);
                }
                m_PropertyType = SyncType.Caches[key];
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_PropertyType.Serialize(binaryWriter);
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var syncType = new SyncType();
            syncType.Deserialize(binaryReader);                       
            if (SyncType.Caches.ContainsKey(syncType.FullName) == false)
            {
                SyncType.Caches.Add(syncType.FullName, syncType);                                
            }
            m_PropertyType = SyncType.Caches[syncType.FullName];
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
            var hash128 = new Hash128();
            hash128.Append(base.GetHashCode());
            hash128.Append(m_PropertyType.GetHashCode());
            return hash128.GetHashCode();            
        }

    }
}