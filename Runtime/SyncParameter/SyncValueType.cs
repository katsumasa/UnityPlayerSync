using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncValueType : SyncObject
    {
        static readonly System.Type[] m_AvailableTypes =
        {
            typeof(bool),
            typeof(bool[]),
            typeof(byte),
            typeof(byte[]),
            typeof(char),
            typeof(char[]),
            typeof(short),
            typeof(short[]),
            typeof(int),
            typeof(int[]),
            typeof(List<int>),
            typeof(long),
            typeof(long[]),
            typeof(sbyte),
            typeof(sbyte[]),
            typeof(float),
            typeof(float[]),
            typeof(string),
            typeof(string[]),
            typeof(ushort),
            typeof(ushort[]),
            typeof(uint),
            typeof(uint[]),
            typeof(ulong),
            typeof(ulong[]),
            //
            typeof(Vector2),
            typeof(Vector2[]),
            typeof(Vector3),
            typeof(Vector3[]),
            typeof(Vector4),
            typeof(Vector4[]),
            typeof(Quaternion),
            typeof(Quaternion[]),
        };


        public static bool IsAvailableType(System.Type type)
        {
            for (var i = 0; i < m_AvailableTypes.Length; i++)
            {
                if (m_AvailableTypes[i] == type)
                {
                    return true;
                }
            }
            return false;
        }
               


        

        public static SyncValueType Allocater(System.Type type,object obj = null)
        {

            if (type == typeof(bool) || type == typeof(bool[]) || type == typeof(List <bool>))
            {                                
                return new SyncBool(obj);
            }            
            if(type == typeof(byte) || type == typeof(byte[]) || type == typeof(List<byte>))
            {                
                return new SyncByte(obj);
            }            
            if(type == typeof(char) || type == typeof(char[]) || type == typeof(List<char>)){
                
                return new SyncChar(obj);
            }
            if(type == typeof(short) || type == typeof(short[]) || type == typeof(List<short>))
            {

                return new SyncInt16(obj);
            }
            if(type == typeof(int) || type == typeof(int[]) || type == typeof(List<int>))
            {
                return new SyncInt(obj);
            }            
            if(type == typeof(long) || type == typeof(long[]) || type == typeof(List<int>))
            {
                return new SyncInt64(obj);
            }
            if(type == typeof(sbyte) || type == typeof(sbyte[]) || type == typeof(List<sbyte>))
            {
                return new SyncSByte(obj);
            }
            if(type == typeof(float) || type == typeof(float[]) || type == typeof(List<float>))
            {
                return new SyncSingle(obj);
            }
            if(type == typeof(string) || type == typeof(string[]) || type == typeof(List<string>))
            {
                return new SyncString(obj);
            }
            if(type == typeof(ushort) || type == typeof(ushort[]) || type == typeof(List<ushort>))
            {
                return new SyncUInt16(obj);
            }
            if(type == typeof(uint) || type == typeof(uint[]) || type == typeof(List<uint>))
            {
                return new SyncUInt(obj);
            }
            if(type == typeof(ulong) || type == typeof(ulong[]) || type == typeof(List<ulong>))
            {
                return new SyncUInt64(obj);
            }
            //
            if(type == typeof(Vector2) || type == typeof(Vector2[]) || type == typeof(List<Vector2>))
            {
                return new SyncVector2((Vector2)obj);
            }
            if(type == typeof(Vector3) || type == typeof(Vector3[]) || type == typeof(List<Vector3>))
            {
                return new SyncVector3((Vector3)obj);
            }
            if(type == typeof(Vector4) || type == typeof(Vector4[]) || type == typeof(List<Vector4>))
            {
                return new SyncVector4((Vector4)obj);
            }

            throw new ArgumentException($"{type}is not avaiavle.");

        }

        

        public SyncValueType() : base() { }



        public SyncValueType(object obj) : base(obj) { }

    }



    public class SyncValueType<T> : SyncValueType
    {
        protected int m_Length;
        protected T[] m_Values;
        


        public virtual T value
        {
            get { return m_Values[0]; }
            set { m_Values[0] = value; m_HasChanded = true; }
        }

        public virtual T[] values
        {
            get { return m_Values; }
            set
            { 
                m_Values = value; 
                m_HasChanded = true;
                if (m_Values == null)
                {
                    m_Length = -1;
                }
                else
                {
                    m_Length = m_Values.Length;
                }
            }
        }
        
        public SyncValueType() : base()
        {
        }


        public SyncValueType(object value):base(value)
        {
            if(value == null)
            {
                return;
            }

            var t = value.GetType();
            if (t.IsArray)
            {
                var array = (T[])value;
                m_Length = array.Length;
                m_Values = new T[m_Length];
                Array.Copy(array, m_Values, array.Length);
            }
            else if(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (List<T>)value;
                m_Length = list.Count;
                m_Values = new T[m_Length];
                for(var i = 0; i < m_Length; i++)
                {
                    m_Values[i] = list[i];
                }
            }
            else
            {
                m_Length = 1;
                m_Values = new T[1];
                m_Values[0] = (T)value;
            }

            
        }
                

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Length);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Length = binaryReader.ReadInt32();
            if (m_Length >= 0)
            {
                m_Values = new T[m_Length];
            }            
        }


        public override object GetValue()
        {
            var t = SyncType.GetType(m_Type);

            if (m_Type.IsArray)
            {
                return m_Values;
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = new List<T>(m_Values);
                return list;
            }
            else
            {
                return m_Values[0];
            }
        }
    }


    public class SyncBool : SyncValueType<bool>
    {
        public SyncBool() : base() { }

        public SyncBool(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);                       
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadBoolean();
            }
        }
    }

    public class SyncByte : SyncValueType<byte>
    {
        public SyncByte() : base() { }

        public SyncByte(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadByte();
            }
        }
    }


    public class SyncChar : SyncValueType<char>
    {
        public SyncChar() : base() { }

        public SyncChar(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadChar();
            }
        }
    }

    public class SyncInt16 : SyncValueType<short>
    {
        public SyncInt16() : base() { }
        public SyncInt16(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadInt16();
            }
        }
    }


    public class SyncInt : SyncValueType<int>
    {
        public SyncInt() : base() { }
        public SyncInt(object arg) : base(arg) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadInt32();
            }
        }
    }

    


    public class SyncInt64 : SyncValueType<long>
    {
        public SyncInt64() : base() { }

        public SyncInt64(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadInt64();
            }
        }
    }

    public class SyncSByte : SyncValueType<sbyte>
    {
        public SyncSByte() : base() { }

        public SyncSByte(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadSByte();
            }
        }        
    }

    public class SyncSingle : SyncValueType<float>
    {
        public SyncSingle() : base() { }
        public SyncSingle(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadSingle();
            }
        }
    }

    public class SyncString : SyncValueType<string>
    {
        public SyncString() : base() { }
        public SyncString(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadString();
            }
        }
    }

    public class SyncUInt16 : SyncValueType<ushort>
    {
        public SyncUInt16() : base() { }
        public SyncUInt16(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadUInt16();
            }
        }
    }


    public class SyncUInt : SyncValueType<uint>
    {
        public SyncUInt() : base() { }
        public SyncUInt(object arg) : base(arg) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadUInt32();
            }
        }
    }


    public class SyncUInt64 : SyncValueType<ulong>
    {
        public SyncUInt64() : base() { }
        public SyncUInt64(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write(m_Values[i]);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i] = binaryReader.ReadUInt64();
            }
        }
    }

}
