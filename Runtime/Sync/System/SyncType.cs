using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public static class SyncTypeTree
    {
        public static Dictionary<int, SyncType> m_Instance;
        public static Dictionary<int, SyncType> Instances
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = new Dictionary<int, SyncType>();
                }
                return m_Instance;
            }
        }               

        public static void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Instances.Count);
            foreach(var instance in Instances)
            {
                binaryWriter.Write(instance.Key);
                instance.Value.Serialize(binaryWriter);
            }
        }

        public static void Deserialize(BinaryReader binaryReader)
        {
            var len = binaryReader.ReadInt32();
            for(var i = 0; i < len; i++)
            {
                var hash = binaryReader.ReadInt32();
                var syncType = new SyncType();
                syncType.Deserialize(binaryReader);
                Instances.Add(hash, syncType);
            }
        }

    }


    public class SyncType : Sync
    {        
                
        public static System.Type GetType(SyncType type)
        {
            if(type.m_Type != null)
            {
                return type.m_Type;
            }

#if !UNITY_EDITOR
            switch(type.FullName)
            {
                case "UnityEditor.Animations.AnimatorController":
                    {
                        if (type.IsArray)
                        {
                            type.m_Type =  typeof(RuntimeAnimatorController[]);
                        }
                        else if (type.IsGenericType)
                        {
                            type.m_Type = typeof(List<RuntimeAnimatorController>);
                        }else{
                            type.m_Type = typeof(RuntimeAnimatorController);
                        }
                        return type.m_Type;
                    }                    

                case "UnityEditor.Audio.AudioMixerController":
                    {
                        if (type.IsArray)
                        {
                            type.m_Type = typeof(UnityEngine.Audio.AudioMixer[]);
                        }
                        else if(type.IsGenericType)
                        {
                            type.m_Type = typeof(List<UnityEngine.Audio.AudioMixer>);
                        }
                        else{
                            type.m_Type = typeof(UnityEngine.Audio.AudioMixer);
                        }
                        return type.m_Type;
                    }
                    

                case "UnityEditor.Audio.AudioMixerGroupController":
                    {
                        if (type.IsArray)
                        {
                            type.m_Type = typeof(UnityEngine.Audio.AudioMixerGroup[]);
                        }
                        else if (type.IsGenericType)
                        {
                            type.m_Type = typeof(List<UnityEngine.Audio.AudioMixerGroup>);
                        }
                        else
                        {
                            type.m_Type = typeof(UnityEngine.Audio.AudioMixerGroup);
                        }
                        return type.m_Type;
                    }                   
            }
#else
            switch (type.FullName)
            {
                case "UnityEngine.RuntimeAnimatorController":
                    {
                        if (type.IsArray)
                        {
                            type.m_Type = typeof(UnityEditor.Animations.AnimatorController[]);
                        }
                        else if (type.IsGenericType)
                        {
                            type.m_Type = typeof(List<UnityEditor.Animations.AnimatorController>);
                        }
                        else
                        {
                            type.m_Type = typeof(UnityEditor.Animations.AnimatorController);
                        }
                        return type.m_Type;
                    }
                    

                case "UnityEngine.Audio.AudioMixer":
                    {
                        var tmp = System.Type.GetType("UnityEditor.Audio.AudioMixerController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");

                        if (type.IsArray)
                        {
                            type.m_Type = tmp.MakeArrayType();
                        }
                        else if (type.IsGenericType)
                        {
                            var generic = typeof(List<>);
                            Type[] args = { tmp };
                            type.m_Type = generic.MakeGenericType(args);
                        }
                        else
                        {
                            type.m_Type = tmp;
                        }
                        return type.m_Type;
                    }                    

                case "UnityEngine.Audio.AudioMixerGroup":
                    {
                        var tmp = System.Type.GetType("UnityEditor.Audio.AudioMixerGroupController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");
                        if (type.IsArray)
                        {
                            type.m_Type = tmp.MakeArrayType();
                        }
                        else if (type.IsGenericType)
                        {
                            var generic = typeof(List<>);
                            Type[] args = { tmp };
                            type.m_Type = generic.MakeGenericType(args);
                        }
                        else
                        {
                            type.m_Type = tmp;
                        }
                        return type.m_Type;
                    }                    
            }
            
#endif
            // 型とAssemblyのフルネームから型を取得する
            var typeName = $"{type.m_FullName},{type.m_Assembly}";                                
            var t = System.Type.GetType(typeName);
            if(t == null)
            {
                // 列挙の場合は適当な列挙を仮に使用する
                if (type.IsEnum)
                {
                    if (type.IsArray)
                    {
                        type.m_Type = typeof(System.TypeCode[]);
                    }
                    else if (type.IsGenericType)
                    {
                        type.m_Type = typeof(List<TypeCode>);
                    }
                    else
                    {
                        type.m_Type = typeof(TypeCode);
                    }
                    return type.m_Type;
                }
                // 列挙では無かった場合、Unityのビルドインクラスであれば随時対応
                Debug.LogError($"{typeName} is not supported. if this class is unity build-in,Prease reauest bug report.");
            }

            type.m_Type = t;

            return type.m_Type;
        }
        
        private System.Type m_Type;        
        protected string m_FullName;
        protected bool m_IsArray;
        protected bool m_IsGenericType;
        protected bool m_IsEnum;
        protected MemberTypes m_MemberType;        
        protected string m_Assembly;


        public string FullName
        {
            get { return m_FullName; }
        }


        public bool IsArray
        {
            get { return m_IsArray; }
        }

        public bool IsGenericType
        {
            get { return m_IsGenericType; }
        }

        public bool IsEnum
        {
            get { return m_IsEnum; }
        }
        
      
        public SyncType() : this(null) { }
       

        public SyncType(System.Type type)
        {
            m_Type = type;

        if (type != null)
            {
                m_FullName = type.FullName;
                m_IsArray = type.IsArray;
                m_IsEnum = type.IsEnum;
                m_IsGenericType = type.IsGenericType;
                m_MemberType = type.MemberType;                
                m_Assembly = type.Assembly.GetName().Name;                
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            try
            {
                binaryWriter.Write(m_FullName);
                binaryWriter.Write(m_IsArray);
                binaryWriter.Write(m_IsEnum);
                binaryWriter.Write(m_IsGenericType);
                binaryWriter.Write((int)m_MemberType);                
                binaryWriter.Write(m_Assembly);                
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
            }            
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            m_FullName = binaryReader.ReadString();
            m_IsArray = binaryReader.ReadBoolean();
            m_IsEnum = binaryReader.ReadBoolean();
            m_IsGenericType= binaryReader.ReadBoolean();
            m_MemberType = (MemberTypes)binaryReader.ReadInt32();
            m_Assembly = binaryReader.ReadString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyncType;
            if(other == null)
            {
                return false;
            }
            if(base.Equals(obj) == false)
            {
                return false;
            }
            if(m_FullName.Equals(other.FullName) == false)
            {
                return false;
            }
            if (m_IsArray.Equals(other.IsArray) == false)
            {
                return false;
            }
            if(m_IsGenericType.Equals(other.m_IsGenericType) == false)
            {
                return false;
            }
            if(m_IsEnum.Equals(other.m_IsEnum) == false)
            {
                return false;
            }
            if(m_MemberType.Equals(other.m_MemberType) == false)
            {
                return false;
            }
            if(m_Assembly.Equals(other.m_Assembly) == false)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {            
            var hash = base.GetHashCode();
            hash = (hash * 397) ^ m_FullName.GetHashCode();
            hash = (hash * 397) ^ m_IsArray.GetHashCode();
            hash = (hash * 397) ^ m_IsGenericType.GetHashCode();
            hash = (hash * 397) ^ m_IsEnum.GetHashCode();
            hash = (hash * 397) ^ m_MemberType.GetHashCode();
            hash = (hash * 397) ^ m_Assembly.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (m_IsArray)
            {
                return $"{m_FullName}[],{m_Assembly}";
            }
            else if (m_IsGenericType)
            {
                return $"List<{m_FullName}>,{m_Assembly}";
            }
            return $"{m_FullName},{m_Assembly}";
        }
    }
}
