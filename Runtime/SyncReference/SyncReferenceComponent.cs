using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.UnityPlayerSync.Runtime
{

    public class SyncReferenceComponent : SyncValueUnityEngineObject
    {
        public SyncReferenceComponent(object obj,System.Type type) : base(obj, type)
        {            
        }

        
        public override object GetValue()
        {
            if (m_Values == null)
            {
                return null;
            }
            for (var i = 0; i < m_Values.Length; i++)
            {
                // HierarchyからObjectを検索する
                m_Values[i] = SyncGameObject.FintObject(m_InstanceIDs[i]);
            }


            return base.GetValue();

        }
    }
}
