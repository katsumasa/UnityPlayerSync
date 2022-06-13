using System.IO;
using System.Reflection;



namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncMemberInfo : SyncObject
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


        public SyncMemberInfo() : base(typeof(MemberInfo)) { }


        public SyncMemberInfo(MemberInfo memberInfo):base(memberInfo)
        {
            m_MemberTypes = memberInfo.MemberType;
            m_Name = memberInfo.Name;
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write((int)m_MemberTypes);
            binaryWriter.Write(m_Name);
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_MemberTypes = (MemberTypes)binaryReader.ReadInt32();
            m_Name = binaryReader.ReadString();
        }
    }
}