//
// Programed by Katsumasa Kimura
//
using System.IO;
using System.Reflection;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// System.Assembly�𓯊�����ׂ̃N���X
    /// </summary>
    public class SyncAssembly : Sync
    {
        /// <summary>
        /// �t���l�[��
        /// </summary>
        protected string m_FullName = "";

        /// <summary>
        /// �t���l�[��
        /// </summary>
        public string FullName
        {
            get { return m_FullName; }
        }


        /// <summary>
        /// �R���X�g���N�^�[
        /// </summary>
        public SyncAssembly() { }


        /// <summary>
        /// �R���X�g���N�^�[
        /// </summary>
        /// <param name="assembly"></param>
        public SyncAssembly(Assembly assembly)
        {
            m_FullName = assembly.FullName;
        }


        /// <summary>
        /// �V���A���C�Y
        /// </summary>
        /// <param name="binaryWriter">BinaryWriter</param>
        public override void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(m_FullName);
        }


        /// <summary>
        /// �f�V���A���C�Y
        /// </summary>
        /// <param name="binaryReader">BinaryReader</param>
        public override void Deserialize(BinaryReader binaryReader)
        {         
            m_FullName = binaryReader.ReadString();
        }
    }
}