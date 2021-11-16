using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfoSlotController : MonoBehaviour
{
	[SerializeField] Icon_TableExcelLoader m_icon_loader;
	[SerializeField] List<SkillInfoSlot> m_slots;

	private void Awake()
	{
		var slots = this.transform.GetComponentsInChildren<SkillInfoSlot>(true);
		foreach (var item in slots)
		{
			m_slots.Add(item);
		}
	}

	public void SetInfos(List<SkillCondition_TableExcel> data)
	{
		for (int i = 0; i < m_slots.Count; i++)
		{
			// TODO : skill icon sprite 불러오기

			m_slots[i].Set(
				m_icon_loader.GetIcon(data[i].Skill_icon),
				data[i].Skill_text);
		}
	}
}
