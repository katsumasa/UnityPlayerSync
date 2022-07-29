using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncLightBakingOutput : SyncValueObject<LightBakingOutput>
    {
        public SyncLightBakingOutput(object lightBakingOutput): base(lightBakingOutput) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for(var i = 0; i < m_Length; i++)
            {
                var lightBakingOutput = m_Values[i];
                binaryWriter.Write(lightBakingOutput.isBaked);
                binaryWriter.Write((int)lightBakingOutput.lightmapBakeType);
                binaryWriter.Write((int)lightBakingOutput.mixedLightingMode);
                binaryWriter.Write(lightBakingOutput.occlusionMaskChannel);
                binaryWriter.Write(lightBakingOutput.probeOcclusionLightIndex);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                var lightBakingOutput = new LightBakingOutput();
                lightBakingOutput.isBaked = binaryReader.ReadBoolean();
                lightBakingOutput.lightmapBakeType = (LightmapBakeType)binaryReader.ReadInt32();
                lightBakingOutput.mixedLightingMode = (MixedLightingMode)binaryReader.ReadInt32();
                lightBakingOutput.occlusionMaskChannel = binaryReader.ReadInt32();
                lightBakingOutput.probeOcclusionLightIndex = binaryReader.ReadInt32();
            }
        }
    }
}