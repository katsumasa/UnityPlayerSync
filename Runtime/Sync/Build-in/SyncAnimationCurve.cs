//
// Programed by Katsumasa Kimuta
//
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// AnimationCurveを同期する為のクラス
    /// </summary>
    public class SyncAnimationCurve : SyncValueType<AnimationCurve>
    {
        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="anim">AnimationCurve</param>
        public SyncAnimationCurve(object anim) : base(anim) { }


        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="binaryWriter">BinaryWriter</param>
        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            for (var i = 0; i < m_Length; i++)
            {
                binaryWriter.Write((int)m_Values[i].postWrapMode);
                binaryWriter.Write((int)m_Values[i].preWrapMode);
                binaryWriter.Write(m_Values[i].length);
                for (var j = 0; j < m_Values[i].length; j++)
                {
                    binaryWriter.Write(m_Values[i].keys[j].time);
                    binaryWriter.Write(m_Values[i].keys[j].value);
                    binaryWriter.Write(m_Values[i].keys[j].inTangent);
                    binaryWriter.Write(m_Values[i].keys[j].outTangent);
                    binaryWriter.Write(m_Values[i].keys[j].inWeight);                    
                    binaryWriter.Write(m_Values[i].keys[j].outWeight);
                                        
                    binaryWriter.Write((int)m_Values[i].keys[j].weightedMode);
                }
            }
        }


        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="binaryReader">BinaryReader</param>
        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            for (var i = 0; i < m_Length; i++)
            {
                m_Values[i].postWrapMode = (WrapMode)binaryReader.ReadInt32();
                m_Values[i].preWrapMode = (WrapMode)binaryReader.ReadInt32();
                var len = binaryReader.ReadInt32();
                m_Values[i].keys = new Keyframe[len];
                for(var j = 0; j < len; j++)
                {                    
                    m_Values[i].keys[j] = new Keyframe(
                        binaryReader.ReadSingle(),
                        binaryReader.ReadSingle(),
                        binaryReader.ReadSingle(),
                        binaryReader.ReadSingle(),
                        binaryReader.ReadSingle(),
                        binaryReader.ReadSingle());
                    m_Values[i].keys[j].weightedMode = (WeightedMode)binaryReader.ReadInt32();
                }
            }
        }
    }
}
