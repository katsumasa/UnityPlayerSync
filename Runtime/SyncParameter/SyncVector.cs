using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncVector2 : SyncObject
    {
        protected SyncSingle m_X;
        protected SyncSingle m_Y;

        public float x
        {
            get { return m_X.value; }
            set { m_X.value = value; m_HasChanded = true; }
        }

        public float y
        {
            get { return m_Y.value; }
            set { m_Y.value = value; m_HasChanded = true; }
        }


        public SyncVector2 (Vector2 vector2) : base(vector2)
        {
            m_X = new SyncSingle(vector2.x);
            m_Y = new SyncSingle(vector2.y);
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_X.Serialize(binaryWriter);
            m_Y.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_X.Deserialize(binaryReader);
            m_Y.Deserialize(binaryReader);
        }

        public virtual void WriteBack(Vector2 vector2)
        {
            
            if (m_X.hasChanged)
            {
                vector2.x = m_X.value;
            }
            if (m_Y.hasChanged)
            {
                vector2.y = m_Y.value;
            }
        }
    }

    public class SyncVector3 : SyncVector2
    {
        protected SyncSingle m_Z;


        public float z
        {
            get { return m_Z.value; }
            set { m_Z.value = value; m_HasChanded = true; }
        }

        public SyncVector3(Vector3 vector3):base(vector3)
        {
            m_Z = new SyncSingle(vector3.z);
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_Z.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Z.Deserialize(binaryReader);
        }

        public virtual void WriteBack(Vector3 vector3)
        {
            base.WriteBack(vector3);
            vector3.z = m_Z.value;            
        }
    }


    public class SyncVector4 : SyncVector3
    {
        protected SyncSingle m_W;


        public SyncVector4(Vector4 vector4) : base(vector4)
        {
            m_W = new SyncSingle(vector4.w);
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_W.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_W.Deserialize(binaryReader);
        }

        public virtual void WriteBack(Vector4 vector4)
        {
            base.WriteBack(vector4);
            vector4.w = m_W.value;
        }

    }

}