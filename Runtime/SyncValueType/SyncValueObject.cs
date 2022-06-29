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
            if (comapreType.IsGenericType)
            {
                comapreType = comapreType.GetGenericArguments()[0];
            }
            else if(comapreType.IsArray)
            {
                comapreType = comapreType.GetElementType();
            }


            // --- ValuType ---
            if (comapreType == typeof(bool))
            {
                return new SyncValueBool(obj);
            }
            if (comapreType == typeof(byte))
            {
                return new SyncValueByte(obj);
            }
            if (comapreType == typeof(char))
            {
                return new SyncValueChar(obj);
            }
            if(comapreType == typeof(Enum))
            {
                return new SyncValueEnum(obj, objectType);
            }
            if (comapreType == typeof(short))
            {
                return new SyncValueInt16(obj);
            }
            if (comapreType == typeof(int))
            {
                return new SyncValueInt(obj);
            }
            if (comapreType == typeof(long))
            {
                return new SyncValueInt64(obj);
            }
            if (comapreType == typeof(sbyte))
            {
                return new SyncValueSByte(obj);
            }
            if (comapreType == typeof(float))
            {
                return new SyncValueSingle(obj);
            }
            if (comapreType == typeof(string))
            {
                return new SyncValueString(obj);
            }
            if (comapreType == typeof(ushort))
            {
                return new SyncValueUInt16(obj);
            }
            if (comapreType == typeof(uint))
            {
                return new SyncValueUInt(obj);
            }
            if (comapreType == typeof(ulong))
            {
                return new SyncValueUInt64(obj);
            }

            // Unity Build-in

            if(comapreType == typeof(AnimationCurve))
            {
                return new SyncAnimationCurve(obj);
            }
            if(comapreType == typeof(Bounds))
            {
                return new SyncBounds(obj);
            }            
            if (comapreType == typeof(Vector2))
            {
                return new SyncVector2(obj);
            }
            if (comapreType == typeof(Vector3))
            {
                return new SyncVector3(obj);
            }
            if (comapreType == typeof(Vector4))
            {
                return new SyncVector4(obj);
            }
            if (comapreType == typeof(Quaternion))
            {
                return new SyncQuaternion(obj);
            }
            if (comapreType == typeof(Color))
            {
                return new SyncColor(obj);
            }
            if(comapreType == typeof(Rect))
            {
                return new SyncRect(obj);
            }
            if(comapreType == typeof(Matrix4x4))
            {
                return new SyncMatrix4x4(obj);
            }

            if(comapreType == typeof(Component))
            {
                return new SyncReferenceComponent(obj, objectType);
            }
            if (comapreType == typeof(UnityEngine.Object))
            {                
                return new SyncReferenceAsset(obj, objectType);                
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


        public virtual void SetValue(object o)
        {
            m_Value = o;
        }
        
    }
}
