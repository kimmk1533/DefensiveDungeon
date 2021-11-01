using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DevilToolTipSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SkillCondition_TableExcel m_data;
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Image m_skill_image;
    [SerializeField] TMPro.TextMeshProUGUI m_skill_text;

    [Space(10)]
    [SerializeField] SkillToolTip m_skillTooltip;

    private void Start()
    {
        m_skillTooltip.DeActivate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_skillTooltip.SetUI(m_data);
        m_skillTooltip.Activate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_skillTooltip.SetUI(m_data);
        m_skillTooltip.DeActivate();
    }

    public void SetUI(SkillCondition_TableExcel data)
    {
        m_data = data;
        m_skill_image.sprite = m_spriteLoader.GetSprite(data.Skill_icon);
        m_skill_text.text = data.Name_KR;        
    }
}
