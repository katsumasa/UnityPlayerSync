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
                list.Add(SyncValueType.Allocater(prop.PropertyType.Name, o));
                propList.Add(new SyncPropertyInfo(prop));                    
            }
            m_Properties = list.ToArray();
            m_PropertyInfos = propList.ToArray();
            
            
            
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
            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);                                                
            var len = binaryReader.ReadInt32();            
            m_PropertyInfos = new SyncPropertyInfo[len];            
            m_Properties = new SyncValueType[len];
            for (var i = 0; i < m_PropertyInfos.Length; i++)
            {
                m_PropertyInfos[i] = new SyncPropertyInfo();
                m_PropertyInfos[i].Deserialize(binaryReader);
            }
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_Properties[i] = SyncValueType.Allocater(m_PropertyInfos[i].PropertyType);
                m_Properties[i].Deserialize(binaryReader);
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
                var o = m_Properties[i].GetValue(m_Properties[i].Type.Name);
                prop.SetValue(m_Component, o);                    
            }            
        }                                
    }
}
