using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncUnityEngineObject : SyncObject
    {
        protected SyncInt m_HideFlags;
        protected SyncString m_Name;        
        protected int m_InstanceID;
        
        

        private UnityEngine.Object m_Object;

        public HideFlags hideFlags
        {
            get
            {
                return (HideFlags)m_HideFlags.value;
            }

            set
            {
                m_HideFlags.value = (int)value;
                m_HasChanded = true;
            }
                    
        }

        public string name { get => m_Name.value; set { m_Name.value = value; } }


        public SyncUnityEngineObject() : base(typeof(UnityEngine.Object)) { }

        public SyncUnityEngineObject(System.Type type) : base(type)
        {
            m_HideFlags = new SyncInt(0);
            m_Name = new SyncString("");
        }


        public SyncUnityEngineObject(object obj):base(obj)
        {
            m_Object = (UnityEngine.Object)obj;                        
            m_HideFlags = new SyncInt((int)m_Object.hideFlags);
            m_InstanceID = m_Object.GetInstanceID();


            m_Name = new SyncString(m_Object.name);            
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_HideFlags.Serialize(binaryWriter);
            m_Name.Serialize(binaryWriter);
            binaryWriter.Write(m_InstanceID);            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_HideFlags.Deserialize(binaryReader);
            m_Name.Deserialize(binaryReader);            
            m_InstanceID = binaryReader.ReadInt32();
        }

        public virtual void WriteBack()
        {    
            if(m_Object == null)
            {
                return;
            }
            if (m_HideFlags.hasChanged)
            {
                m_Object.hideFlags = (HideFlags)m_HideFlags.value;
            }
            if (m_Name.hasChanged)
            {
                m_Object.name = m_Name.value;
            }            
        }

        public int GetInstanceID()
        {
            return m_InstanceID;
        }
    }



}