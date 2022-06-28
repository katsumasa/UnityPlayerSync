using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{
    public abstract class Sync : ISerializer
    {
        public abstract void Serialize(BinaryWriter binaryWriter);
        public abstract void Deserialize(BinaryReader binaryReader);
    }
}