using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellLord : Devil
{
	bool useskill = false;

	#region 내부 함수
	protected override void DoSkill01(DevilSkillArg arg)
	{
		StartCoroutine(SK001(arg));
	}
	protected override void DoSkill02(DevilSkillArg arg)
	{
		StartCoroutine(SK002(arg));
	}
	//패시브 스킬 적용.
	override public void CallAttack()
	{
		// 내부 데이터 정리
		m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;

		if ((E_TargetType)m_DevilInfo.Condition_Default.Target_type != E_TargetType.TileTarget &&
			null == m_Target_Default)
			return;

		// 기본 스킬 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
		SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

		// 기본 대미지 설정
		statData.Dmg_Fix += m_DevilInfo_Excel.Atk;

		#region 크리티컬
		float dmg_percent = m_DevilInfo.m_Skill02.m_Dmg_Percent;
		// 크리티컬 확률
		float CritRate = m_DevilInfo_Excel.Crit_rate;
		// 크리티컬 배율
		float CritDmg = m_DevilInfo_Excel.Crit_Dmg;

		// 크리티컬 대미지 설정
		float CritRand = Random.Range(0.00001f, 1f);
		bool CritApply = CritRand <= CritRate;
		if (CritApply)
		{
			dmg_percent *= CritDmg;
		}
		m_DevilInfo.m_Skill02.m_total_Dmg = m_DevilInfo.m_Skill02.m_Dmg_Fix * dmg_percent;
		OnSkill02(GetDevilSkillArg(Devil.E_SkillNumber.Skill2));
		#endregion

		// 기본 스킬 투사체 생성
		int DefaultSkillCode = conditionData.projectile_prefab;

		void Attack(Enemy target)
		{
			Skill skill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					skill.transform.position = m_DevilInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					skill.transform.position = target.HitPivot.position;
					break;
			}

			skill.enabled = true;
			skill.gameObject.SetActive(true);
			S_Critical critical=new S_Critical(CritRate,CritDmg);
			// 기본 스킬 데이터 설정
			skill.InitializeSkill(target, conditionData, statData,critical);
		}

		if ((E_TargetType)m_DevilInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList();

			for (int i = 0; i < EnemyList.Count; ++i)
			{
				Attack(EnemyList[i]);
			}
		}
		else
		{
			Attack(m_Target_Default);
		}

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_DevilInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}
	}
	protected IEnumerator SK001(DevilSkillArg arg)
	{
		float total_dmg = 0;
		float size = m_DevilInfo.m_Size;
		int mask = 1 << LayerMask.NameToLayer("Enemy");
		Collider[] enemyhit = Physics.OverlapSphere(arg.mousepos, size, mask);

		for (int i = 0; i < enemyhit.Length; ++i)
		{
			#region 크리티컬
			float dmg_percent = m_DevilInfo.m_Skill01.m_Dmg_Percent;
			// 크리티컬 확률
			float CritRate = m_DevilInfo_Excel.Crit_rate;
			// 크리티컬 배율
			float CritDmg = m_DevilInfo_Excel.Crit_Dmg;

			// 크리티컬 대미지 설정
			float CritRand = Random.Range(0.00001f, 1f);
			bool CritApply = CritRand <= CritRate;
			if (CritApply)
			{
				dmg_percent *= CritDmg;
			}

			//최종 데미지
			arg.skillData.m_total_Dmg = m_DevilInfo.m_Skill01.m_Dmg_Fix * dmg_percent;

			#endregion

			enemyhit[i].transform.GetComponent<Enemy>().On_DaMage(arg.skillData.m_total_Dmg,CritApply);
			total_dmg += arg.skillData.m_total_Dmg;
		}


		float SelfHeal = total_dmg * 0.2f;
		if (m_DevilInfo.m_HP <= m_DevilInfo.m_halfHP)
		{
			m_DevilInfo.m_HP += SelfHeal;
		}
		//임시값
		yield break;
	}
	protected IEnumerator SK002(DevilSkillArg arg)
	{
		float dmg_percent = m_DevilInfo.m_Skill02.m_Dmg_Percent;
		//최종 데미지
		m_DevilInfo.m_Skill02.m_total_Dmg = m_DevilInfo.m_Skill02.m_Dmg_Fix * dmg_percent;
		float SelfHeal = m_DevilInfo.m_Skill02.m_total_Dmg;
		if (m_DevilInfo.m_HP <= m_DevilInfo.m_halfHP)
		{
			m_DevilInfo.m_HP += SelfHeal;
		}
		//임시값
		yield break;
	}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		InitializeDevil(E_Devil.HellLord);
	}
	#endregion
}
