//
// Programed by Katsumasa.Kimura
//
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncAnimatorControllerParameter : SyncValueType<AnimatorControllerParameter>
    {
        public SyncAnimatorControllerParameter(object arg) : base(arg) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for(var i = 0; i < m_Length; i++)
            {
                var param = m_Values[i];
                binaryWriter.Write(param.name);
                binaryWriter.Write(param.nameHash);
                binaryWriter.Write(param.defaultBool);
                binaryWriter.Write(param.defaultFloat);
                binaryWriter.Write(param.defaultInt);                
                binaryWriter.Write((int)param.type);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                var name = binaryReader.ReadString();
                var hash = binaryReader.ReadInt32();


                m_Values[i] = new AnimatorControllerParameter();
#if UNITY_EDITOR
                m_Values[i].name = name;
#endif
                m_Values[i].defaultBool = binaryReader.ReadBoolean();
                m_Values[i].defaultFloat = binaryReader.ReadSingle();
                m_Values[i].defaultInt = binaryReader.ReadInt32();
                m_Values[i].type = (AnimatorControllerParameterType)binaryReader.ReadInt32();
            }
        }
    }
}
