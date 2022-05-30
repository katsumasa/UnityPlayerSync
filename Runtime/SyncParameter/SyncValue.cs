using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace UTJ.UnityPlayerSyncEngine
{
    
    

    public class SyncValue<T> : SyncObject
    {        
        protected T m_Value;
        

        public virtual T value
        {
            get { return m_Value; }
            set { m_Value = value; m_HasChanded = true; }
        }

        
        public SyncValue(T value):base(value)
        {                        
            m_Value = value;
        }        
    }

    public class SyncBool : SyncValue<bool>
    {
        public SyncBool(bool arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadBoolean();
        }
    }

    public class SyncByte : SyncValue<byte>
    {
        public SyncByte(byte arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            binaryReader.ReadByte();
        }
    }


    public class SyncChar : SyncValue<char>
    {
        public SyncChar(char arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadChar();
        }
    }

    public class SyncInt16 : SyncValue<short>
    {
        public SyncInt16(short arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadInt16();
        }
    }


    public class SyncInt : SyncValue<int>
    {
        public SyncInt(int arg) : base(arg) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadInt32();
        }
    }


    public class SyncInt64 : SyncValue<long>
    {
        public SyncInt64(long arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadInt64();
        }
    }

    public class SyncSByte : SyncValue<sbyte>
    {
        public SyncSByte(sbyte arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadSByte();
        }        
    }

    public class SyncSingle : SyncValue<float>
    {
        public SyncSingle(float arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadSingle();
        }
    }

    public class SyncString : SyncValue<string>
    {
        public SyncString(string arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadString();
        }
    }

    public class SyncUInt16 : SyncValue<ushort>
    {
        public SyncUInt16(ushort arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadUInt16();
        }
    }


    public class SyncUInt : SyncValue<uint>
    {
        public SyncUInt(uint arg) : base(arg) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadUInt32();
        }
    }


    public class SyncUInt64 : SyncValue<ulong>
    {
        public SyncUInt64(ulong arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Value);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Value = binaryReader.ReadUInt64();
        }
    }

}
