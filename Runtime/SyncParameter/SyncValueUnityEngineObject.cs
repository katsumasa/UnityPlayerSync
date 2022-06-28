using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTJ.UnityPlayerSyncEngine
{    
    public class SyncValueUnityEngineObject : SyncValueType<UnityEngine.Object>
    {
        protected int[] m_InstanceIDs;
        protected string[] m_Names;

        public SyncValueUnityEngineObject() : base() { }
        

        public SyncValueUnityEngineObject(object value,System.Type type) : base(value, type) { }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            if (m_Values != null)
            {
                for (var i = 0; i < m_Values.Length; i++)
                {
                    try
                    {
                        binaryWriter.Write(m_Values[i].GetInstanceID());
                        binaryWriter.Write(m_Values[i].name);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            if (m_Values != null)
            {
                m_InstanceIDs = new int[m_Values.Length];
                m_Names = new string[m_Values.Length];
                for (var i = 0; i < m_Values.Length; i++)
                {
                    m_InstanceIDs[i] = binaryReader.ReadInt32();
                    m_Names[i] = binaryReader.ReadString();
                }
            }
        }


        public override object GetValue()
        {
            var type = SyncType.GetType(m_Type);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }


            var listType = typeof(List<>);
            var genericType = listType.MakeGenericType(type);
            var list = Activator.CreateInstance(genericType);
            var Add = list.GetType().GetMethod("Add");
            for (var i = 0; i < m_Values.Length; i++)
            {
                Add.Invoke(list, new object[] { m_Values[i] });
            }
            var ToAttay = list.GetType().GetMethod("ToArray");
            var array = ToAttay.Invoke(list, new object[] { });


            type = SyncType.GetType(m_Type);
            if (type.IsArray)
            {
                return (object[])array;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return list;
            }
            else
            {
                var items = (object[])array;
                return items[0];
            }
        }
    }



}