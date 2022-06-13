using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public abstract class Sync
    {
        public virtual void Serialize(BinaryWriter binaryWriter) { }
        public virtual void Deserialize(BinaryReader binaryReader) { }
    }
}