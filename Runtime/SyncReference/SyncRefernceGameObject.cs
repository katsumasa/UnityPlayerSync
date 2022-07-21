using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncRefernceGameObject : SyncValueType<GameObject>
    {
        protected int[] m_InstanceIDs;
        protected string[] m_Names;

        public SyncRefernceGameObject() : base() { }
        public SyncRefernceGameObject(object value,System.Type type) : base(value,type) { }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            if (m_Values != null)
            {
                for (var i = 0; i < m_Values.Length; i++)
                {
                    binaryWriter.Write(m_Values[i].GetInstanceID());
                    binaryWriter.Write(m_Values[i].name);
                }
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            if (m_Values != null)
            {
                m_InstanceIDs = new int[m_Values.Length];
                m_Names = new string[m_Values.Length];
                for (var i = 0; i < m_Values.Length; i++)
                {
                    m_InstanceIDs[i] = binaryReader.ReadInt32();
                    m_Names[i] = binaryReader.ReadString();
                }
            }
        }


        public override object GetValue()
        {
            var gameObjects = new List<GameObject>();
            
            for (var i = 0; i < m_Values.Length; i++)
            {
                GameObject go = null;
#if UNITY_EDITOR
                var sync = SyncGameObject.Find(m_InstanceIDs[i]);
                if (sync != null)
                {
                    go = sync.gameObject;
                }
#else
                go = SyncUnityEngineObject.FindObjectFromInstanceID(m_InstanceIDs[i]) as GameObject;
#endif
                gameObjects.Add(go);


            }
            var array = gameObjects.ToArray();


            var type = SyncType.GetType(m_Type);
            if (type.IsArray)
            {
                return (object[])array;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return gameObjects;
            }
            else
            {
                try
                {
                    var items = (object[])array;
                    if (items.Length > 0)
                    {
                        return items[0];
                    }
                    return null;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }
        }
    }
}