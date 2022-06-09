using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTJ.UnityPlayerSyncEngine
{
    public class SyncScene : SyncObject
    {
        protected SyncInt m_Handle;
        protected SyncString m_Name;
        protected SyncInt m_RootCount;        
        protected SyncGameObject[] m_syncGameObjects;

        private Scene m_Scene;


        public SyncScene() : base(typeof(Scene)) 
        {
            m_Handle = new SyncInt();
            m_Name = new SyncString();
            m_RootCount = new SyncInt();
        }

        public SyncScene(Scene scene) : base(scene) 
        {
            m_Scene = scene;
            m_Name = new SyncString(scene.name);
            m_Handle = new SyncInt(scene.handle);
            m_RootCount = new SyncInt(scene.rootCount);
            var list = new List<SyncGameObject>();
            foreach(var gameObject in scene.GetRootGameObjects())
            {
                AddGameObjectInChildren(gameObject,list);
            }
            m_syncGameObjects = list.ToArray();
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            m_Handle.Serialize(binaryWriter);
            m_Name.Serialize(binaryWriter);
            m_RootCount.Serialize(binaryWriter);
            binaryWriter.Write(m_syncGameObjects.Length);
            for(var i = 0; i < m_syncGameObjects.Length; i++)
            {
                m_syncGameObjects[i].Serialize(binaryWriter);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            m_Handle.Deserialize(binaryReader);
            m_Name.Deserialize(binaryReader);
            m_RootCount.Deserialize(binaryReader);


            if (m_Scene == null)
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.name == m_Name.value)
                    {
                        m_Scene = scene;
                        break;
                    }
                }
                if(m_Scene == null)
                {
                    m_Scene = SceneManager.CreateScene(m_Name.value);
                }
            }


            var len = binaryReader.ReadInt32();
            m_syncGameObjects = new SyncGameObject[len];
            for(var i = 0; i < len; i++)
            {
                m_syncGameObjects[i] = new SyncGameObject();
                m_syncGameObjects[i].Deserialize(binaryReader);
            }
        }


        public void WriteBack()
        {
            m_Scene.name = m_Name.value;

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
