using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserInfoManagerDebug))]
public class UserInfoManagerDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /**************** level *****************/
        GUILayout.Space(10);
        if(GUILayout.Button("Reset Level"))
        {
            UserInfoManager.Instance.ResetLevel();
        }
        if(GUILayout.Button("Add Level"))
        {
            UserInfoManager.Instance.AddLevel(1);
        }


        /**************** gold *****************/
        GUILayout.Space(10);
        if (GUILayout.Button("Reset Gold"))
        {
            UserInfoManager.Instance.ResetGold();
        }
        if (GUILayout.Button("Add 10 Gold"))
        {
            UserInfoManager.Instance.AddGold(10);
        }
        if (GUILayout.Button("Add 100 Gold"))
        {
            UserInfoManager.Instance.AddGold(100);
        }
        if (GUILayout.Button("Add 1000 Gold"))
        {
            UserInfoManager.Instance.AddGold(1000);
        }
    }
}
