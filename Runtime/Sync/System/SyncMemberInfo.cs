using System.IO;
using System.Collections.Generic;
using System.Reflection;



namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncMemberInfo : Sync,System.IDisposable
    {        
        protected MemberTypes m_MemberTypes;
        protected string m_Name;

        

        public MemberTypes MemberTypes
        {
            get { return m_MemberTypes; }
        }


        public string Name
        {
            get { return m_Name; }
        }


        public SyncMemberInfo(){ }


        public SyncMemberInfo(MemberInfo memberInfo)
        {
            m_MemberTypes = memberInfo.MemberType;
            m_Name = memberInfo.Name;
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {            
            binaryWriter.Write((int)m_MemberTypes);
            binaryWriter.Write(m_Name);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            m_MemberTypes = (MemberTypes)binaryReader.ReadInt32();
            m_Name = binaryReader.ReadString();
        }

        public virtual void Dispose()
        {
            m_Name = null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyncMemberInfo;
            if(other == null)
            {
                return false;
            }
            if (!base.Equals(obj))
            {
                return false;
            }
            if (!m_Name.Equals(other.m_Name)){
                return false;
            }
            if(!m_MemberTypes.Equals(other.m_MemberTypes))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return new { m_MemberTypes, m_Name}.GetHashCode() ^ base.GetHashCode();
        }
    }
}