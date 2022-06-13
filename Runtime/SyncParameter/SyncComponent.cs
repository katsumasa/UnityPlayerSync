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

        public static Dictionary<string, System.Type> m_AvaiavleTypes = new Dictionary<string, System.Type>()
        {
            {"UnityEngine.Component",typeof(UnityEngine.Component) },
        };

        
        
        
        protected SyncValueType[] m_Properties;
        protected SyncPropertyInfo[] m_PropertyInfos;

        protected SyncValueType[] m_Fields;
        protected SyncFieldInfo[] m_FieldInfos;


        private Component m_Component;

        


        public SyncComponent(Component component) : base(component)
        {
            m_Component = component;    

            var type = component.GetType();                
            var props = type.GetProperties();
                

            List<SyncValueType> list = new List<SyncValueType>();
            List<SyncPropertyInfo> propList = new List<SyncPropertyInfo>();
            foreach(var prop in props)
            {
                if(!SyncValueType.IsAvailableType(prop.PropertyType))
                {
                    continue;
                }                                       
                if(!prop.CanRead)
                {
                    continue;
                }

                // UnityではObsoleteになったgetterでNotSupportedExceptionをthrowしている為、キャッチしてスルーする必要があるが、
                // TestRunnerがSystem.ExceptionでキャッチしているのでこちらもSystem.Exceptionでキャッチせざるを得ない
                object o;
                try
                {
                    o = prop.GetValue(component);
                }
                //catch(System.NotSupportedException)
                catch(System.Exception)
                {                            
                    continue;
                }                    
                list.Add(SyncValueType.Allocater(prop.PropertyType, o));
                propList.Add(new SyncPropertyInfo(prop));                    
            }
            m_Properties = list.ToArray();
            m_PropertyInfos = propList.ToArray();

                     
            list.Clear();
            var fiList = new List<SyncFieldInfo>();
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var fi in fis)
            {
                var at = Attribute.GetCustomAttribute(fi, typeof(SerializeField));
                var IsSerializeField = at != null ? true : false;
                if (!SyncValueType.IsAvailableType(fi.FieldType))
                {
                    continue;
                }
                if(fi.IsInitOnly)
                {
                    continue;
                }
                if (fi.IsLiteral)
                {
                    continue;
                }
                if(fi.IsPrivate && !IsSerializeField)
                {
                    continue;
                }
                if (fi.IsStatic)
                {
                    continue;
                }
                
                object o;
                try
                {
                    o = fi.GetValue(component);
                }
                catch(System.Exception)
                {
                    continue;
                }
                list.Add(SyncValueType.Allocater(fi.FieldType,o));
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
            
            m_Properties = new SyncValueType[len];
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_Properties[i] = SyncValueType.Allocater(SyncType.GetType(m_PropertyInfos[i].PropertyType));
                m_Properties[i].Deserialize(binaryReader);
            }

            len = binaryReader.ReadInt32();
            m_FieldInfos = new SyncFieldInfo[len];            
            for(var i = 0; i < len; i++)
            {
                m_FieldInfos[i] = new SyncFieldInfo();
                m_FieldInfos[i].Deserialize(binaryReader);
            }

            m_Fields = new SyncValueType[len];
            for(var i = 0; i < len; i++)
            {
                m_Fields[i] = SyncValueType.Allocater(SyncType.GetType(m_FieldInfos[i].FieldType));
                m_Fields[i].Deserialize(binaryReader);
            }
        }


        public override void WriteBack()
        {
            base.WriteBack();
            if(m_Component == null)
            {
                return;
            }
            var type = m_Component.GetType();                
            for(var i = 0; i < m_Properties.Length; i++)
            {
                if(!m_Properties[i].hasChanged)
                {
                    continue;
                }
                var prop = type.GetProperty(m_PropertyInfos[i].Name);
                if (!prop.CanWrite)
                {
                    continue;
                }                                    
                var o = m_Properties[i].GetValue();
                prop.SetValue(m_Component, o);                
            }

            for(var i = 0; i < m_Fields.Length; i++)
            {
                if(!m_Fields[i].hasChanged)
                {
                    continue;
                }
                var fi = type.GetField(m_FieldInfos[i].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi.IsInitOnly||fi.IsLiteral)
                {
                    continue;
                }                
                var o = m_Fields[i].GetValue();
                fi.SetValue(m_Component, o);                                
            }
        }
    }
}
