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
            // AssetDataBase����Asset����������
            for (var i = 0; i < m_Values.Length; i++)
            {
                // Project������Asset����������
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
            // Hierarchy����Asset����������
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
        // Runtime�ł�Instance�����ꂽ�I�u�W�F�N�g����Instance�Ƃ��̂Ŏ�菜���K�v������
        string ReplaceInstanceName(System.Type type,string name)
        {
            if (type == typeof(Material) || type == typeof(Material[]) || type == typeof(List<Material>))
            {
                // Material�̏ꍇ
                name = name.Replace(" (Instance)", "");
            }
            if(type == typeof(Mesh) || type == typeof(Mesh[]) || type == typeof(List<Mesh>))
            {
                // Mesh�̏ꍇ
                name = name.Replace(" Instance", "");
            }
            // Shader�̏ꍇ
            if(type == typeof(Shader) || type == typeof(Shader[]) || type == typeof(List<Shader>))
            {
                name = System.IO.Path.GetFileName(name);
            }
            

            return name;
        }
#endif
    }
}
