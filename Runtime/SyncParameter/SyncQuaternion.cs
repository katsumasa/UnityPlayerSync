using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncQuaternion : SyncValueType<Quaternion>
    {
        public SyncQuaternion(object quaternion) : base(quaternion) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].x);
                binaryWriter.Write(m_Values[i].y);
                binaryWriter.Write(m_Values[i].z);
                binaryWriter.Write(m_Values[i].w);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for(var i = 0; i < m_Length; i++)
            {
                m_Values[i].x = binaryReader.ReadSingle();
                m_Values[i].y = binaryReader.ReadSingle();
                m_Values[i].z = binaryReader.ReadSingle();
                m_Values[i].w = binaryReader.ReadSingle();
            }
        }
    }
}