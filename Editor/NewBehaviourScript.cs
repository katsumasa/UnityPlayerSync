using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameObject))]
public class NewBehaviourScript : Editor
{


    public override void OnInspectorGUI()
    {
        GameObject gameObject = target as GameObject;


        GUILayout.Button("SYNC");
        base.OnInspectorGUI();
        
    }
}


[CustomEditor(typeof(Transform))]
public class NewBehaviourScript2 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}