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
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(float),
            typeof(string),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            //
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),            
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
               


        public static SyncValueType Allocater(System.Type type)
        {
            return Allocater(type.Name);
        }

        public static SyncValueType Allocater(System.Type type,object obj)
        {
            return Allocater(type.Name, obj);
        }

        public static SyncValueType Allocater(string name)
        {
            switch (name)
            {
                case "Boolean": return new SyncBool();
                case "Byte": return new SyncByte();
                case "Char": return new SyncChar();
                case "Int16": return new SyncInt16();
                case "Int32": return new SyncInt();
                case "Int64": return new SyncInt64();
                case "SByte": return new SyncSByte();
                case "Single": return new SyncSingle();
                case "String": return new SyncString();
                case "UInt16": return new SyncUInt16();
                case "UInt32": return new SyncUInt();
                case "UInt64": return new SyncUInt64();                
                case "Vector2": return new SyncVector2(Vector2.zero);
                case "Vector3": return new SyncVector3(Vector3.zero);
                case "Vector4": return new SyncVector4(Vector4.zero);                
            }
            throw new ArgumentException($"{name}is not avaiavle.");            
        }

        public static SyncValueType Allocater(string name, object obj)
        {
            switch (name)
            {
                case "Boolean": return new SyncBool(obj);
                case "Byte": return new SyncByte(obj);
                case "Char": return new SyncChar(obj);
                case "Int16": return new SyncInt16(obj);
                case "Int32": return new SyncInt(obj);
                case "Int64": return new SyncInt64(obj);
                case "SByte": return new SyncSByte(obj);
                case "Single": return new SyncSingle(obj);
                case "String": return new SyncString(obj);
                case "UInt16": return new SyncUInt16(obj);
                case "UInt32": return new SyncUInt(obj);
                case "UInt64": return new SyncUInt64(obj);                
                case "Vector2": return new SyncVector2((Vector2)obj);
                case "Vector3": return new SyncVector3((Vector3)obj);
                case "Vector4": return new SyncVector4((Vector4)obj);                
            }
            throw new ArgumentException($"{name}is not avaiavle.");
        }

        public object GetValue(string name)
        {
            switch (name)
            {
                case "Boolean": return ((SyncBool)this).value;
                case "Byte":    return ((SyncByte)this).value;
                case "Char":    return ((SyncChar)this).value;
                case "Int16":   return ((SyncInt16)this).value;
                case "Int32":   return ((SyncInt)this).value;
                case "Int64":   return ((SyncInt64)this).value;
                case "SByte":   return ((SyncSByte)this).value;
                case "Single":  return ((SyncSingle)this).value;
                case "String":  return ((SyncString)this).value;
                case "UInt16":  return ((SyncUInt16)this).value;
                case "UInt32":  return ((SyncUInt)this).value;
                case "UInt64":  return ((SyncUInt64)this).value;
                case "Vector2": return ((SyncVector2)this).value;
                case "Vector3": return ((SyncVector3)this).value;
                case "Vector4": return ((SyncVector4)this).value;
            }
            throw new ArgumentException($"{name}is not avaiavle.");
        }

        public object GetValues(string name)
        {
            switch (name)
            {
                case "Boolean": return ((SyncBool)this).values;
                case "Byte":    return ((SyncByte)this).values;
                case "Char":    return ((SyncChar)this).values;
                case "Int16":   return ((SyncInt16)this).values;
                case "Int32":   return ((SyncInt)this).values;
                case "Int64":   return ((SyncInt64)this).values;
                case "SByte":   return ((SyncSByte)this).values;
                case "Single":  return ((SyncSingle)this).values;
                case "String":  return ((SyncString)this).values;
                case "UInt16":  return ((SyncUInt16)this).values;
                case "UInt32":  return ((SyncUInt)this).values;
                case "UInt64":  return ((SyncUInt64)this).values;
                case "Vector2": return ((SyncVector2)this).values;
                case "Vector3": return ((SyncVector3)this).values;
                case "Vector4": return ((SyncVector4)this).values;
            }
            throw new ArgumentException($"{name}is not avaiavle.");
        }


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
        
        public SyncValueType() : base(typeof(T))
        {
            m_Length = 1;
            m_Values = new T[1];
        }

        public SyncValueType(Type type) : base(type)
        {            
            m_Length = 1;
            m_Values = new T[1];
        }
        
        public SyncValueType(object value):base(value)
        {
            m_Length = 1;
            m_Values = new T[1];
            m_Values[0] = (T)value;
        }
        
        public SyncValueType(object[] values):base(values)
        {
            if (values != null)
            {
                m_Length = values.Length;
                m_Values = new T[values.Length];
                Array.Copy(values, m_Values, values.Length);
            }
            else
            {
                m_Length = -1;
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

        public virtual void WriteBack(ref T v)
        {
            if (m_HasChanded)
            {
                v = m_Values[0];
            }
        }

        public virtual void WriteBack(ref T[] v)
        {
            if (m_HasChanded)
            {
                v = m_Values;
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
