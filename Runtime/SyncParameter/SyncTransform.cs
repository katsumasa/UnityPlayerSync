using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncTransform : SyncComponent
    {
        protected SyncVector3 m_localPosition;
        protected SyncVector3 m_localScale;
        protected SyncVector3 m_localRotation;
        protected SyncInt m_parentInstanceID;
        protected SyncInt m_childCount;
        protected SyncInt m_sceneHn;


        public SyncTransform(Transform transform) : base(transform)
        {
            m_localPosition = new SyncVector3(transform.localPosition);
            m_localRotation = new SyncVector3(transform.localRotation.eulerAngles);
            m_localScale = new SyncVector3(transform.localScale);
            if (transform.parent != null)
            {
                m_parentInstanceID = new SyncInt(transform.parent.GetInstanceID());
            }
            else
            {
                m_parentInstanceID = new SyncInt(-1);
            }
            m_sceneHn = new SyncInt(transform.gameObject.scene.handle);
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_localPosition.Serialize(binaryWriter);
            m_localRotation.Serialize(binaryWriter);
            m_localScale.Serialize(binaryWriter);
            m_parentInstanceID.Serialize(binaryWriter);
            m_sceneHn.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_localPosition.Deserialize(binaryReader);
            m_localRotation.Deserialize(binaryReader);
            m_localScale.Deserialize(binaryReader);
            m_parentInstanceID.Deserialize(binaryReader);
            m_sceneHn.Deserialize(binaryReader);
        }

        
    }
}