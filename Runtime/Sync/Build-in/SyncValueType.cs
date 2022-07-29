using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    
    public class SyncValueObject<T> : SyncValueObject
    {
        protected int m_Length;
        protected T[] m_Values;


        public override void Dispose()
        {
            base.Dispose();
            m_Length = 0;
            m_Values = null;
        }

        public SyncValueObject() : base() { }
        

        public SyncValueObject(object value,System.Type type):base(value,type)
        {
            m_Length = 0;
            m_Values = null;         
            if(value is UnityEngine.Object)
            {
                var obj = (UnityEngine.Object)value;                
                if(obj == null)
                {
                    return;
                }
            } 
            else if(value == null)
            {
                return;
            }            
            var t = value.GetType();
            
            if (t.IsArray)
            {                
                var array = (Array)value;
                m_Length = array.Length;
                m_Values = new T[m_Length];                
                for(var i = 0; i < m_Length; i++)
                {
                    m_Values[i] = (T)array.GetValue(i);
                }                
            }
            else if (t.IsGenericType)
            {
                // 現時点ではSyncValuObject.AllocateでList型以外のGenericを弾いているので
                // Generic型 = List<>である。
                if (t.GetGenericTypeDefinition() == typeof(List<>))
                {                    
                    try
                    {
                        var array = (Array)t.GetMethod("ToArray").Invoke(value, new object[] {});
                        m_Length = array.Length;
                        m_Values = new T[m_Length];
                        for (var i = 0; i < m_Length; i++)
                        {
                            m_Values[i] = (T)array.GetValue(i);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);                        
                    }
                 }
                else if(t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                }
                else
                {
                }
            }
            else
            {
                m_Length = 1;
                try
                {
                    m_Values = new T[1];
                    m_Values[0] = (T)value;
                }
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
            }            
            if(m_Values == null)
            {
                Debug.Log($"{type.Name}: m_Values is null.");
            }
        }


        public SyncValueObject(object value) : this(value, typeof(T)) { }
                        

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Length);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            try
            {
                base.Deserialize(binaryReader);
                m_Length = binaryReader.ReadInt32();
                if (m_Length == 0)
                {
                    m_Values = new T[0];
                }
                else if (m_Length > 0)
                {
                    m_Values = new T[m_Length];
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
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

        public override void SetValue(object value)
        {
            m_Length = 0;
            m_Values = null;
            if (value is UnityEngine.Object)
            {
                var obj = (UnityEngine.Object)value;
                if (obj == null)
                {
                    return;
                }
            }
            else if (value == null)
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
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                try
                {
                    var list = (List<T>)value;
                    m_Length = list.Count;
                    m_Values = new T[m_Length];
                    for (var i = 0; i < m_Length; i++)
                    {
                        m_Values[i] = list[i];
                    }
                }
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                m_Length = 1;
                m_Values = new T[1];
                m_Values[0] = (T)value;
            }
        }
    }


    public class SyncValueBool : SyncValueObject<bool>
    {
        public SyncValueBool() : base() { }

        public SyncValueBool(object arg) : base(arg) { }

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

    public class SyncValueByte : SyncValueObject<byte>
    {
        public SyncValueByte() : base() { }

        public SyncValueByte(object arg) : base(arg) { }

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


    public class SyncValueChar : SyncValueObject<char>
    {
        public SyncValueChar() : base() { }

        public SyncValueChar(object arg) : base(arg) { }

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


    public class SyncValueEnum : SyncValueObject<int>
    {
        public SyncValueEnum() : base() { }

        public SyncValueEnum(object arg,System.Type type) : base(arg, type) { }

        public SyncValueEnum(object arg) : base(arg,arg.GetType()) { }


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

        public override object GetValue()
        {
            var t = SyncType.GetType(m_Type);
            // 配列の場合は要素とする
            if (t.IsArray)
            {
                t = t.GetElementType();
            }
            var listType = typeof(List<>);
            var genericType = listType.MakeGenericType(t);
            var list = Activator.CreateInstance(genericType);
            var Add = list.GetType().GetMethod("Add");
            for (var i = 0; i < m_Values.Length; i++)
            {
                Add.Invoke(list, new object[] { Enum.ToObject(t, m_Values[i]) });
            }
            var ToAttay = list.GetType().GetMethod("ToArray");

            if (m_Type.IsArray)
            {
#if false
                var arrayList = new ArrayList();
                for (var i = 0; i < m_Values.Length; i++)
                {
                    arrayList.Add(Enum.ToObject(t, m_Values[i]));
                }               
                return arrayList.ToArray();                                                               
#else
                return ToAttay.Invoke(list,new object[] { });        
#endif  


            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                return list;   
            }
            else
            {
                return Enum.ToObject(t, m_Values[0]);
            }
        }
    }

    public class SyncValueGuid : SyncValueObject<System.Guid>
    {
        public SyncValueGuid() : base() { }
        public SyncValueGuid(object arg) : base(arg) { }
        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for(var i = 0; i < m_Length; i++)
            {
                var array = m_Values[i].ToByteArray();
                binaryWriter.Write(array.Length);
                binaryWriter.Write(array);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                var len = binaryReader.ReadInt32();
                var array = binaryReader.ReadBytes(len);
                m_Values[i] = new System.Guid(array);
            }
        }
    }



    public class SyncValueInt16 : SyncValueObject<short>
    {
        public SyncValueInt16() : base() { }
        public SyncValueInt16(object arg) : base(arg) { }

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


    public class SyncValueInt : SyncValueObject<int>
    {
        public SyncValueInt() : base() { }
        public SyncValueInt(object arg) : base(arg) { }


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

    


    public class SyncValueInt64 : SyncValueObject<long>
    {
        public SyncValueInt64() : base() { }

        public SyncValueInt64(object arg) : base(arg) { }

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

    public class SyncValueSByte : SyncValueObject<sbyte>
    {
        public SyncValueSByte() : base() { }

        public SyncValueSByte(object arg) : base(arg) { }

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

    public class SyncValueSingle : SyncValueObject<float>
    {
        public SyncValueSingle() : base() { }
        public SyncValueSingle(object arg) : base(arg) { }

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

    public class SyncValueString : SyncValueObject<string>
    {
        public SyncValueString() : base() { }
        public SyncValueString(object arg) : base(arg) { }

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

    public class SyncValueUInt16 : SyncValueObject<ushort>
    {
        public SyncValueUInt16() : base() { }
        public SyncValueUInt16(object arg) : base(arg) { }

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


    public class SyncValueUInt : SyncValueObject<uint>
    {
        public SyncValueUInt() : base() { }
        public SyncValueUInt(object arg) : base(arg) { }


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


    public class SyncValueUInt64 : SyncValueObject<ulong>
    {
        public SyncValueUInt64() : base() { }
        public SyncValueUInt64(object arg) : base(arg) { }

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


    public class SyncValueComponent : SyncValueObject<Component>
    {
        private int[] m_TransformInstanceIDs;


        public SyncValueComponent() : base() { }

        public SyncValueComponent(object value):base(value)
        {
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);            
            for(var i = 0; i < m_Values.Length; i++)
            {                
                binaryWriter.Write(m_Values[i].transform.GetInstanceID());
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var len = binaryReader.ReadInt32();
            m_TransformInstanceIDs = new int[len];
            for(var i = 0; i < len; i++)
            {
                m_TransformInstanceIDs[i] = binaryReader.ReadInt32();
            }
        }


    }

    

}
