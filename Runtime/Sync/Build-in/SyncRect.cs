//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncRect : SyncValueObject<Rect>
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
                var x = binaryReader.ReadSingle();
                var y = binaryReader.ReadSingle();
                var w = binaryReader.ReadSingle();
                var h = binaryReader.ReadSingle();                                
                m_Values[i] = new Rect(x, y, w, h);
                
                //Debug.Log($"Rect({x},{y},{w},{h})");
            }
        }
    }
}