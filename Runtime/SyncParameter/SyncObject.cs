using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public abstract class Sync
    {
        public virtual void Serialize(BinaryWriter binaryWriter) { }
        public virtual void Deserialize(BinaryReader binaryReader) { }
    }

    public class SyncAssembly : Sync
    {
        protected string m_FullName;

        public string FullName
        {
            get { return m_FullName; }
        }

        public SyncAssembly() { }

        public SyncAssembly(Assembly assembly)
        {
            m_FullName = assembly.FullName;
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_FullName);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_FullName = binaryReader.ReadString();
        }
    }


    public class SyncMemberInfo : Sync
    {
        protected MemberTypes m_MemberTypes;

        public MemberTypes MemberTypes
        {
            get { return m_MemberTypes; }
        }

        public SyncMemberInfo() { }

        public SyncMemberInfo(MemberInfo memberInfo)
        {
            m_MemberTypes = memberInfo.MemberType;
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write((int)m_MemberTypes);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_MemberTypes = (MemberTypes)binaryReader.ReadInt32();
        }
    }

    public class SyncPropertyInfo : SyncMemberInfo
    {
        protected bool m_CanRead;
        protected bool m_CanWrite;
        protected string m_Name;
        protected string m_PropertyType;

        public bool CanRead
        {
            get { return m_CanRead; }
        }

        public bool CanWrite
        {
            get { return m_CanWrite; }
        }

        public string Name
        {
            get { return m_Name; }
        }


        public string PropertyType
        {
            get { return m_PropertyType; }
        }

        public SyncPropertyInfo() : base() { }

        public SyncPropertyInfo(PropertyInfo propInfo) : base(propInfo)
        {
            m_CanRead = propInfo.CanRead;
            m_CanWrite = propInfo.CanWrite;
            m_Name = propInfo.Name;
            m_PropertyType = propInfo.PropertyType.Name;
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_CanRead);
            binaryWriter.Write(m_CanWrite);
            binaryWriter.Write(m_Name);
            binaryWriter.Write(m_PropertyType);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_CanRead = binaryReader.ReadBoolean();
            m_CanWrite = binaryReader.ReadBoolean();
            m_Name = binaryReader.ReadString();
            m_PropertyType = binaryReader.ReadString();
        }
    }


    public class SyncType : Sync
    {
        public static TypeCode GetTypeCode(SyncType syncType)
        {
            switch(syncType.m_FullName)
            {
                case "System.Empty":    return TypeCode.Empty;
                case "System.Object":   return TypeCode.Object;
                case "System.DBNull":   return TypeCode.DBNull;
                case "System.Boolean":  return TypeCode.Boolean;
                case "System.Char":     return TypeCode.Char;
                case "System.SByte":    return TypeCode.SByte;
                case "System.Byte":     return TypeCode.Byte;
                case "System.Int16":    return TypeCode.Int16;
                case "System.UInt16":   return TypeCode.UInt16;
                case "System.Int32":    return TypeCode.Int32;
                case "System.UInt32":   return TypeCode.UInt32;
                case "System.Int64":    return TypeCode.Int64;
                case "System.UInt64":   return TypeCode.UInt64;
                case "System.Single":   return TypeCode.Single;
                case "System.Double":   return TypeCode.Double;
                case "System.Decimal":  return TypeCode.Decimal;
                case "System.DateTime": return TypeCode.DateTime;
                case "System.String":   return TypeCode.String;                
            }
            return TypeCode.Object;
        }

        public static System.Type GetType(SyncType type)
        {
            // å^Ç∆AssemblyÇÃÉtÉãÉlÅ[ÉÄÇ©ÇÁå^ÇéÊìæÇ∑ÇÈ
            var typeName = $"{type.FullName},{type.Assembly.FullName}";
            return System.Type.GetType(typeName);
        }


        protected string m_FullName;        
        protected bool m_IsArray;
        protected MemberTypes m_MemberType;
        protected string m_Name;
        public SyncAssembly m_Assembly;


        public string FullName
        {
            get { return m_FullName; }
        }

        public bool IsArray
        {
            get { return m_IsArray; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public MemberTypes MemberTypes
        {
            get { return m_MemberType; }
        }

        public SyncAssembly Assembly
        {
            get { return m_Assembly; }
        }
        

        public SyncType() 
        {
            m_Assembly = new SyncAssembly();
        }

        public SyncType(System.Type type)
        {
            m_FullName = type.FullName;
            m_IsArray = type.IsArray;
            m_MemberType = type.MemberType;
            m_Name = type.Name;
            m_Assembly = new SyncAssembly(type.Assembly);
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(m_FullName);
            binaryWriter.Write(m_IsArray);
            binaryWriter.Write((int)m_MemberType);
            binaryWriter.Write(m_Name);
            m_Assembly.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            m_FullName = binaryReader.ReadString();
            m_IsArray = binaryReader.ReadBoolean();
            m_MemberType = (MemberTypes)binaryReader.ReadInt32();
            m_Name = binaryReader.ReadString();
            m_Assembly.Deserialize(binaryReader);
        }
    }

    
    public class SyncObject
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


        public SyncObject(object o)
        {            
            if (o != null)
            {
                var t = o.GetType();                
                m_Type = new SyncType(t);
            }
            m_HasChanded = true;
        }
        

        public SyncObject(object[] objects):this(objects[0])
        {
            if (objects != null)
            {
                var t = objects.GetType();
                m_Type = new SyncType(t);                
            }
            m_HasChanded = true;
        }


        public virtual void Serialize(BinaryWriter binaryWriter)
        {
            m_Type.Serialize(binaryWriter);
            binaryWriter.Write(m_HasChanded);
        }


        public virtual void Deserialize(BinaryReader binaryReader)
        {
            m_Type.Deserialize(binaryReader);            
            m_HasChanded = binaryReader.ReadBoolean();
        }
    }
}
