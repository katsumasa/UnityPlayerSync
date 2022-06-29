//
// Programed by Katsumasa Kimura
//
using System.IO;
using System.Reflection;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// System.Assemblyを同期する為のクラス
    /// </summary>
    public class SyncAssembly : Sync
    {
        /// <summary>
        /// フルネーム
        /// </summary>
        protected string m_FullName = "";

        /// <summary>
        /// フルネーム
        /// </summary>
        public string FullName
        {
            get { return m_FullName; }
        }


        /// <summary>
        /// コンストラクター
        /// </summary>
        public SyncAssembly() { }


        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="assembly"></param>
        public SyncAssembly(Assembly assembly)
        {
            m_FullName = assembly.FullName;
        }


        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="binaryWriter">BinaryWriter</param>
        public override void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(m_FullName);
        }


        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="binaryReader">BinaryReader</param>
        public override void Deserialize(BinaryReader binaryReader)
        {         
            m_FullName = binaryReader.ReadString();
        }
    }
}