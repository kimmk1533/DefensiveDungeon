using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneStartEventReciever))]
public class StartEventRecieverDebugEditor : Editor
{
    SceneStartEventReciever com;

    private void OnEnable()
    {
        com = (SceneStartEventReciever)target;
    }

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Activate Event"))
        {
            com.__Start();
        }

        GUILayout.Space(30);
        base.OnInspectorGUI();
    }
}
