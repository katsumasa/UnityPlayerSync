//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncBounds : SyncValueType<Bounds>
    {
        public SyncBounds(object bounds) : base(bounds) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                var bounds = m_Values[i];
                binaryWriter.Write(bounds.center.x);
                binaryWriter.Write(bounds.center.y);
                binaryWriter.Write(bounds.center.z);
                binaryWriter.Write(bounds.extents.x);
                binaryWriter.Write(bounds.extents.y);
                binaryWriter.Write(bounds.extents.z);
                binaryWriter.Write(bounds.max.x);
                binaryWriter.Write(bounds.max.y);
                binaryWriter.Write(bounds.max.z);
                binaryWriter.Write(bounds.min.x);
                binaryWriter.Write(bounds.min.y);
                binaryWriter.Write(bounds.min.z);
                binaryWriter.Write(bounds.size.x);
                binaryWriter.Write(bounds.size.y);
                binaryWriter.Write(bounds.size.z);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].center   = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                m_Values[i].extents  = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                m_Values[i].max      = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                m_Values[i].min      = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                m_Values[i].size     = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());                
            }
        }
    }
}