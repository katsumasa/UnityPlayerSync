﻿using System.IO;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncObject : Sync
    {
        protected object m_object;

        
        public SyncObject(object obj)
        {
            m_object = obj;
        }
        

        public override void Serialize(BinaryWriter binaryWriter) { }
        

        public override void Deserialize(BinaryReader binaryReader) { }        

       
    }
}