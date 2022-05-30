using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{

    // object = Object
    // Object - GameObject
    // Object - Component - Transform
    // Component - Behaviour

    public abstract class SyncObject 
    {
        protected string m_MemberName;
        
        protected string m_FullName;
        protected string m_Name;
        protected MemberTypes m_MemberType;
        protected TypeCode m_TypeCode;
        protected bool m_HasChanded;
        

        public string fullName
        {
            get { return m_FullName; }
        }

        public virtual string name
        {
            get { return m_Name; }

            set { m_Name = value; }
        }



        public MemberTypes memberType
        {
            get { return m_MemberType; }
            set { m_MemberType = value; }
        }

        public bool hasChanged
        {
            get { return m_HasChanded; }
            set { m_HasChanded = value; }
        }

        public SyncObject(object o)
        {
            if (o != null)
            {
                var t = o.GetType();
                m_FullName = t.FullName;
                
                m_Name = t.Name;
                m_MemberType = t.MemberType;
                m_TypeCode = Type.GetTypeCode(t);
            }
            m_HasChanded = false;
        }

        public virtual void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(m_FullName);
            binaryWriter.Write(m_Name);
            binaryWriter.Write((int)m_MemberType);
            binaryWriter.Write((int)m_TypeCode);
            binaryWriter.Write(m_HasChanded);
        }

        public virtual void Deserialize(BinaryReader binaryReader)
        {
            m_FullName = binaryReader.ReadString();
            m_Name = binaryReader.ReadString();
            m_MemberType = (MemberTypes)binaryReader.ReadInt32();
            m_TypeCode = (TypeCode)binaryReader.ReadInt32();
            m_HasChanded = binaryReader.ReadBoolean();
        }
    }
}
