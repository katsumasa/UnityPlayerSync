//
// Programed by Katsumasa Kimura
//
using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// “¯Šú—p‚Ì’ŠÛ‰»ƒNƒ‰ƒX
    /// </summary>
    public abstract class Sync : ISerializer
    {
        public abstract void Serialize(BinaryWriter binaryWriter);
        public abstract void Deserialize(BinaryReader binaryReader);
    }
}