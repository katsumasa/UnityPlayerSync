using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTJ.UnityPlayerSyncEngine
{

    public class SyncSceneManager : SyncUnityEngineObject
    {        
        protected SyncScene[] m_Scenes;

        
        public SyncSceneManager():base(typeof(SceneManager))
        {
            var len = SceneManager.sceneCount;
            m_Scenes = new SyncScene[len];
            for(var i = 0; i < len; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                m_Scenes[i] = new SyncScene(scene);
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(m_Scenes.Length);
            for(var i = 0; i < m_Scenes.Length; i++)
            {
                m_Scenes[i].Serialize(binaryWriter);
            }
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            var len = binaryReader.ReadInt32();

            m_Scenes = new SyncScene[len];
            for(var i = 0; i < len; i++)
            {                
                m_Scenes[i] = new SyncScene();
                m_Scenes[i].Deserialize(binaryReader);
            }
        }

        public override void WriteBack()
        {
            base.WriteBack();

            for(var i = 0; i < m_Scenes.Length; i++)
            {
                m_Scenes[i].WriteBack();
            }
        }


    }
}