using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTJ.UnityPlayerSyncEngine
{    
    public class SyncValuUnityEngineObject : SyncValueType<UnityEngine.Object>
    {
        int[] m_InstanceIDs;
        string[] m_Names;

        public SyncValuUnityEngineObject() : base() { }
        

        public SyncValuUnityEngineObject(object value,System.Type type):base(value,type)
        {        
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            if (m_Values != null)
            {
                for (var i = 0; i < m_Values.Length; i++)
                {
                    try
                    {

                        binaryWriter.Write(m_Values[i].GetInstanceID());                        
                        binaryWriter.Write(m_Values[i].name);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            if (m_Values != null)
            {
                m_InstanceIDs = new int[m_Values.Length];
                m_Names = new string[m_Values.Length];
                for (var i = 0; i < m_Values.Length; i++)
                {
                    m_InstanceIDs[i] = binaryReader.ReadInt32();
                    m_Names[i] = binaryReader.ReadString();
                }
            }
        }

        public override object GetValue()
        {
            if (m_Values == null)
            {
                return null;
            }
            
            // m_ValuesをinstanceIDとnameを使って紐づける

            var type = SyncType.GetType(m_Type);            
            for (var i = 0; i < m_Values.Length; i++)
            {
                // HierarchyからObjectを検索する
                m_Values[i] = SyncGameObject.FintObject(m_InstanceIDs[i]);
                if(m_Values[i] != null)
                {
                    continue;
                }
#if UNITY_EDITOR
                // Project内からAssetを検索する
                var name = ReplaceInstanceName(m_Names[i]);
            var filter = $"{name} t:{m_Type.Name}";
                var guids = AssetDatabase.FindAssets(filter);
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);                    
                    m_Values[i]  = AssetDatabase.LoadAssetAtPath(path, type);
                }
#endif
            }


            if (type.IsArray)
            {                
                return m_Values;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return new List<UnityEngine.Object>(m_Values);                
            }
            else
            {

                return m_Values[0];
            }
        }

        // RuntimeではInstance化されたオブジェクト名にInstanceとつくので取り除く必要がある
        string ReplaceInstanceName(string name)
        {
            // Materialの場合
            name = name.Replace(" (Instance)", "");

            // Meshの場合
            name = name.Replace(" Instance", "");

            return name;
        }
    }



}