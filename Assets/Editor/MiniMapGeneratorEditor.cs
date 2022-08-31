using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(MiniMapGenerator))]
public class MiniMapGeneratorEditor : Editor
{
    private MiniMapGenerator t;

    private void OnEnable()
    {
        t = (MiniMapGenerator)target;


    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("NewPoint"))
        {
            t.SetPoint();
        }
        if(GUILayout.Button("RemovePoint"))
        {
            t.RemovePoint();
        }
    }
    
}
