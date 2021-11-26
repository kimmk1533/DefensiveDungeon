using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : Singleton<SkillUIManager>
{
	[SerializeField]
	protected SkillUI m_Skill01;
	[SerializeField]
	protected SkillUI m_Skill02;

	bool activebtn = false;

	protected DevilManager M_Devil => DevilManager.Instance;

	void Skill1()
	{
		activebtn = !M_Devil.UseSkill;
		M_Devil.UseSkill = activebtn;
		M_Devil.SkillNumber = (int)Devil.E_SkillNumber.Skill1;
	}
	void Skill2()
	{
		activebtn = !M_Devil.UseSkill;
		M_Devil.UseSkill = activebtn;
		M_Devil.SkillNumber = (int)Devil.E_SkillNumber.Skill2;
	}

	private void Start()
	{
		#region 스킬01
		m_Skill01.onClick.AddListener(Skill1);

		#endregion

		#region 스킬02
		if (M_Devil.Devil.GetBossType == E_Devil.HellLord)
			m_Skill02.SetOnOff(false);
		else
			m_Skill02.onClick.AddListener(Skill2);
		#endregion
	}
}
