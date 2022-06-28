using System.IO;
namespace UTJ.UnityPlayerSyncEngine
{
    public interface ISerializer
    {
        public void Serialize(BinaryWriter binaryWriter);
        public void Deserialize(BinaryReader binaryReader);
    }
}
