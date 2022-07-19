
using System.IO;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncSceneManager : SyncObject
    {        
        protected SyncScene[] m_Scenes;


        public SyncSceneManager(bool isPlayer):base(typeof(SceneManager))
        {
            if (isPlayer)
            {
                var len = SceneManager.sceneCount;
                m_Scenes = new SyncScene[len];
                for (var i = 0; i < len; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    m_Scenes[i] = new SyncScene(scene);
                }
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

            SyncGameObject.ClearList();
            SyncTransform.Clear();
            SyncComponent.Clear();



            base.Deserialize(binaryReader);
            var len = binaryReader.ReadInt32();
            m_Scenes = new SyncScene[len];
            for(var i = 0; i < len; i++)
            {
                Scene scene;
#if UNITY_EDITOR                
                if(i == 0)
                {
                    scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                }
                else
                {
                    scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                }
#else
                // Runtime上でDeserializeが走ることは想定していない為、ここが実行されることは現時点では想定外
                scene = new Scene();
#endif
                m_Scenes[i] = new SyncScene(scene);
                m_Scenes[i].Deserialize(binaryReader);
            }
        }    
        

        public void WriteBack()
        {
            for(var i = 0; i < m_Scenes.Length; i++)
            {
                m_Scenes[i].WriteBack();
            }
        }
    }
}