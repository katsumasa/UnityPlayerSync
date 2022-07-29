//
// Programed by Katsumasa.Kimura
//
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncLayerMask : SyncValueObject<LayerMask>
    {
        public SyncLayerMask(object obj) : base(obj) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (int i = 0; i < m_Length; i++) {
                binaryWriter.Write(m_Values[i].value);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].value = binaryReader.ReadInt32();
            }
        }
    }
}