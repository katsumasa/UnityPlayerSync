using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncUnityEngineObject : SyncObject
    {
        protected int m_InstanceID;


        UnityEngine.Object m_Object
        {
            get { return (UnityEngine.Object)m_object; }
        }
                      
        
        public SyncUnityEngineObject(object obj) : base(obj) { }
                            

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Object.GetInstanceID());
            binaryWriter.Write((int)m_Object.hideFlags);
            binaryWriter.Write(m_Object.name);            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_InstanceID = binaryReader.ReadInt32();
            m_Object.hideFlags = (HideFlags)binaryReader.ReadInt32();
            m_Object.name = binaryReader.ReadString();
        }        

        public int GetInstanceID()
        {
            return m_InstanceID;
        }
    }
}