using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncUnityEngineObject : SyncObject
    {
        private int m_InstanceID;



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
#if !UNITY_EDITOR
            m_InstanceID = m_Object.GetInstanceID();
#endif
        }
                            

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_InstanceID);
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
            m_Object.hideFlags = (HideFlags)binaryReader.ReadInt32();
            m_Object.name = binaryReader.ReadString();


            //Debug.Log($"instanceID:{m_InstanceID}");
        }        

        public int GetInstanceID()
        {
            return m_InstanceID;
        }
    }
}