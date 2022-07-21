using System;
//
// Programed by Katsumasa Kimura
//
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{
    /// <summary>
    /// Componentを同期させる為のクラス
    /// </summary>
    public class SyncComponent : SyncUnityEngineObject
    {
        enum DummyEnum
        {
            Dummy,
        };


        /// <summary>
        /// 生成されたSyncComponentのキャッシュ
        /// </summary>
        static List<SyncComponent> m_Caches;
        static List<SyncComponent> Caches
        {
            get
            {
                if(m_Caches == null)
                {
                    m_Caches = new List<SyncComponent>();
                }
                return m_Caches;
            }
        }


        /// <summary>
        /// ComponentをキーにしてSyncComponentを検索する
        /// </summary>
        /// <param name="component">キーとするComponent</param>
        /// <returns>SyncComponent</returns>
        public static SyncComponent Find(Component component)
        {
            foreach(var sys in Caches)
            {
                if(sys.Object == null)
                {
                    Caches.Remove(sys);
                    continue;
                }
                if (component.Equals(sys.Object))
                {
                    return sys;
                }
            }
            return null;
        }

        public static void ClearCache()
        {
            while (Caches.Count > 0)
            {
                Caches[0].Dispose();
            }
            Caches.Clear();
        }

        /// <summary>
        /// instanceIDをキーにしてSyncComponentを検索する
        /// </summary>
        /// <param name="instanceID">キーとするinstanceID</param>
        /// <returns>SyncComponent</returns>
        public static SyncComponent Find(int instanceID)
        {
            foreach (var sys in Caches)
            {
                if (sys.Object == null)
                {
                    Caches.Remove(sys);
                    continue;
                }
                if (sys.GetInstanceID() == instanceID)
                {
                    return sys;
                }
            }
            return null;
        }


        protected SyncValueObject[] m_Properties;
        protected SyncPropertyInfo[] m_PropertyInfos;
        protected SyncValueObject[] m_Fields;
        protected SyncFieldInfo[] m_FieldInfos;
        

        /// <summary>
        /// 対応するComponentを取得する
        /// </summary>
        /// <returns></returns>
        public Component GetComponent()
        {
            return (Component)m_object;
        }
        

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="obj">Component</param>
        /// <param name="isPlayer"></param>
        public SyncComponent(object obj,bool isPlayer = true) : base(obj)
        {
            Caches.Add(this);
            if (isPlayer)
            {
                Init();
            }
        }
        

        public override void Serialize(BinaryWriter binaryWriter)
        {            
            base.Serialize(binaryWriter);
            
            binaryWriter.Write(m_Properties.Length);
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_PropertyInfos[i].Serialize(binaryWriter);
            }
            for (var i = 0; i < m_Properties.Length; i++)
            {
                m_Properties[i].Serialize(binaryWriter);
            }

            binaryWriter.Write(m_Fields.Length);
            for(var i = 0; i < m_FieldInfos.Length; i++)
            {
                m_FieldInfos[i].Serialize(binaryWriter);
            }
            for(var i = 0; i < m_Fields.Length; i++)
            {
                m_Fields[i].Serialize(binaryWriter);
            }
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);

            var len = binaryReader.ReadInt32();
            
            m_PropertyInfos = new SyncPropertyInfo[len];
            
            for (var i = 0; i < m_PropertyInfos.Length; i++)
            {             
                m_PropertyInfos[i] = new SyncPropertyInfo();             
                m_PropertyInfos[i].Deserialize(binaryReader);
            }
            
            m_Properties = new SyncValueObject[len];            
            for (var i = 0; i < m_Properties.Length; i++)
            {
                var t = SyncType.GetType(m_PropertyInfos[i].PropertyType);
                if (t == null)
                {
                    var typeName = $"{m_PropertyInfos[i].PropertyType.Name},{m_PropertyInfos[i].PropertyType.Assembly.FullName}";
                    Debug.LogError($"{typeName} is not GetTyped.");


                    // 現状読み込めないBuild-In型は列挙型だけの筈なので、ダミーの列挙型で空読みを行う
                    //Debug.Log($"{typeName} is not found.");
                    var dummyEnum = DummyEnum.Dummy;
                    var sync = new SyncValueEnum(dummyEnum);
                    sync.Deserialize(binaryReader);                                        
                }
                else
                {
                    m_Properties[i] = SyncValueObject.Allocater(t, t);
                    m_Properties[i].Deserialize(binaryReader);
                }
            }

            len = binaryReader.ReadInt32();            
            m_FieldInfos = new SyncFieldInfo[len];            
            for(var i = 0; i < len; i++)
            {            
                m_FieldInfos[i] = new SyncFieldInfo();           
                m_FieldInfos[i].Deserialize(binaryReader);
            }

            
            m_Fields = new SyncValueObject[len];                      
            for(var i = 0; i < len; i++)
            {
                var t = SyncType.GetType(m_FieldInfos[i].FieldType);
                if (t == null)
                {
                    var typeName = $"{m_FieldInfos[i].FieldType.Name},{m_FieldInfos[i].FieldType.Assembly.FullName}";
                    Debug.LogError($"{typeName} is not GetTyped.");

                    // 現状読み込めないBuild-In型は列挙型だけの筈なので、ダミーの列挙型で空読みを行う
                    //Debug.Log($"{typeName} is not found.");
                    var dummyEnum = DummyEnum.Dummy;
                    var sync = new SyncValueEnum(dummyEnum);
                    sync.Deserialize(binaryReader);
                }
                else
                {
                    m_Fields[i] = SyncValueObject.Allocater(t, t);
                    m_Fields[i].Deserialize(binaryReader);
                }
            }
        }


        public void WriteBack()        
        {
            var component = (Component)m_object;

            var type = component.GetType();                
            for(var i = 0; i < m_Properties.Length; i++)
            {
                // デシリアライズ出来なかったプロパティはスキップ
                if(m_Properties[i] == null)
                {
                    continue;
                }

                var prop = type.GetProperty(m_PropertyInfos[i].Name);
                if(prop == null)
                {
                    continue;
                }
                if (!prop.CanWrite)
                {
                    continue;
                }
                if (IsSkipGetValue(component,prop.PropertyType,prop))
                {
                    continue;
                }
                var o = m_Properties[i].GetValue();
                if (o != null)
                {                   
                    var current = prop.GetValue(component);
                    if(!object.Equals(current,o))                    
                    {
                        prop.SetValue(component, o);
                    }
                }
            }

            for(var i = 0; i < m_Fields.Length; i++)
            {            
                // デシリアライズ出来無かったフィールドはスキップ
                if(m_Fields[i] == null)
                {
                    continue;
                }
                var fi = type.GetField(m_FieldInfos[i].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if(fi == null)
                {
                    continue;
                }
                if (fi.IsInitOnly||fi.IsLiteral)
                {
                    continue;
                }                
                var o = m_Fields[i].GetValue();
                if (o != null)
                {
                    var current = fi.GetValue(component);
                    if (!object.Equals(current, o))
                    {
                        try
                        {
                            fi.SetValue(component, o);
                        }
                        catch(System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }





        /// <summary>
        /// 使うと問題のあるプロパティを判定する
        /// </summary>
        /// <param name="info">プロパティ名</param>
        /// <returns>true:問題あり</returns>
        bool IsSkipGetValue(object declaringObject ,Type type,MemberInfo info)
        {
#if UNITY_EDITOR
            // mesh/material/materialsはEditorモードではアクセス出来ない為、Skip
            if (info.DeclaringType == typeof(UnityEngine.MeshFilter))
            {
                if (type == typeof(UnityEngine.Mesh) && info.Name == "mesh")
                {
                    return true;
                }
            }
            if(info.DeclaringType == typeof(UnityEngine.Renderer))
            {
                if(type == typeof(UnityEngine.Material) && (info.Name == "material"))
                {
                    return true;
                }
                if (type == typeof(UnityEngine.Material[]) && (info.Name == "materials"))
                {
                    return true;
                }
            }
#endif
            // Runtime<->Editor間でpixelRectを触ると色々不具合が起きるので触らない
            if(info.DeclaringType == typeof(Camera))
            {
                if(info.Name == "pixelRect")
                {
                    return true;
                }
            }

            
            if (info.DeclaringType == typeof(Animator))
            {
                // Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK
                if (
                    (info.Name == "bodyPosition") ||
                    (info.Name == "bodyRotation")) 
                {
                    return true; 
                }
                // Can't call GetPlaybackTime while not in playback mode. You must call StartPlayback before.
                if (info.Name == "playbackTime")
                {
                    return true;
                }
            }

            if(info.DeclaringType == typeof(Light))
            {
                if(
                    (info.Name == "shadowRadius ")||
                    (info.Name == "shadowAngle")  ||
                    (info.Name == "areaSize") ||
                    (info.Name == "lightmapBakeType")
                    )
                {
                    return true;
                }
            }           
            return false;
        }       


        public override void Reset()
        {
            base.Reset();

            var component = (Component)m_object;
            var type = component.GetType();                        
            for (var i = 0; i < m_PropertyInfos.Length; i++)
            {
                var propInfo = m_PropertyInfos[i];
                
                var prop = type.GetProperty(propInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                if(prop == null)
                {
                    //Debug.LogWarning($"component:{component.name} property:{propInfo.Name} is not found.");
                    continue;
                }
                var at = Attribute.GetCustomAttribute(prop, typeof(ObsoleteAttribute));
                if (at != null)
                {
                    //Debug.Log($"{prop.Name} is Obsolete.");
                    continue;
                }                
                if (IsSkipGetValue(component,prop.PropertyType,prop))
                {
                    continue;
                }
                try
                {
                    var o = prop.GetValue(component);
                    m_Properties[i].SetValue(o);
                }
                catch (System.Exception) { }                                    
            }

            for(var i = 0; i < m_FieldInfos.Length; i++)
            {
                var fieldInfo = m_FieldInfos[i];                
                var field = type.GetField(fieldInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if(field == null)
                {
                    //Debug.LogWarning($"component : {component.name} field: {fieldInfo.Name} is not found.");
                    continue;
                }                
                var o = field.GetValue(component);
                m_Fields[i].SetValue(o);                                    
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Caches.Contains(this))
            {
                Caches.Remove(this);
            }
        }

        void Init()
        {
            var component = (Component)m_object;
            var type = component.GetType();
            // プロパティの取得
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<SyncValueObject>();
            var propList = new List<SyncPropertyInfo>();
            foreach (var prop in props)
            {
                if (!prop.CanRead)
                {
                    continue;
                }
                if (!prop.CanWrite)
                {
                    continue;
                }
                // Obsoleteなフィールドは無視する
                var at = Attribute.GetCustomAttribute(prop, typeof(ObsoleteAttribute));
                if (at != null)
                {
                    //Debug.Log($"{prop.Name} is Obsolete.");
                    continue;
                }
                // UnityではObsoleteになったgetterがNotSupportedExceptionをthrowしている為、キャッチしてスルーする必要があるが、
                // TestRunnerがSystem.ExceptionでキャッチしているのでこちらもSystem.Exceptionでキャッチせざるを得ない
                object o = null;
                try
                {
                    if (!IsSkipGetValue(component, prop.PropertyType,prop))
                    {
                        o = prop.GetValue(component);
                    }
                }
                //catch(System.NotSupportedException)
                catch (System.Exception)
                {
                    continue;
                }
                var syncValueType = SyncValueObject.Allocater(prop.PropertyType, prop.PropertyType, o);
                if (syncValueType == null)
                {
                    continue;
                }
                list.Add(syncValueType);
                propList.Add(new SyncPropertyInfo(prop));
            }
            m_Properties = list.ToArray();
            m_PropertyInfos = propList.ToArray();


            // フィールド（変数)の取得
            list.Clear();
            var fiList = new List<SyncFieldInfo>();
            // Serializeアトリビュートが付いたフィールドも設定出来る必要がある為、NonPublicフラグも有効にする
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fi in fis)
            {
                if (fi.IsInitOnly)
                {
                    continue;
                }
                if (fi.IsLiteral)
                {
                    continue;
                }
                if (fi.IsStatic)
                {
                    continue;
                }                                
                var isNonPublic = (fi.IsAssembly || fi.IsFamily || fi.IsFamilyOrAssembly || fi.IsFamilyAndAssembly || fi.IsPrivate) ? true : false;
                var at = Attribute.GetCustomAttribute(fi, typeof(SerializeField));
                var IsSerializeField = at != null ? true : false;
                if (isNonPublic && !IsSerializeField)
                {
                    continue;
                }
                // Inspectorに非表示のフィールドは無視する
                at = Attribute.GetCustomAttribute(fi, typeof(HideInInspector));
                if(at != null)
                {
                    continue;
                }

                object o = null;
                try
                {
                    o = fi.GetValue(component);
                }
                catch (System.Exception)
                {
                    continue;
                }
                var syncValueType = SyncValueObject.Allocater(fi.FieldType, fi.FieldType, o);
                if (syncValueType == null)
                {
                    continue;
                }
                list.Add(syncValueType);
                fiList.Add(new SyncFieldInfo(fi));
            }
            m_Fields = list.ToArray();
            m_FieldInfos = fiList.ToArray();
        }
    }
}
