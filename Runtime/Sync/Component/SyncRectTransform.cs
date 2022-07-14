using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UTJ.UnityPlayerSync.Runtime
{
    public class SyncRectTransform : SyncTransform
    {
        public SyncRectTransform(object obj) : base(obj)
        {            
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            var rectTransform = m_object as RectTransform;
            new SyncVector2(rectTransform.anchoredPosition).Serialize(binaryWriter);
            new SyncVector3(rectTransform.anchoredPosition3D).Serialize(binaryWriter);
            new SyncVector2(rectTransform.anchorMax).Serialize(binaryWriter);
            new SyncVector2(rectTransform.anchorMin).Serialize(binaryWriter);
            new SyncVector2(rectTransform.offsetMax).Serialize(binaryWriter);
            new SyncVector2(rectTransform.offsetMin).Serialize(binaryWriter);
            new SyncVector2(rectTransform.pivot).Serialize(binaryWriter);            
            new SyncVector2(rectTransform.sizeDelta).Serialize(binaryWriter);

            
        }

        public override void Deserialize(BinaryReader binaryReader)
        {            
            base.Deserialize(binaryReader);

            var rectTransform = m_object as RectTransform;

            var pos = rectTransform.localPosition;
            var rote = rectTransform.localRotation;
            var scale = rectTransform.localScale;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            var v2 = new SyncVector2(rectTransform.anchoredPosition);
            v2.Deserialize(binaryReader);
            rectTransform.anchoredPosition = (Vector2)v2.GetValue();

            var v3 = new SyncVector3(rectTransform.anchoredPosition3D);
            v3.Deserialize(binaryReader);
            rectTransform.anchoredPosition3D = (Vector3)v3.GetValue();

            v2 = new SyncVector2(rectTransform.anchorMax);
            v2.Deserialize(binaryReader);
            rectTransform.anchorMax = (Vector2)v2.GetValue();

            v2 = new SyncVector2(rectTransform.anchorMin);
            v2.Deserialize(binaryReader);
            rectTransform.anchorMin = (Vector2)v2.GetValue();

            v2 = new SyncVector2(rectTransform.offsetMax);
            v2.Deserialize(binaryReader);
            rectTransform.offsetMax= (Vector2)v2.GetValue();

            v2 = new SyncVector2(rectTransform.offsetMin);
            v2.Deserialize(binaryReader);
            rectTransform.offsetMin = (Vector2)v2.GetValue();

            v2 = new SyncVector2(rectTransform.pivot);
            v2.Deserialize(binaryReader);
            rectTransform.pivot = (Vector2)v2.GetValue();

            v2 = new SyncVector2(rectTransform.sizeDelta);
            v2.Deserialize(binaryReader);
            rectTransform.sizeDelta = (Vector2)v2.GetValue();


            rectTransform.localPosition = pos;
            rectTransform.localRotation = rote;
            rectTransform.localScale = scale;
        }
    }
}