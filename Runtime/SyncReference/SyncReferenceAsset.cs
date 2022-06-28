using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UTJ.UnityPlayerSyncEngine
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
            var type = SyncType.GetType(m_Type);
            if(type.IsArray)
            {
                type = type.GetElementType();
            } 
            else if(type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }


#if UNITY_EDITOR
            // AssetDataBase内のAssetを検索する
            for (var i = 0; i < m_Values.Length; i++)
            {
                // Project内からAssetを検索する
                var name = ReplaceInstanceName(type,m_Names[i]);
                var filter = $"{name} t:{type.Name}";

                var guids = AssetDatabase.FindAssets(filter);
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_Values[i] = AssetDatabase.LoadAssetAtPath(path, type);
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
