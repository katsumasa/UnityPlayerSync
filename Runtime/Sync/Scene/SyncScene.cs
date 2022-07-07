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
                var list = new List<SyncGameObject>();
                var gameObjects = scene.GetRootGameObjects();
                foreach (var gameObject in gameObjects)
                {
                    AddGameObjectInChildren(gameObject, list);
                }
                m_syncGameObjects = list.ToArray();
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

#if false
            if (m_Scene == null)
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.name == m_Name)
                    {
                        m_Scene = scene;
                        break;
                    }
                }
                if(m_Scene == null)
                {
                    m_Scene = SceneManager.CreateScene(m_Name);
                }
            }
#endif

            
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



        void AddGameObjectInChildren(GameObject go,List<SyncGameObject> syncGames)
        {
            if (go == null || syncGames == null)
            {
                return;
            }
            var syncGame = new SyncGameObject(go);
            syncGames.Add(syncGame);
            for(var i = 0; i < go.transform.childCount; i++)
            {
                AddGameObjectInChildren(go.transform.GetChild(i).gameObject, syncGames);
            }
        }
    }
}
