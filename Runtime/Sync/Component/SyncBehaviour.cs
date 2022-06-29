//
// Programed by Katsumasa Kimura
//
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// Behaviour‚ð“¯Šú‚³‚¹‚éˆ×‚ÌƒNƒ‰ƒX
    /// </summary>
    public class SyncBehaviour : SyncComponent
    {        
        public SyncBehaviour(object obj):base(obj) { }
                            
        public override void Serialize(BinaryWriter binaryWriter)
        {
            var behaviour = (Behaviour)m_object;
            base.Serialize(binaryWriter);
            binaryWriter.Write(behaviour.enabled);            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            var behaviour = (Behaviour)m_object;
            base.Deserialize(binaryReader);
            behaviour.enabled = binaryReader.ReadBoolean();
        }                
    }
}
