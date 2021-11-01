using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipLeftSynergy : MonoBehaviour
{
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Image m_synergy_image;
    [SerializeField] TMPro.TextMeshProUGUI m_synergy_name;

    public void SetUI(int sprite_code,string name)
    {
        m_synergy_image.sprite = m_spriteLoader.GetSprite(sprite_code);
        m_synergy_name.text = name;
    }
}
