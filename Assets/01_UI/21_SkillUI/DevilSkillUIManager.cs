using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilSkillUIManager : Singleton<DevilSkillUIManager>
{
	[SerializeField]
	protected Icon_TableExcelLoader icon_table_loader;

	[SerializeField]
	protected DevilSkillUI m_Skill01;
	[SerializeField]
	protected DevilSkillUI m_Skill02;

	KeyCode m_Skill01Key;
	KeyCode m_Skill02Key;

	protected DevilManager M_Devil => DevilManager.Instance;
	protected OptionManager M_Option => OptionManager.Instance;

	public UnityEngine.UI.Button.ButtonClickedEvent OnSkill01ButtonClickedEvent
	{
		get => m_Skill01.onClick;
	}
	public UnityEngine.UI.Button.ButtonClickedEvent OnSkill02ButtonClickedEvent
	{
		get => m_Skill02.onClick;
	}

	private void Start()
	{

		#region 스킬01설정
		m_Skill01Key = M_Option.GetKeyCode(KeyOptionType.Skill1);

		m_Skill01.SetSkillImage(icon_table_loader.GetIcon(M_Devil.Skill01_Icon));
		m_Skill01.skillCountText = M_Devil.Skill01_ChargeCount.ToString();
		m_Skill01.skillKeyText = m_Skill01Key.ToString();
		#endregion

		#region 스킬02 설정
		m_Skill02Key = M_Option.GetKeyCode(KeyOptionType.Skill2);

		m_Skill02.SetSkillImage(icon_table_loader.GetIcon(M_Devil.Skill02_Icon));
		if (M_Devil.Devil.GetBossType == E_Devil.HellLord)
		{
			m_Skill02.SetOnOff(false);
		}
		else
		{
			m_Skill02.skillCountText = M_Devil.Skill02_ChargeCount.ToString();
			m_Skill02.skillKeyText = m_Skill02Key.ToString();
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
		#region 스킬 아이콘 업데이트
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
		#endregion

		#region 스킬 사용
		if (Input.GetKeyDown(m_Skill01Key))
		{
			m_Skill01.onClick?.Invoke();
		}
		if (Input.GetKeyDown(m_Skill02Key))
		{
			m_Skill02.onClick?.Invoke();
		}
		#endregion
	}
}
