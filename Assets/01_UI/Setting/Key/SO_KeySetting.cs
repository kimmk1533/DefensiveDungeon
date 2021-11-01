using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct KeySettingData
{
    public string m_Descript;
    public int m_Order;

    // E_tempKeyInput  이게 무슨 키인지 알아야함
    public KeyCode m_First;
    public KeyCode m_Second;

    public int CompareTo(KeySettingData other)
    {
        if (m_Order > other.m_Order)
            return 1;
        else if (m_Order == other.m_Order)
            return 0;
        else
            return -1;
    }
}

[System.Serializable, CreateAssetMenu(fileName = "New Key", menuName = "Scriptable Object/Key")]
public class SO_KeySetting : ScriptableObject
{
    public KeySettingData m_Data;
}
