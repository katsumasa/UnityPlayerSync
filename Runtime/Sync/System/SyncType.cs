using System;
using System.IO;
using System.Reflection;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncType : Sync
    {
        public static TypeCode GetTypeCode(SyncType syncType)
        {
            switch (syncType.m_FullName)
            {
                case "System.Empty": return TypeCode.Empty;
                case "System.Object": return TypeCode.Object;
                case "System.DBNull": return TypeCode.DBNull;
                case "System.Boolean": return TypeCode.Boolean;
                case "System.Char": return TypeCode.Char;
                case "System.SByte": return TypeCode.SByte;
                case "System.Byte": return TypeCode.Byte;
                case "System.Int16": return TypeCode.Int16;
                case "System.UInt16": return TypeCode.UInt16;
                case "System.Int32": return TypeCode.Int32;
                case "System.UInt32": return TypeCode.UInt32;
                case "System.Int64": return TypeCode.Int64;
                case "System.UInt64": return TypeCode.UInt64;
                case "System.Single": return TypeCode.Single;
                case "System.Double": return TypeCode.Double;
                case "System.Decimal": return TypeCode.Decimal;
                case "System.DateTime": return TypeCode.DateTime;
                case "System.String": return TypeCode.String;
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
        protected SyncAssembly m_Assembly;


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
            if (type == null)
            {
                m_Assembly = new SyncAssembly();
            }
            else if (type != null)
            {
                m_FullName = type.FullName;
                m_IsArray = type.IsArray;
                m_MemberType = type.MemberType;
                m_Name = type.Name;
                m_Assembly = new SyncAssembly(type.Assembly);
            }
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
}
