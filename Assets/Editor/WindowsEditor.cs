using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Windows))]
public class WindowsEditor : Editor
{
    private Windows win;
    private void OnEnable()
    {
        win = (Windows)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Show"))
        {
            win.Show();
        }
        if (GUILayout.Button("Hide"))
        {
            win.Hide();
        }

    }
}
