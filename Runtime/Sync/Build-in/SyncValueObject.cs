//
// Programed by Katsumasa Kimura
//
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{        
    public class SyncValueObject : Sync
    {
        // 
        /// <summary>
        /// Componentのパラメータ用のクラスをアロケーションする
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="comapreType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SyncValueObject Allocater(System.Type objectType, System.Type comapreType,object obj = null)
        {
            // 型を変換してシンプルな形へ・・・
            if (comapreType.IsGenericType)
            {
                var d = comapreType.GetGenericTypeDefinition();
                // List以外のGeneric型はサポートしない
                //if(d == typepf(Dicttionary<,>))
                if (d != typeof(List<>))
                {
                    Debug.Log($"GenericType{d} is not supported.");
                    return null;
                }
                // Generic型で管理している値を比較する
                comapreType = comapreType.GetGenericArguments()[0];
            }
            else if(comapreType.IsArray)
            {
                comapreType = comapreType.GetElementType();
            }            
            if(comapreType == typeof(MulticastDelegate))
            {
                Debug.Log($"MulticastDelegate({comapreType.Name}) is not supported.");
                return null;
            }
            if(
                (comapreType == typeof(UnityEngine.Events.UnityEvent)) ||
                (comapreType == typeof(UnityEngine.Events.UnityEvent<>)))                                
            {
                Debug.Log("UnityEvent is not supported.");
                return null;
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
            if(comapreType == typeof(System.Guid))
            {
                return new SyncValueGuid(obj);
            }


            // Unity Build-in

            if(comapreType == typeof(AnimationCurve))
            {
                return new SyncAnimationCurve(obj);
            }
            if(comapreType == typeof(AnimatorControllerParameter))
            {
                return new SyncAnimatorControllerParameter(obj);
            }
            if(comapreType == typeof(Bounds))
            {
                return new SyncBounds(obj);
            }            
            if(comapreType == typeof(LightBakingOutput))
            {
                return new SyncLightBakingOutput(obj);
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
            if (comapreType == typeof(Color32))
            {
                return new SyncColor32(obj);
            }
            if (comapreType == typeof(Rect))
            {
                return new SyncRect(obj);
            }
            if(comapreType == typeof(Matrix4x4))
            {
                return new SyncMatrix4x4(obj);
            }
            if(comapreType == typeof(LayerMask))
            {
                return new SyncLayerMask(obj);
            }

            // UnityEngine.Objectへの参照

            if(comapreType == typeof(Component))
            {
                return new SyncReferenceComponent(obj, objectType);
            }
            if (comapreType == typeof(UnityEngine.Object))
            {                
                return new SyncReferenceAsset(obj, objectType);                
            }
            

            // 再帰・・・            
            if (comapreType.BaseType != null)
            {
                return Allocater(objectType, comapreType.BaseType, obj);
            }


            Debug.LogWarning($"{objectType} is not avaiavle.");
            
            return null;
        }
        

        protected SyncType m_Type;
        
        /// <summary>
        /// このクラスが管理する値のType
        /// </summary>
        public SyncType Type
        {
            get { return m_Type; }
        }

        /// <summary>
        /// このクラスが管理する値
        /// </summary>
        private object m_Value;
        

             
        public SyncValueObject()
        {
            m_Type = new SyncType();            
        }


        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="o">同期する値</param>
        /// <param name="type">同期する値の型</param>
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


        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="binaryWriter"></param>
        public override void Serialize(BinaryWriter binaryWriter)
        {
            m_Type.Serialize(binaryWriter);            
        }


        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="binaryReader"></param>
        public override void Deserialize(BinaryReader binaryReader)
        {            
            m_Type.Deserialize(binaryReader);                        
        }


        /// <summary>
        /// 値を取得する
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue()
        {
            return m_Value;
        }    


        public virtual void SetValue(object o)
        {
            if(o != null)
            {
                var t1 = o.GetType();
                var t2 = m_Type.GetType();
                Debug.Assert(t1 == t2);

            }

            m_Value = o;
        }
        
    }
}
