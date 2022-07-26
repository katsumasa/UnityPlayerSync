using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncScene : SyncObject
    {        
        protected SyncGameObject[] m_syncGameObjects;

        Scene m_Scene
        {
            get { return (Scene)m_object; }            
        }


        

        public SyncScene(Scene scene) : base(scene) 
        {
            if (scene.rootCount <= 0)
            {
                m_syncGameObjects = new SyncGameObject[0];
            }
            else
            {
                var gameObjects = scene.GetRootGameObjects();
                int count = 0;
                foreach (var gameObject in gameObjects)
                {
                    GetGameObjectCount(gameObject, ref count);
                }
                m_syncGameObjects = new SyncGameObject[count];
                count = 0;
                foreach (var gameObject in gameObjects)
                {                    
                    AddGameObjectInChildren(gameObject, ref m_syncGameObjects,ref count);                    
                }                
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            var scene = (Scene)m_object;                        
            binaryWriter.Write(scene.name);            
            binaryWriter.Write(m_syncGameObjects.Length);
            for(var i = 0; i < m_syncGameObjects.Length; i++)
            {
                m_syncGameObjects[i].Serialize(binaryWriter);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var scene = (Scene)m_object;
            scene.name = binaryReader.ReadString();            
            var len = binaryReader.ReadInt32();
            
            m_syncGameObjects = new SyncGameObject[len];
            for(var i = 0; i < len; i++)
            {
                var gameObject = new GameObject();                
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, scene);
                m_syncGameObjects[i] = new SyncGameObject(gameObject);
                m_syncGameObjects[i].Deserialize(binaryReader);
            }
        }


        public  void WriteBack()
        {            
            for(var i = 0; i < m_syncGameObjects.Length; i++)
            {
                m_syncGameObjects[i].WriteBack();
            }
        }

        void  GetGameObjectCount(GameObject go,ref int count)
        {
            if (go == null)
            {
                return;
            }

            count++;
            for (var i = 0; i < go.transform.childCount; i++)
            {
                GetGameObjectCount(go.transform.GetChild(i).gameObject, ref count);
            }
        }


        void AddGameObjectInChildren(GameObject go,ref SyncGameObject[] syncGames,ref int index)
        {            
            if (go == null)
            {
                return;
            }
            syncGames[index] = new SyncGameObject(go);           
            index++;
            for(var i = 0; i < go.transform.childCount; i++)
            {
                AddGameObjectInChildren(go.transform.GetChild(i).gameObject, ref syncGames,ref index);
            }
        }
    }
}
