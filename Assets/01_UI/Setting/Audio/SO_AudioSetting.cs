using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AudioSettingData
{
    public string m_Descript;
    public int m_Order;

    [Range(0.0f, 1.0f)]
    public float m_Volume;
    public bool m_Mute;

    public int CompareTo(AudioSettingData other)
    {
        if (m_Order > other.m_Order)
            return 1;
        else if (m_Order == other.m_Order)
            return 0;
        else
            return -1;
    }
}

[System.Serializable, CreateAssetMenu(fileName = "New Audio", menuName = "Scriptable Object/Audio")]
public class SO_AudioSetting : ScriptableObject
{
    public AudioSettingData m_Data;
}
