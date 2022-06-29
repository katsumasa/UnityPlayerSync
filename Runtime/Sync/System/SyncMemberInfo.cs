using System.IO;
using System.Reflection;



namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncMemberInfo : Sync
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
    }
}