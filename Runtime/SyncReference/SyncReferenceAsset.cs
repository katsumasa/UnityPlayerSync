using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncReferenceAsset : SyncValueUnityEngineObject
    {
        public SyncReferenceAsset(object value,System.Type type) : base(value, type) { }


        public override object GetValue()
        {
            if (m_Values == null)
            {
                return null;
            }
            System.Type type;


            type = SyncType.GetType(m_Type);
            if (type == null)
            {                               
                return null;
            }
            if(type.IsArray)
            {
                type = type.GetElementType();
            } 
            else if(type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            // EditorとRuntimeでAssetが異なる場合の処理
#if UNITY_EDITOR
            if(type == typeof(RuntimeAnimatorController))
            {
                type = typeof(UnityEditor.Animations.AnimatorController);
            }
            if(type == typeof(UnityEngine.Audio.AudioMixer))
            {                
                type = System.Type.GetType("UnityEditor.Audio.AudioMixerController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");
            }
            if(type == typeof(UnityEngine.Audio.AudioMixerGroup))               
            {
                // UnityEditor.Audio.AudioMixerGroupControllerは非公開クラスの為、文字列で変換する
                type = System.Type.GetType("UnityEditor.Audio.AudioMixerGroupController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");
            }            
#else

#endif



#if UNITY_EDITOR
            for (var i = 0; i < m_Values.Length; i++)
            {
                // AssetDataBase内のAssetを検索する
                var name = ReplaceInstanceName(type,m_Names[i]);
                var typeName = type.Name;
                

                var filter = $"{name} t:{typeName}";
                var guids = AssetDatabase.FindAssets(filter);
                foreach(var guid in guids) 
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);                    
                    var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach(var obj in objs)
                    {
                        // nullの場合もあると・・・
                        if(obj == null)
                        {
                            continue;
                        }
                        try
                        {
                            if ((obj.GetType() == type) && (obj.name == name))
                            {
                                m_Values[i] = obj;
                                break;
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    if(m_Values[i] != null)
                    {
                        break;
                    }
                }                
                

                if (m_Values[i] == null)
                {
                    // Resourecesから検索する
                    var objs = Resources.FindObjectsOfTypeAll(type);
                    foreach (var obj in objs)
                    {
                        if (obj == null)
                        {
                            continue;
                        }
                        try
                        {
                            if ((obj.GetType() == type) && (obj.name == name))
                            {
                                m_Values[i] = obj;
                                break;
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    if (m_Values[i] != null)
                    {
                        break;
                    }
                }
                if (m_Values[i] == null)
                {
                    Debug.LogWarning($"{filter} is not found.");
                }
            }
#else
            // Hierarchy内のAssetを検索する
            var objects = Resources.FindObjectsOfTypeAll(type);
            for (var i = 0; i < m_Values.Length; i++)
            {
                for(var j = 0; j < objects.Length; j++)
                {
                    if(objects[j].name == m_Names[i])
                    {
                        m_Values[i] = objects[j];
                        break;
                    }
                }
            }
#endif
            return base.GetValue();
        }


#if UNITY_EDITOR
        // RuntimeではInstance化されたオブジェクト名にInstanceとつくので取り除く必要がある
        string ReplaceInstanceName(System.Type type,string name)
        {
            if (type == typeof(Material) || type == typeof(Material[]) || type == typeof(List<Material>))
            {
                // Materialの場合
                name = name.Replace(" (Instance)", "");
            }
            if(type == typeof(Mesh) || type == typeof(Mesh[]) || type == typeof(List<Mesh>))
            {
                // Meshの場合
                name = name.Replace(" Instance", "");
            }
            // Shaderの場合
            if(type == typeof(Shader) || type == typeof(Shader[]) || type == typeof(List<Shader>))
            {
                name = System.IO.Path.GetFileName(name);
            }
            

            return name;
        }
#endif
    }
}
