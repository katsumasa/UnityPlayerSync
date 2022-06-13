using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncTransform : SyncUnityEngineObject
    {



        protected SyncVector3 m_localPosition;
        protected SyncQuaternion m_localRotation;
        protected SyncVector3 m_localScale;        
        protected SyncInt m_parentInstanceID;
        protected SyncInt m_childCount;
        protected SyncInt m_SceneHn;

        private Transform m_Transform;

        public SyncTransform():base(typeof(Transform))
        {
            m_localPosition = new SyncVector3(Vector3.zero);
            m_localRotation = new SyncQuaternion(Quaternion.identity);
            m_localScale = new SyncVector3(Vector3.zero);
            m_parentInstanceID = new SyncInt();
            m_childCount = new SyncInt();
            m_SceneHn = new SyncInt();

        }


        public SyncTransform(Transform transform) : base(transform)
        {
            m_Transform = transform;
            if (transform != null)
            {
                m_localPosition = new SyncVector3(transform.localPosition);
                m_localRotation = new SyncQuaternion(transform.localRotation);
                m_localScale = new SyncVector3(transform.localScale);
                if (transform.parent != null)
                {
                    m_parentInstanceID = new SyncInt(transform.parent.GetInstanceID());
                }
                else
                {
                    m_parentInstanceID = new SyncInt(-1);
                }
                m_SceneHn = new SyncInt(transform.gameObject.scene.handle);
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_localPosition.Serialize(binaryWriter);
            m_localRotation.Serialize(binaryWriter);
            m_localScale.Serialize(binaryWriter);
            m_parentInstanceID.Serialize(binaryWriter);
            m_SceneHn.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_localPosition.Deserialize(binaryReader);
            m_localRotation.Deserialize(binaryReader);
            m_localScale.Deserialize(binaryReader);
            m_parentInstanceID.Deserialize(binaryReader);
            m_SceneHn.Deserialize(binaryReader);
        }

        public override void WriteBack()
        {
            base.WriteBack();
            if(m_parentInstanceID.value <= 0)
            {
                // 下記のエラーが発生するので、parentをnullにするのはペンディング
                // Setting the parent of a transform which resides in a Prefab instance is not possible
                // 
                //m_Transform.parent = null;
                if(m_Transform.gameObject.scene.handle != m_SceneHn.value)
                {
                    for(var i = 0; i < SceneManager.sceneCount; i++)
                    {
                        var scene = SceneManager.GetSceneAt(i);
                        if(scene.handle == m_SceneHn.value)
                        {
                            SceneManager.MoveGameObjectToScene(m_Transform.gameObject,scene);
                            break;
                        }
                    }
                }
            }
            else
            {
                m_Transform.parent = SyncTransform.GetTransform(m_parentInstanceID.value);
            }
            m_Transform.localPosition = m_localPosition.value;
            m_Transform.localRotation = m_localRotation.value;
            m_Transform.localScale = m_localScale.value;

        }

        public void SetTransform(Transform transform)
        {
            m_Transform = transform;
        }


        public static Transform GetTransform(int instanceID)
        {
            var ts = Transform.FindObjectsOfType<Transform>();
            foreach(var t in ts)
            {
                if(t.GetInstanceID() == instanceID)
                {
                    return t;
                }
            }
            return null;
            //var transform = Transform.FindObjectsOfType<Transform>().FirstOrDefault(q => q.GetInstanceID() == instanceID);
            //return transform;

        }

    }
}