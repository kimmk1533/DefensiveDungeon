using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipRightSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] Icon_TableExcelLoader m_iconLoader;
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
		m_skillTooltip.Activate();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_skillTooltip.DeActivate();
	}

	public void SetUI(SkillCondition_TableExcel data)
	{
		m_skill_image.sprite = m_iconLoader.GetIcon(data.Skill_icon);
		m_skill_text.text = data.Name_KR;

		m_skillTooltip.SetUI(data);
	}
}
