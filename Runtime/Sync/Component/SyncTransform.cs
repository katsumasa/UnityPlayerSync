using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncTransform : SyncUnityEngineObject
    {        
        static List<SyncTransform> m_SyncTransforms;
        static List<SyncTransform> syncTransforms
        {
            get
            {
                if(m_SyncTransforms == null)
                {
                    m_SyncTransforms = new List<SyncTransform>();
                }
                return m_SyncTransforms;
            }
        }


        public static SyncTransform Find(Component component)
        {
            foreach(var sync in syncTransforms)
            {
                if (component.Equals(sync.Object))
                {
                    return sync;
                }                
            }
            return null;
        }


        public static SyncTransform Find(int instanceID)
        {
            foreach (var sync in syncTransforms)
            {
                if(sync.GetInstanceID() == instanceID)
                {
                    return sync;
                }
                Debug.Log(sync.GetInstanceID());
            }
            return null;
        }


        public static void Clear()
        {
            if (m_SyncTransforms != null)
            {
                m_SyncTransforms.Clear();
            }
        }
        

        public Transform GetTransform()
        {
            return (Transform)m_object;
        }


        public SyncTransform(object obj) : base(obj) 
        {                         
            syncTransforms.Add(this);
        }

        ~SyncTransform()
        {
            syncTransforms.Remove(this);
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            var transform = (Transform)m_object;
            var localPosition = new SyncVector3(transform.localPosition);
            var localRotation = new SyncQuaternion(transform.localRotation);
            var localScale = new SyncVector3(transform.localScale);
            int parentInstanceID = (transform.parent != null) ? transform.parent.GetInstanceID() : SyncUnityEngineObject.InstanceID_None;
                                                
            base.Serialize(binaryWriter);         
            binaryWriter.Write(parentInstanceID);
            localPosition.Serialize(binaryWriter);
            localRotation.Serialize(binaryWriter);
            localScale.Serialize(binaryWriter);                                    
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            var transform = (Transform)m_object;
            var localPosition = new SyncVector3(transform.localPosition);
            var localRotation = new SyncQuaternion(transform.localRotation);
            var localScale = new SyncVector3(transform.localScale);
                        
            base.Deserialize(binaryReader);           
            var parentInstanceID = binaryReader.ReadInt32();
            localPosition.Deserialize(binaryReader);
            localRotation.Deserialize(binaryReader);
            localScale.Deserialize(binaryReader);

            // 親のTransformは構築済み
            // From Player To Editor
            // 
            // 
            //
            // From Editor To Player
            //
            // parentInstanceIDにはEditor上のInstanceIDが入っている
            // 



            var parentSyncTransform = SyncTransform.GetSyncTransform(parentInstanceID);
            if (parentSyncTransform != null)
            {
                var rectTransform = transform as Transform;
                if (rectTransform == null)
                {                    
                    transform.SetParent((Transform)parentSyncTransform.m_object,false);
                }
                else
                {
                    // ワーニング対応
                    // Parent of RectTransform is being set with parent property.
                    // Consider using the SetParent method instead, with the worldPositionStays argument set to false.
                    // This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.
                    rectTransform.SetParent((Transform)parentSyncTransform.m_object, false);
                }
            }
            transform.localPosition = (Vector3)localPosition.GetValue();
            transform.localRotation = (Quaternion)localRotation.GetValue();
            transform.localScale = (Vector3)localScale.GetValue();

        }


        static SyncTransform GetSyncTransform(int instanceID)
        {
            if(instanceID == SyncUnityEngineObject.InstanceID_None)
            {
                return null;
            }

            foreach(var syncTransForm in syncTransforms)
            {
                if(
                    (syncTransForm.GetInstanceID() == instanceID) ||
                    (syncTransForm.GetInstanceEditorID() == instanceID)
                    )
                {
                    return syncTransForm;
                }
            }
            return null;
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