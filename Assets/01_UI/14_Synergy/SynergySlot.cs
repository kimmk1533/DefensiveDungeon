using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct SynergySlotInfo
{
    public int index;

    public bool isActivated;

    public string name;
    public string synergy_text;
    public string synergy_ability;

    public int sprite_code;
}

public class SynergySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite_TableExcelLoader m_sprite_loader;
    [SerializeField] SynergySlotInfo m_info;
    [SerializeField] Image m_synergy_image;

    [SerializeField] SynergyTooltip m_tooltip;

    

    // info 가 업데이트 됫을 경우
    public void SetInfo(SynergySlotInfo info /*-> excel data*/)
    {        
        m_info = info;

        OnInfoChanged();
    }

    private void OnInfoChanged()
    {
        Sprite sprite = m_sprite_loader.GetSprite(m_info.sprite_code);
        m_synergy_image.sprite = sprite;
        if (m_info.isActivated)
        {            
            m_synergy_image.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_synergy_image.color = new Color(0.3f, 0.3f, 0.3f);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        m_tooltip.SetPoisition(eventData.position);
        m_tooltip.SetInfo(
            m_info.name,
            m_info.synergy_text,
            m_info.synergy_ability,
            m_info.sprite_code);
        m_tooltip.Activate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_tooltip.DeActivate();
    }
}
