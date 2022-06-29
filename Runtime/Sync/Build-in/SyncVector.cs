using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncVector2 : SyncValueType<Vector2>
    {
        public SyncVector2(object vector2) : base(vector2) { }
        

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].x);
                binaryWriter.Write(m_Values[i].y);
            }            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for(var i = 0; i < m_Length; i++)
            {
                m_Values[i].x =  binaryReader.ReadSingle();
                m_Values[i].y = binaryReader.ReadSingle();
            }
        }        
    }

    public class SyncVector3 : SyncValueType<Vector3>
    {
        public SyncVector3(object vector3):base(vector3)
        {     
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for(var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i].x);
                binaryWriter.Write(m_Values[i].y);
                binaryWriter.Write(m_Values[i].z);
            }            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].x = binaryReader.ReadSingle();
                m_Values[i].y = binaryReader.ReadSingle();
                m_Values[i].z = binaryReader.ReadSingle();
            }
        }        
    }


    public class SyncVector4 : SyncValueType<Vector4>
    {        
        public SyncVector4(object vector4) : base(vector4) { }
        

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
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].x = binaryReader.ReadSingle();
                m_Values[i].y = binaryReader.ReadSingle();
                m_Values[i].z = binaryReader.ReadSingle();
                m_Values[i].w = binaryReader.ReadSingle();
            }
        }        
    }

}