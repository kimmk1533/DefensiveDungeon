using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellLord : Devil
{
	#region 내부 함수
	//패시브 스킬 적용.
	public override void CallAttack()
	{
		// 내부 데이터 정리
		m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;

		if ((E_TargetType)m_DevilInfo.Condition_Default.Target_type != E_TargetType.TileTarget &&
			IsTargetDead_Default)
			return;

		// 기본 스킬 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
		SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

		// 기본 대미지 설정
		statData.Dmg_Fix += m_DevilInfo_Excel.Atk;

		#region 크리티컬
		float dmg_percent = m_DevilInfo.Stat_Default.Dmg_Percent;
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
		#endregion

		float damage = m_DevilInfo.m_Skill02.m_Dmg_Fix * dmg_percent;

		// 스킬02 (패시브)
		if (m_DevilInfo.m_HP <= m_DevilInfo.m_halfHP)
		{
			float SelfHeal = (damage - m_Target_Default.Get_EnemyDef) * 0.3f;

			if (m_DevilInfo.m_HP + SelfHeal > m_DevilInfo.m_originalHP)
				SelfHeal = m_DevilInfo.m_originalHP - m_DevilInfo.m_HP;

			m_DevilInfo.m_HP += SelfHeal;
		}

		// 기본 스킬 투사체 생성
		int DefaultSkillCode = conditionData.projectile_prefab;

		void Attack(Enemy target)
		{
			Skill skill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_self:
					skill.transform.position = m_DevilInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					skill.transform.position = target.HitPivot.position;
					break;
			}

			skill.enabled = true;
			skill.gameObject.SetActive(true);
			S_Critical critical = new S_Critical(CritRate, CritDmg);
			// 기본 스킬 데이터 설정
			skill.InitializeSkill(
				target,
				conditionData,
				statData,
				critical,
				null
				);
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
	public override void CallSkill01()
	{
		float total_dmg = 0;
		float size = Skill01.m_StatData.Size;
		int mask = 1 << LayerMask.NameToLayer("Enemy");
		Collider[] enemyhit = Physics.OverlapSphere(m_DevilInfo.m_Skill01.m_MousePos, size, mask);

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
			#endregion

			//최종 데미지
			float damage = m_DevilInfo.m_Skill01.m_Dmg_Fix * dmg_percent;

			enemyhit[i].transform.GetComponent<Enemy>().On_Damage(
				damage,
				CritApply
				);

			total_dmg += damage;
		}

		if (m_DevilInfo.m_HP <= m_DevilInfo.m_halfHP)
		{
			float SelfHeal = total_dmg * 0.05f;

			if (m_DevilInfo.m_HP + SelfHeal > m_DevilInfo.m_originalHP)
				SelfHeal = m_DevilInfo.m_originalHP - m_DevilInfo.m_HP;

			m_DevilInfo.m_HP += SelfHeal;
		}

		m_DevilInfo.RotateSpeed = 5f;
	}
	public override void CallSkill02()
	{
		return;
	}
	//protected IEnumerator SK002()
	//{
	//	float dmg_percent = m_DevilInfo.m_Skill02.m_Dmg_Percent;
	//	//최종 데미지
	//	m_DevilInfo.m_Skill02.m_Total_Dmg = m_DevilInfo.m_Skill02.m_Dmg_Fix * dmg_percent;

	//	if (m_DevilInfo.m_HP <= m_DevilInfo.m_halfHP)
	//	{
	//		float SelfHeal = m_DevilInfo.m_Skill02.m_Total_Dmg;
	//		m_DevilInfo.m_HP += SelfHeal;
	//	}
	//	//임시값
	//	yield break;
	//}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		InitializeDevil(E_Devil.HellLord);
	}
	#endregion
}
