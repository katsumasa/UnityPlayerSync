//
// Programed by Katsumasa Kimura
//
using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// �����p�̒��ۉ��N���X
    /// </summary>
    public abstract class Sync : ISerializer
    {
        public abstract void Serialize(BinaryWriter binaryWriter);
        public abstract void Deserialize(BinaryReader binaryReader);
    }
}