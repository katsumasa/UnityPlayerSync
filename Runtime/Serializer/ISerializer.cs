//
// Programed by Katsumasa.Kimura
//
using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// Serialize/Deserialize���s���N���X�̃C���^�[�t�F�[�X
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialize���s��
        /// </summary>
        /// <param name="binaryWriter">BinaryWriter</param>
        public void Serialize(BinaryWriter binaryWriter);


        /// <summary>
        /// Deserialize���s��
        /// </summary>
        /// <param name="binaryReader"></param>
        public void Deserialize(BinaryReader binaryReader);
    }
}
