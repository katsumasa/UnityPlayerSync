using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncMatrix4x4 : SyncValueType<Matrix4x4>
    {
        public SyncMatrix4x4(object matrix) : base(matrix) { }
        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    binaryWriter.Write(m_Values[i].GetColumn(j).x);
                    binaryWriter.Write(m_Values[i].GetColumn(j).y);
                    binaryWriter.Write(m_Values[i].GetColumn(j).z);
                    binaryWriter.Write(m_Values[i].GetColumn(j).w);
                }
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var v = new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    m_Values[i].SetColumn(j,v);
                }
            }
        }
    }
}
