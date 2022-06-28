using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncComponent : SyncUnityEngineObject
    {                                
        protected SyncValueObject[] m_Properties;
        protected SyncPropertyInfo[] m_PropertyInfos;
        protected SyncValueObject[] m_Fields;
        protected SyncFieldInfo[] m_FieldInfos;
        
        public Component GetComponent()
        {
            return (Component)m_object;
        }
        
        public SyncComponent(object obj,bool isPlayer = true) : base(obj)
        {
            if(isPlayer == false)
            {
                return;
            }

            var component = (Component)m_object;
            var type = component.GetType();

            

            // プロパティの取得
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);                
            var list = new List<SyncValueObject>();
            var propList = new List<SyncPropertyInfo>();
            foreach(var prop in props)
            {
                
                if(!prop.CanRead)
                {
                    continue;
                }
                // UnityではObsoleteになったgetterがNotSupportedExceptionをthrowしている為、キャッチしてスルーする必要があるが、
                // TestRunnerがSystem.ExceptionでキャッチしているのでこちらもSystem.Exceptionでキャッチせざるを得ない
                object o = null;
                try
                {
                    if (!IsSkipGetValue(prop))
                    {
                        o = prop.GetValue(component);
                    }
                }
                //catch(System.NotSupportedException)
                catch(System.Exception e)
                {                            
                    continue;
                }                    
                var syncValueType = SyncValueObject.Allocater(prop.PropertyType, prop.PropertyType,o);
                if(syncValueType == null)
                {
                    continue;
                }
                list.Add(syncValueType);
                propList.Add(new SyncPropertyInfo(prop));                    
            }
            m_Properties = list.ToArray();
            m_PropertyInfos = propList.ToArray();


            // フィールド（変数)の取得
            list.Clear();
            var fiList = new List<SyncFieldInfo>();
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var fi in fis)
            {                                
                if(fi.IsInitOnly)
                {
                    continue;
                }
                if (fi.IsLiteral)
                {
                    continue;
                }
                if (fi.IsStatic)
                {
                    continue;
                }
                var at = Attribute.GetCustomAttribute(fi, typeof(SerializeField));
                var IsSerializeField = at != null ? true : false;
                if (fi.IsPrivate && !IsSerializeField)
                {
                    continue;
                }                
                object o = null;
                try
                {                    
                     o = fi.GetValue(component);                    
                }
                catch(System.Exception)
                {
                    continue;
                }
                var syncValueType = SyncValueObject.Allocater(fi.FieldType, fi.FieldType,o);
                if(syncValueType == null)
                {
                    continue;
                }
                list.Add(syncValueType);
                fiList.Add(new SyncFieldInfo(fi));
            }
            m_Fields = list.ToArray();
            m_FieldInfos = fiList.ToArray();            
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            
            binaryWriter.Write(m_Properties.Length);
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_PropertyInfos[i].Serialize(binaryWriter);
            }
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_Properties[i].Serialize(binaryWriter);
            }

            binaryWriter.Write(m_Fields.Length);
            for(var i = 0; i < m_FieldInfos.Length; i++)
            {
                m_FieldInfos[i].Serialize(binaryWriter);
            }
            for(var i = 0; i < m_Fields.Length; i++)
            {
                m_Fields[i].Serialize(binaryWriter);
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var len = binaryReader.ReadInt32();
            m_PropertyInfos = new SyncPropertyInfo[len];
            for (var i = 0; i < m_PropertyInfos.Length; i++)
            {
                m_PropertyInfos[i] = new SyncPropertyInfo();
                m_PropertyInfos[i].Deserialize(binaryReader);
            }
            
            m_Properties = new SyncValueObject[len];
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_Properties[i] = SyncValueObject.Allocater(SyncType.GetType(m_PropertyInfos[i].PropertyType), SyncType.GetType(m_PropertyInfos[i].PropertyType));
                m_Properties[i].Deserialize(binaryReader);
            }

            len = binaryReader.ReadInt32();
            m_FieldInfos = new SyncFieldInfo[len];            
            for(var i = 0; i < len; i++)
            {
                m_FieldInfos[i] = new SyncFieldInfo();
                m_FieldInfos[i].Deserialize(binaryReader);
            }

            m_Fields = new SyncValueObject[len];
            for(var i = 0; i < len; i++)
            {
                m_Fields[i] = SyncValueObject.Allocater(SyncType.GetType(m_FieldInfos[i].FieldType), SyncType.GetType(m_FieldInfos[i].FieldType));
                m_Fields[i].Deserialize(binaryReader);
            }
        }


        public void WriteBack()        
        {
            var component = (Component)m_object;

            var type = component.GetType();                
            for(var i = 0; i < m_Properties.Length; i++)
            {            
                var prop = type.GetProperty(m_PropertyInfos[i].Name);
                if (!prop.CanWrite)
                {
                    continue;
                }                                    
                var o = m_Properties[i].GetValue();
                if (o != null)
                {
                    prop.SetValue(component, o);
                }
            }

            for(var i = 0; i < m_Fields.Length; i++)
            {            
                var fi = type.GetField(m_FieldInfos[i].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi.IsInitOnly||fi.IsLiteral)
                {
                    continue;
                }                
                var o = m_Fields[i].GetValue();
                fi.SetValue(component, o);                                
            }
        }


        bool IsSkipGetValue(PropertyInfo info)
        {
#if UNITY_EDITOR
            if (info.DeclaringType == typeof(UnityEngine.MeshFilter))
            {
                if (info.PropertyType == typeof(UnityEngine.Mesh) && info.Name == "mesh")
                {
                    return true;
                }
            }

            if(info.DeclaringType == typeof(UnityEngine.Renderer))
            {
                if(info.PropertyType == typeof(UnityEngine.Material) && (info.Name == "material"))
                {
                    return true;
                }
                if (info.PropertyType == typeof(UnityEngine.Material[]) && (info.Name == "materials"))
                {
                    return true;
                }
            }
#endif
            return false;
        }       
    }
}
