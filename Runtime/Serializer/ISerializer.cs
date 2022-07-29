//
// Programed by Katsumasa.Kimura
//
using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// Serialize/Deserializeを行うクラスのインターフェース
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializeを行う
        /// </summary>
        /// <param name="binaryWriter">BinaryWriter</param>
        public void Serialize(BinaryWriter binaryWriter);


        /// <summary>
        /// Deserializeを行う
        /// </summary>
        /// <param name="binaryReader"></param>
        public void Deserialize(BinaryReader binaryReader);
    }
}
