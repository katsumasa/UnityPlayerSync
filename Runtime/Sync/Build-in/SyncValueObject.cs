//
// Programed by Katsumasa Kimura
//
using System;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{        
    public class SyncValueObject : Sync
    {
        // 
        /// <summary>
        /// Component�̃p�����[�^�p�̃N���X���A���P�[�V��������
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="comapreType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SyncValueObject Allocater(System.Type objectType, System.Type comapreType,object obj = null)
        {
            // �^��ϊ����ăV���v���Ȍ`�ցE�E�E
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
            if(comapreType == typeof(System.Guid))
            {
                return new SyncValueGuid(obj);
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
            if(comapreType == typeof(Rect))
            {
                return new SyncRect(obj);
            }
            if(comapreType == typeof(Matrix4x4))
            {
                return new SyncMatrix4x4(obj);
            }

            // UnityEngine.Object�ւ̎Q��

            if(comapreType == typeof(Component))
            {
                return new SyncReferenceComponent(obj, objectType);
            }
            if (comapreType == typeof(UnityEngine.Object))
            {                
                return new SyncReferenceAsset(obj, objectType);                
            }
            

            // �ċA�E�E�E            
            if (comapreType.BaseType != null)
            {
                return Allocater(objectType, comapreType.BaseType, obj);
            }


            Debug.LogWarning($"{objectType} is not avaiavle.");
            
            return null;
        }
        

        protected SyncType m_Type;
        
        /// <summary>
        /// ���̃N���X���Ǘ�����l��Type
        /// </summary>
        public SyncType Type
        {
            get { return m_Type; }
        }

        /// <summary>
        /// ���̃N���X���Ǘ�����l
        /// </summary>
        private object m_Value;
        

             
        public SyncValueObject()
        {
            m_Type = new SyncType();            
        }


        /// <summary>
        /// �R���X�g���N�^�[
        /// </summary>
        /// <param name="o">��������l</param>
        /// <param name="type">��������l�̌^</param>
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
        /// �V���A���C�Y
        /// </summary>
        /// <param name="binaryWriter"></param>
        public override void Serialize(BinaryWriter binaryWriter)
        {
            m_Type.Serialize(binaryWriter);            
        }


        /// <summary>
        /// �f�V���A���C�Y
        /// </summary>
        /// <param name="binaryReader"></param>
        public override void Deserialize(BinaryReader binaryReader)
        {            
            m_Type.Deserialize(binaryReader);                        
        }


        /// <summary>
        /// �l���擾����
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
