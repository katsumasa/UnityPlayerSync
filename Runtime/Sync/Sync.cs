//
// Programed by Katsumasa Kimura
//
using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// 同期用の抽象化クラス
    /// </summary>
    public abstract class Sync : ISerializer
    {
        public abstract void Serialize(BinaryWriter binaryWriter);
        public abstract void Deserialize(BinaryReader binaryReader);
    }
}