using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : Singleton<SkillUIManager>
{
	[SerializeField]
	protected Icon_TableExcelLoader icon_table_loader;

	[SerializeField]
	protected SkillUI m_Skill01;
	[SerializeField]
	protected SkillUI m_Skill02;

	bool activebtn = false;

	protected DevilManager M_Devil => DevilManager.Instance;
	protected OptionManager M_Option => OptionManager.Instance;

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

		m_Skill01.skillCountText = M_Devil.Skill01_ChargeCount.ToString();
		//m_Skill01.skillKeyText = M_Option.GetKeyCode(KeyOptionType.Skill01);
		m_Skill01.SetSkillImage(icon_table_loader.GetIcon(M_Devil.Skill01_Icon));
		#endregion

		#region 스킬02
		if (M_Devil.Devil.GetBossType == E_Devil.HellLord)
			m_Skill02.SetOnOff(false);
		else
		{
			m_Skill02.onClick.AddListener(Skill2);

			m_Skill02.skillCountText = M_Devil.Skill02_ChargeCount.ToString();
			//m_Skill02.skillKeyText = M_Option.GetKeyCode(KeyOptionType.Skill02);
			m_Skill02.SetSkillImage(icon_table_loader.GetIcon(M_Devil.Skill02_Icon));
		}
		#endregion

		M_Devil.OnSkillCountChangedEvent += () =>
		{
			m_Skill01.skillCountText = M_Devil.Skill01_ChargeCount.ToString();
			m_Skill02.skillCountText = M_Devil.Skill02_ChargeCount.ToString();
		};
		M_Devil.OnUseSkillEvent += () =>
		{
			m_Skill01.skillCountText = M_Devil.Skill01_ChargeCount.ToString();
			m_Skill02.skillCountText = M_Devil.Skill02_ChargeCount.ToString();
		};
	}

	private void Update()
	{
		if (M_Devil.Skill01_ChargeCount == M_Devil.Skill01_MaxChargeCount)
			m_Skill01.skillFillAmount = 1f;
		else
		{
			float skill01_Timer = 1f - M_Devil.Skill01_CoolTimeTimer / M_Devil.Skill01_CoolTime;
			m_Skill01.skillFillAmount = skill01_Timer < 0f ? 0f : skill01_Timer;
		}

		if (M_Devil.Skill02_ChargeCount == M_Devil.Skill02_MaxChargeCount)
			m_Skill02.skillFillAmount = 1f;
		else
		{
			float skill02_Timer = 1f - M_Devil.Skill02_CoolTimeTimer / M_Devil.Skill02_CoolTime;
			m_Skill02.skillFillAmount = skill02_Timer < 0f ? 0f : skill02_Timer;
		}
	}
}
