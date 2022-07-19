using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncType : Sync
    {
        public static TypeCode GetTypeCode(SyncType syncType)
        {
            switch (syncType.m_FullName)
            {
                case "System.Empty": return TypeCode.Empty;
                case "System.Object": return TypeCode.Object;
                case "System.DBNull": return TypeCode.DBNull;
                case "System.Boolean": return TypeCode.Boolean;
                case "System.Char": return TypeCode.Char;
                case "System.SByte": return TypeCode.SByte;
                case "System.Byte": return TypeCode.Byte;
                case "System.Int16": return TypeCode.Int16;
                case "System.UInt16": return TypeCode.UInt16;
                case "System.Int32": return TypeCode.Int32;
                case "System.UInt32": return TypeCode.UInt32;
                case "System.Int64": return TypeCode.Int64;
                case "System.UInt64": return TypeCode.UInt64;
                case "System.Single": return TypeCode.Single;
                case "System.Double": return TypeCode.Double;
                case "System.Decimal": return TypeCode.Decimal;
                case "System.DateTime": return TypeCode.DateTime;
                case "System.String": return TypeCode.String;
            }
            return TypeCode.Object;
        }


        public static System.Type GetType(SyncType type)
        {
#if !UNITY_EDITOR
            switch(type.FullName)
            {
                case "UnityEditor.Animations.AnimatorController":
                    {
                        if (type.IsArray)
                        {
                            return typeof(RuntimeAnimatorController[]);
                        }
                        else if (type.IsGenericType)
                        {
                            return typeof(List<RuntimeAnimatorController>);
                        }
                        return typeof(RuntimeAnimatorController);
                    }

                case "UnityEditor.Audio.AudioMixerController":
                    {
                        if (type.IsArray)
                        {
                            return typeof(UnityEngine.Audio.AudioMixer[]);
                        }
                        else if(type.IsGenericType)
                        {
                            return typeof(List<UnityEngine.Audio.AudioMixer>);
                        }
                        return typeof(UnityEngine.Audio.AudioMixer);
                    }

                case "UnityEditor.Audio.AudioMixerGroupController":
                    {
                        if (type.IsArray)
                        {
                            return typeof(UnityEngine.Audio.AudioMixerGroup[]);
                        }
                        else if (type.IsGenericType)
                        {
                            return typeof(List<UnityEngine.Audio.AudioMixerGroup>);
                        }
                            
                        return typeof(UnityEngine.Audio.AudioMixerGroup);
                    }
            }
#else
            switch(type.FullName)
            {
                case "UnityEngine.RuntimeAnimatorController":
                    if (type.IsArray)
                    {
                        return typeof(UnityEditor.Animations.AnimatorController[]);
                    }
                    else if (type.IsGenericType)
                    {
                        

                        return typeof(List<UnityEditor.Animations.AnimatorController>);
                    }
                    return typeof(UnityEditor.Animations.AnimatorController);

                case "UnityEngine.Audio.AudioMixer":
                    {
                        var tmp = System.Type.GetType("UnityEditor.Audio.AudioMixerController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");

                        if (type.IsArray)
                        {
                            return tmp.MakeArrayType();
                        }
                        else if (type.IsGenericType)
                        {
                            var generic = typeof(List<>);
                            Type[] args = {tmp};
                            return generic.MakeGenericType(args);                            
                        }
                        return tmp;
                    }
                case "UnityEngine.Audio.AudioMixerGroup":
                    {
                        var tmp = System.Type.GetType("UnityEditor.Audio.AudioMixerGroupController,UnityEditor.CoreModule, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");
                        if (type.IsArray)
                        {
                            return tmp.MakeArrayType();
                        }
                        else if (type.IsGenericType) 
                        {
                            var generic = typeof(List<>);
                            Type[] args = { tmp };
                            return generic.MakeGenericType(args);
                        }
                        return tmp;
                    }
            }
#endif
            // 型とAssemblyのフルネームから型を取得する
            var typeName = $"{type.FullName},{type.Assembly.FullName}";                                
            var t = System.Type.GetType(typeName);
            if(t == null)
            {
                // 列挙の場合は適当な列挙を仮に使用する
                if (type.IsEnum)
                {
                    if (type.IsArray)
                    {
                        return typeof(System.TypeCode[]);
                    }
                    else if (type.IsGenericType)
                    {
                        return typeof(List<TypeCode>);
                    }
                    return typeof(TypeCode);
                }
                // 列挙では無かった場合、Unityのビルドインクラスであれば随時対応
                Debug.LogError($"{typeName} is not supported. if this class is unity build-in,Prease reauest bug report.");
            }
            return t;
        }


        protected string m_FullName;
        protected bool m_IsArray;
        protected bool m_IsGenericType;
        protected bool m_IsEnum;
        protected MemberTypes m_MemberType;
        protected string m_Name;
        protected SyncAssembly m_Assembly;


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

        public string Name
        {
            get { return m_Name; }
        }


        public MemberTypes MemberTypes
        {
            get { return m_MemberType; }
        }


        public SyncAssembly Assembly
        {
            get { return m_Assembly; }
        }


        public SyncType()
        {
            m_Assembly = new SyncAssembly();
        }

        public SyncType(System.Type type)
        {
            if (type == null)
            {
                m_Assembly = new SyncAssembly();
            }
            else if (type != null)
            {
                m_FullName = type.FullName;
                m_IsArray = type.IsArray;
                m_IsEnum = type.IsEnum;
                m_IsGenericType = type.IsGenericType;
                m_MemberType = type.MemberType;
                m_Name = type.Name;
                m_Assembly = new SyncAssembly(type.Assembly);
            }
        }


        public override void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(m_FullName);
            binaryWriter.Write(m_IsArray);
            binaryWriter.Write(m_IsEnum);
            binaryWriter.Write(m_IsGenericType);
            binaryWriter.Write((int)m_MemberType);
            binaryWriter.Write(m_Name);
            m_Assembly.Serialize(binaryWriter);
        }


        public override void Deserialize(BinaryReader binaryReader)
        {
            m_FullName = binaryReader.ReadString();
            m_IsArray = binaryReader.ReadBoolean();
            m_IsEnum = binaryReader.ReadBoolean();
            m_IsGenericType= binaryReader.ReadBoolean();
            m_MemberType = (MemberTypes)binaryReader.ReadInt32();
            m_Name = binaryReader.ReadString();            
            m_Assembly.Deserialize(binaryReader);
        }
    }
}
