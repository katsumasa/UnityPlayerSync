using System;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncUnityEngineObject : SyncObject,IDisposable
    {

        public static readonly int InstanceID_None = 0;


        public static UnityEngine.Object FindObjectFromInstanceID(int instanceId)
        {
            if(instanceId != InstanceID_None)
            {
                var type = typeof(UnityEngine.Object);
                var flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod;
                var ret = type.InvokeMember("FindObjectFromInstanceID", flags, null, null, new object[] { instanceId });
                return (UnityEngine.Object)ret;
            }            
            return null;
        }



        private int m_InstanceID = InstanceID_None;
        private int m_InstanceEditorID = InstanceID_None;



        UnityEngine.Object m_Object
        {
            get { return (UnityEngine.Object)m_object; }
        }

        public UnityEngine.Object Object
        {
            get { return m_Object; }
        }
                      
        
        public SyncUnityEngineObject(object obj) : base(obj) 
        {
#if UNITY_EDITOR
            m_InstanceEditorID = m_Object.GetInstanceID();            
#else
            m_InstanceID = m_Object.GetInstanceID();            
#endif
        }

        public virtual void Reset()
        {            
#if UNITY_EDITOR
            m_InstanceEditorID = m_Object.GetInstanceID();
#else
            m_InstanceID = m_Object.GetInstanceID();            
#endif
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_InstanceID);
            binaryWriter.Write(m_InstanceEditorID);
            if(m_Object == null)
            {
                binaryWriter.Write(0);
                binaryWriter.Write("null");
            }
            else
            {
                binaryWriter.Write((int)m_Object.hideFlags);
                binaryWriter.Write(m_Object.name);
            }
            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_InstanceID = binaryReader.ReadInt32();
            m_InstanceEditorID = binaryReader.ReadInt32();
            m_Object.hideFlags = (HideFlags)binaryReader.ReadInt32();
            m_Object.name = binaryReader.ReadString();
            //Debug.Log($"instanceID:{m_InstanceID}");
        }        

        public int GetInstanceID()
        {
            return m_InstanceID;
        }

        public int GetInstanceEditorID()
        {
            return m_InstanceEditorID;
        }

        public virtual void Dispose()
        {

        }
    }
}