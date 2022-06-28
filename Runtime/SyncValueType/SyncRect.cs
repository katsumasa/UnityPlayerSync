using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncRect : SyncValueType<Rect>
    {
        public SyncRect(object rect): base(rect) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].x);
                binaryWriter.Write(m_Values[i].y);
                binaryWriter.Write(m_Values[i].width);
                binaryWriter.Write(m_Values[i].height);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = new Rect(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());                
            }
        }
    }
}