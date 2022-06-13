using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncAssembly : Sync
    {
        protected string m_FullName = "";

        public string FullName
        {
            get { return m_FullName; }
        }

        public SyncAssembly() { }

        public SyncAssembly(Assembly assembly)
        {
            m_FullName = assembly.FullName;
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_FullName);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_FullName = binaryReader.ReadString();
        }
    }
}