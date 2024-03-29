﻿//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncColor : SyncValueObject<Color>
    {
        public SyncColor(object color) : base(color) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for(var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].r);
                binaryWriter.Write(m_Values[i].g);
                binaryWriter.Write(m_Values[i].b);
                binaryWriter.Write(m_Values[i].a);
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for(var i = 0; i < m_Length; i++)
            {
                m_Values[i].r = binaryReader.ReadSingle();
                m_Values[i].g = binaryReader.ReadSingle();
                m_Values[i].b = binaryReader.ReadSingle();
                m_Values[i].a = binaryReader.ReadSingle();
            }
        }
    }


    public class SyncColor32 : SyncValueObject<Color32>
    {
        public SyncColor32(object color) : base(color) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].r);
                binaryWriter.Write(m_Values[i].g);
                binaryWriter.Write(m_Values[i].b);
                binaryWriter.Write(m_Values[i].a);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].r = binaryReader.ReadByte();
                m_Values[i].g = binaryReader.ReadByte();
                m_Values[i].b = binaryReader.ReadByte();
                m_Values[i].a = binaryReader.ReadByte();
            }
        }
    }
}
