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
    /// Component�𓯊�������ׂ̃N���X
    /// </summary>
    public class SyncComponent : SyncUnityEngineObject
    {
        enum DummyEnum
        {
            Dummy,
        };


        /// <summary>
        /// �������ꂽSyncComponent�̃L���b�V��
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
        /// Component���L�[�ɂ���SyncComponent����������
        /// </summary>
        /// <param name="component">�L�[�Ƃ���Component</param>
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


        /// <summary>
        /// instanceID���L�[�ɂ���SyncComponent����������
        /// </summary>
        /// <param name="instanceID">�L�[�Ƃ���instanceID</param>
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
        /// �Ή�����Component���擾����
        /// </summary>
        /// <returns></returns>
        public Component GetComponent()
        {
            return (Component)m_object;
        }
        

        /// <summary>
        /// �R���X�g���N�^�[
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

        ~SyncComponent()
        {
            Caches.Remove(this);
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
                    // ����ǂݍ��߂Ȃ�Build-In�^�͗񋓌^�����̔��Ȃ̂ŁA�_�~�[�̗񋓌^�ŋ�ǂ݂��s��
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
                    // ����ǂݍ��߂Ȃ�Build-In�^�͗񋓌^�����̔��Ȃ̂ŁA�_�~�[�̗񋓌^�ŋ�ǂ݂��s��
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
                // �f�V���A���C�Y�o���Ȃ������v���p�e�B�̓X�L�b�v
                if(m_Properties[i] == null)
                {
                    continue;
                }

                var prop = type.GetProperty(m_PropertyInfos[i].Name);
                if (!prop.CanWrite)
                {
                    continue;
                }
                if (IsSkipGetValue(prop))
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
                // �f�V���A���C�Y�o�����������t�B�[���h�̓X�L�b�v
                if(m_Fields[i] == null)
                {
                    continue;
                }
                var fi = type.GetField(m_FieldInfos[i].Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
                        fi.SetValue(component, o);
                    }
                }
            }
        }


        /// <summary>
        /// �g���Ɩ��̂���v���p�e�B�𔻒肷��
        /// </summary>
        /// <param name="info">�v���p�e�B��</param>
        /// <returns>true:��肠��</returns>
        bool IsSkipGetValue(PropertyInfo info)
        {
#if UNITY_EDITOR
            // mesh/material/materials��Editor���[�h�ł̓A�N�Z�X�o���Ȃ��ׁASkip
            if (info.DeclaringType == typeof(UnityEngine.MeshFilter))
            {
                if (info.PropertyType == typeof(UnityEngine.Mesh) && info.Name == "mesh")
                {
                    return true;
                }
            }
            if(info.DeclaringType == typeof(UnityEngine.Renderer))
            {
                if(info.PropertyType == typeof(UnityEngine.Material) && (info.Name == "material"))
                {
                    return true;
                }
                if (info.PropertyType == typeof(UnityEngine.Material[]) && (info.Name == "materials"))
                {
                    return true;
                }
            }
#endif
            // Runtime<->Editor�Ԃ�pixelRect��G��ƐF�X�s����N����̂ŐG��Ȃ�
            if(info.DeclaringType == typeof(Camera))
            {
                if(info.Name == "pixelRect")
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
                    Debug.LogError($"{propInfo.Name} is not found.");
                    continue;
                }
                var at = Attribute.GetCustomAttribute(prop, typeof(ObsoleteAttribute));
                if (at != null)
                {
                    //Debug.Log($"{prop.Name} is Obsolete.");
                    continue;
                }
                if (IsSkipGetValue(prop))
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
                var field = type.GetField(fieldInfo.Name);
                if(field == null)
                {
                    Debug.LogError($"{fieldInfo.Name} is not found.");
                    continue;
                }                
                var o = field.GetValue(component);
                m_Fields[i].SetValue(o);                                    
            }
        }



        void Init()
        {
            var component = (Component)m_object;
            var type = component.GetType();
            // �v���p�e�B�̎擾
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<SyncValueObject>();
            var propList = new List<SyncPropertyInfo>();
            foreach (var prop in props)
            {
                if (!prop.CanRead)
                {
                    continue;
                }
                // Obsolete�ȃt�B�[���h�͖�������
                var at = Attribute.GetCustomAttribute(prop, typeof(ObsoleteAttribute));
                if (at != null)
                {
                    //Debug.Log($"{prop.Name} is Obsolete.");
                    continue;
                }
                // Unity�ł�Obsolete�ɂȂ���getter��NotSupportedException��throw���Ă���ׁA�L���b�`���ăX���[����K�v�����邪�A
                // TestRunner��System.Exception�ŃL���b�`���Ă���̂ł������System.Exception�ŃL���b�`������𓾂Ȃ�
                object o = null;
                try
                {
                    if (!IsSkipGetValue(prop))
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


            // �t�B�[���h�i�ϐ�)�̎擾
            list.Clear();
            var fiList = new List<SyncFieldInfo>();
            // Serialize�A�g���r���[�g���t�����t�B�[���h���ݒ�o����K�v������ׁANonPublic�t���O���L���ɂ���
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
