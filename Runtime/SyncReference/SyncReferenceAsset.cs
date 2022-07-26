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
                // 型が得られない場合もある
                return null;
            }
            // 配列やList<>の場合、実際の型に変換する
            if(type.IsArray)
            {
                type = type.GetElementType();
            } 
            else if(type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            // Assetを検索する
            for (var i = 0; i < m_Values.Length; i++)
            {
                if (string.IsNullOrEmpty(m_Names[i])){
                    continue;
                }
#if UNITY_EDITOR
                var name = ReplaceInstanceName(type, m_Names[i]);
                m_Values[i] = FindAssetInAssetDataBase(type, name);
#else
                var name = m_Names[i];
#endif
                if (m_Values[i] == null)
                {
                    m_Values[i] = FindAssetInResources(type, name);
                }
                if (m_Values[i] == null)
                {
                    m_Values[i] = FindAssetInBuildinExtraResource(type, name);
                }
                if (m_Values[i] == null)
                {
                    Debug.LogWarning($"{type.Name} {name} is not found.");
                }
            }
            return base.GetValue();
        }


#if UNITY_EDITOR
        // RuntimeではInstance化されたオブジェクト名にInstanceとつくので取り除く必要がある
        string ReplaceInstanceName(System.Type type,string name)
        {
            if (type == typeof(Material))
            {
                // Materialの場合
                name = name.Replace(" (Instance)", "");
            }
            if(type == typeof(Mesh))
            {
                // Meshの場合
                name = name.Replace(" Instance", "");
            }
            // Shaderの場合
            // Shaderの場合、BuildInExtraResourcesから読む場合、パスがついている必要があるのでこのままだと困るなあ
            if(type == typeof(Shader))
            {
                name = System.IO.Path.GetFileName(name);
            }
            

            return name;
        }

        UnityEngine.Object FindAssetInAssetDataBase(System.Type type,string name)
        {            
            var typeName = type.Name;
            var filter = $"{name} t:{typeName}";
            var guids = AssetDatabase.FindAssets(filter);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in objs)
                {
                    // nullの場合もあると・・・
                    if (obj == null)
                    {
                        continue;
                    }
                    try
                    {
                        if ((obj.GetType() == type) && (obj.name == name))
                        {
                           return obj;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }                
            }
            return null;
        }
#endif
        UnityEngine.Object FindAssetInResources(System.Type type,string name)
        {

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
                        return obj;                    
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return null;
        }

        UnityEngine.Object FindAssetInBuildinExtraResource(System.Type type,string name)
        {
            string fpath;
            if(type == typeof(Sprite)||type == typeof(Texture2D))
            {
                // 拡張子は基本'psd'だが一部'png'のものも混じっている
                string[] paths = {"UI/Skin/", "" };
                string[] exts = {"psd","png" };
                
                for(var i = 0; i < 2; i++)
                {
                    fpath = $"{paths[i]}{name}";
                    fpath = System.IO.Path.ChangeExtension(fpath, exts[i]);
#if UNITY_EDITOR
                    var sprite = AssetDatabase.GetBuiltinExtraResource(type, fpath);
#else
                    var sprite = Resources.GetBuiltinResource(type, fpath);
#endif
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }
            }
            if(type == typeof(Material))
            {
                fpath = System.IO.Path.ChangeExtension(name, "mat");
#if UNITY_EDITOR
                return AssetDatabase.GetBuiltinExtraResource(type, fpath);
#else
                return Resources.GetBuiltinResource(type, fpath);
#endif
            }

#if UNITY_EDITOR
            if (type == typeof(LightmapParameters))
            {
                fpath = System.IO.Path.Combine("LightmapParameters",name);
                fpath = System.IO.Path.ChangeExtension(fpath, "giparams");
                return AssetDatabase.GetBuiltinExtraResource(type, fpath);
            }
#endif
            return null;
        }

    }
}
