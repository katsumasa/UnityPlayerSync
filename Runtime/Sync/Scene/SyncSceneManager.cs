
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncSceneManager : SyncObject
    {        

        public static void ClearCache()
        {
            SyncGameObject.ClearCache();
            SyncTransform.ClearCache();
            SyncComponent.ClearCache();
            SyncType.Caches.Clear();
        }

        protected SyncScene[] m_Scenes;


        public SyncSceneManager(bool isPlayer):base(typeof(SceneManager))
        {
            if (isPlayer)
            {
                // DontDestroyOnloadされたGameObjectは専用のSceneへ移動されるが、
                // SceneManagerからのそのSceneへのアクセスパスが無い為、
                // DontDestroyOnloadされたGameObjectのscneプロパティからscene情報を取得するというトリッキーコード
                bool dontDestroyOnload = false;
                var go = new GameObject();
                GameObject.DontDestroyOnLoad(go);
                var dontDestroyOnloadScne = go.scene;
                GameObject.DestroyImmediate(go);
                if (dontDestroyOnloadScne != null && dontDestroyOnloadScne.rootCount > 0)
                {
                    dontDestroyOnload = true;
                }
                var len = SceneManager.sceneCount;
                if (dontDestroyOnload)
                {
                    m_Scenes = new SyncScene[len + 1];
                }
                else
                {
                    m_Scenes = new SyncScene[len];
                }
                
                for (var i = 0; i < len; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    m_Scenes[i] = new SyncScene(scene);
                }
                if (dontDestroyOnload)
                {
                    m_Scenes[m_Scenes.Length - 1] = new SyncScene(dontDestroyOnloadScne);
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

            ClearCache();

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

#if UNITY_EDITOR
                // Unity Editorでは名前無しのSceneは一つしか存在出来ない為、マルチシーンを構築する為にはSceneをSaveする必要がある。
                // ここではプロジェクトのテンプフォルダーへScneを保存しています。
                var name = scene.name;
                var fpath = UnityEditor.FileUtil.GetUniqueTempPathInProject();
                fpath = Path.GetDirectoryName(fpath);
                fpath = Path.Combine(fpath, scene.name);                
                fpath = System.IO.Path.ChangeExtension(fpath, "unity");
                EditorSceneManager.SaveScene(scene,fpath);                
#endif
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