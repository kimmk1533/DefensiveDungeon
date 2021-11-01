using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoSlot : MonoBehaviour
{
    [SerializeField] Image m_icon;
    [SerializeField] TMPro.TextMeshProUGUI m_textpro;

    public void Set(Sprite sprite, string text)
    {
        m_icon.sprite = sprite;
        m_textpro.text = text;
    }
}
