using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace UTJ.UnityPlayerSyncEngine
{        
    public class SyncValueObject : Sync
    {

        public static SyncValueObject Allocater(System.Type objectType, System.Type comapreType,object obj = null)
        {
            // --- ValuType ---
            if (comapreType == typeof(bool) || comapreType == typeof(bool[]) || comapreType == typeof(List<bool>))
            {
                return new SyncValueBool(obj);
            }
            if (comapreType == typeof(byte) || comapreType == typeof(byte[]) || comapreType == typeof(List<byte>))
            {
                return new SyncValueByte(obj);
            }
            if (comapreType == typeof(char) || comapreType == typeof(char[]) || comapreType == typeof(List<char>))
            {
                return new SyncValueChar(obj);
            }
            if(comapreType == typeof(Enum) || comapreType == typeof(Enum[]) || comapreType == typeof(List<Enum>))
            {
                return new SyncValueEnum(obj, objectType);
            }
            if (comapreType == typeof(short) || comapreType == typeof(short[]) || comapreType == typeof(List<short>))
            {
                return new SyncValueInt16(obj);
            }
            if (comapreType == typeof(int) || comapreType == typeof(int[]) || comapreType == typeof(List<int>))
            {
                return new SyncValueInt(obj);
            }
            if (comapreType == typeof(long) || comapreType == typeof(long[]) || comapreType == typeof(List<int>))
            {
                return new SyncValueInt64(obj);
            }
            if (comapreType == typeof(sbyte) || comapreType == typeof(sbyte[]) || comapreType == typeof(List<sbyte>))
            {
                return new SyncValueSByte(obj);
            }
            if (comapreType == typeof(float) || comapreType == typeof(float[]) || comapreType == typeof(List<float>))
            {
                return new SyncValueSingle(obj);
            }
            if (comapreType == typeof(string) || comapreType == typeof(string[]) || comapreType == typeof(List<string>))
            {
                return new SyncValueString(obj);
            }
            if (comapreType == typeof(ushort) || comapreType == typeof(ushort[]) || comapreType == typeof(List<ushort>))
            {
                return new SyncValueUInt16(obj);
            }
            if (comapreType == typeof(uint) || comapreType == typeof(uint[]) || comapreType == typeof(List<uint>))
            {
                return new SyncValueUInt(obj);
            }
            if (comapreType == typeof(ulong) || comapreType == typeof(ulong[]) || comapreType == typeof(List<ulong>))
            {
                return new SyncValueUInt64(obj);
            }

            
            if (comapreType == typeof(Vector2) || comapreType == typeof(Vector2[]) || comapreType == typeof(List<Vector2>))
            {
                return new SyncVector2(obj);
            }
            if (comapreType == typeof(Vector3) || comapreType == typeof(Vector3[]) || comapreType == typeof(List<Vector3>))
            {
                return new SyncVector3(obj);
            }
            if (comapreType == typeof(Vector4) || comapreType == typeof(Vector4[]) || comapreType == typeof(List<Vector4>))
            {
                return new SyncVector4(obj);
            }
            if (comapreType == typeof(Quaternion) || comapreType == typeof(Quaternion[]) || comapreType == typeof(List<Quaternion>))
            {
                return new SyncQuaternion(obj);
            }
            if (comapreType == typeof(Color) || comapreType == typeof(Color[]) || comapreType == typeof(List<Color>))
            {
                return new SyncColor(obj);
            }
            if (comapreType == typeof(UnityEngine.Object) || comapreType == typeof(UnityEngine.Object[]) || comapreType == typeof(List<UnityEngine.Object>))
            {                
                return new SyncValuUnityEngineObject(obj, objectType);                
            }
            //throw new ArgumentException($"{type}is not avaiavle.");

            // 再帰・・・            
            if (comapreType.BaseType != null)
            {
                return Allocater(objectType, comapreType.BaseType, obj);
            }
            return null;
        }
        
        protected SyncType m_Type;        
        private object m_Value;
        

        public SyncType Type
        {
            get { return m_Type; }
        }                
        


        public SyncValueObject()
        {
            m_Type = new SyncType();            
        }


        public SyncValueObject(object o,Type type)
        {
            if (o == null)
            {
                m_Type = new SyncType(type);
            }
            else
            {
                var t = o.GetType();
                m_Type = new SyncType(t);
            }            
        }                


        public override void Serialize(BinaryWriter binaryWriter)
        {
            m_Type.Serialize(binaryWriter);            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {            
            m_Type.Deserialize(binaryReader);                        
        }

        public virtual object GetValue()
        {
            return m_Value;
        }    
        
    }
}
