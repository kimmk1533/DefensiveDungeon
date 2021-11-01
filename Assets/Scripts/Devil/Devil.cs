using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Devil : MonoBehaviour
{
	// 타겟
	public Enemy m_Target;

	// 마왕 정보(엑셀)
	protected Tower_TableExcel m_DevilInfo_Excel;
	// 마왕 정보
	protected S_DevilData m_DevilInfo;

	protected delegate void DevilSkillHandler(DevilSkillArg arg);
	protected event DevilSkillHandler Skill01Event;
	protected event DevilSkillHandler Skill02Event;

	public delegate void DevilUpdateHPHandler(float max, float current);
	public event DevilUpdateHPHandler UpdateHPEvent;

	public delegate void DevilDeathHandler(GameEndData data);
	public event DevilDeathHandler DeathEvent;

	#region 내부 컴포넌트
	[SerializeField, ReadOnly]
	protected DevilAnimator m_DevilAnimator;

	[SerializeField]
	protected AttackRange m_AttackRange_Default;
	#endregion

	#region 내부 프로퍼티
	// 마왕 매니져
	protected DevilManager M_Devil => DevilManager.Instance;
	// 타워 매니져
	protected TowerManager M_Tower => TowerManager.Instance;
	// 스킬 매니져
	protected SkillManager M_Skill => SkillManager.Instance;
	// 적 매니져
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	// 이펙트 매니져
	protected EffectManager M_Effect => EffectManager.Instance;
	// 버프 매니져
	protected BuffManager M_Buff => BuffManager.Instance;

	// 타워 회전 속도
	protected float RotateSpeed => m_DevilInfo.RotateSpeed * Time.deltaTime;
	// 타겟까지의 거리
	protected float DistanceToTarget => Vector3.Distance(transform.position, m_Target.transform.position);
	#endregion

	#region 외부 프로퍼티
	public float MaxHP => m_DevilInfo_Excel.HP;
	public float HP => m_DevilInfo.m_HP;
	public bool IsDie => m_DevilInfo.isDie;

	public Tower_TableExcel ExcelData => m_DevilInfo_Excel;
	public Transform HitPivot => m_DevilInfo.HitPivot;
	#endregion

	#region 유니티 콜백 함수
	protected void Update()
	{
		if (IsDie)
			return;

		RotateToTarget();
		UpdateTarget();
		AttackTarget();
		ReduceSkillCooldown();
	}
	#endregion

	#region 내부 함수
	// 마왕 초기화
	protected void InitializeDevil(E_Devil no)
	{
		#region 엑셀 데이터 정리
		m_DevilInfo_Excel = M_Devil.GetData(no);
		#endregion

		#region 내부 데이터 정리
		m_DevilInfo.RotateSpeed = 5f;
		m_DevilInfo.ShouldFindTarget = true;

		// 공격 피벗
		// m_DevilInfo.AttackPivot ??= transform.GetChild("AttackPivot");
		if (null == m_DevilInfo.AttackPivot)
		{
			m_DevilInfo.AttackPivot = transform.GetChild("AttackPivot");
		}
		// 피격 피벗
		// m_DevilInfo.HitPivot ??= transform.GetChild("HitPivot");
		if (null == m_DevilInfo.HitPivot)
		{
			m_DevilInfo.HitPivot = transform.GetChild("HitPivot");
		}

		// 기본 스킬 데이터
		m_DevilInfo.Condition_Default = M_Skill.GetConditionData(m_DevilInfo_Excel.Atk_Code);
		m_DevilInfo.Stat_Default = M_Skill.GetStatData(m_DevilInfo_Excel.Atk_Code);
		// 기본 스킬
		m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
		m_DevilInfo.AttackTimer_Default = m_DevilInfo.Stat_Default.CoolTime;

		// 현재 체력
		m_DevilInfo.m_HP = m_DevilInfo_Excel.HP;
		// 현재 방어력
		m_DevilInfo.m_Def = m_DevilInfo_Excel.Def;
		#endregion

		#region 내부 컴포넌트
		if (null == m_DevilAnimator)
		{
			m_DevilAnimator = GetComponentInChildren<DevilAnimator>(true);
			m_DevilAnimator.SetDevil(this);
		}

		if (null == m_AttackRange_Default)
		{
			m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
			m_AttackRange_Default.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
		}
		m_AttackRange_Default.Range = m_DevilInfo.Stat_Default.Range;
		#endregion

		#region 마왕 스킬 정리
		Skill01Event += DoSkill01;
		Skill02Event += DoSkill02;
		#endregion
	}
	// 마왕 회전
	protected void RotateToTarget()
	{
		// 회전할 방향
		Vector3 lookingDir;

		// 타겟이 없으면
		if (null == m_Target)
		{
			// 초기 방향으로 방향 설정
			lookingDir = m_DevilInfo.LookingDir;
		}
		// 타겟이 있으면
		else
		{
			// 타겟 방향으로 방향 설정
			lookingDir = m_Target.transform.position - transform.position;
		}

		// 회전
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookingDir), RotateSpeed);
	}
	// 타겟 업데이트
	protected void UpdateTarget()
	{
		// 타겟 변경 기준에 따라
		switch ((E_TargetType)m_DevilInfo.Condition_Default.Target_type)
		{
			case E_TargetType.CloseTarget:
				if (m_DevilInfo.ShouldFindTarget)
				{
					m_Target = m_AttackRange_Default.GetNearTarget();
					m_DevilInfo.ShouldFindTarget = false;
				}
				break;
			case E_TargetType.RandTarget:
				if (m_DevilInfo.ShouldFindTarget)
				{
					m_Target = m_AttackRange_Default.GetRandomTarget();
					m_DevilInfo.ShouldFindTarget = false;
				}
				break;
			// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
			case E_TargetType.FixTarget:
				if (null == m_Target || // 예외처리
					DistanceToTarget > m_DevilInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
				{
					m_Target = m_AttackRange_Default.GetNearTarget();
				}
				break;
			case E_TargetType.TileTarget:
				if (m_DevilInfo.ShouldFindTarget)
				{
					m_Target = m_AttackRange_Default.GetNearTarget();
					m_DevilInfo.ShouldFindTarget = false;
				}
				break;
		}
	}
	// 마왕 공격
	protected void AttackTarget()
	{
		#region 기본 스킬
		// 기본 스킬 타이머
		if (m_DevilInfo.AttackTimer_Default < m_DevilInfo.AttackSpeed_Default)
		{
			m_DevilInfo.AttackTimer_Default += Time.deltaTime;
		}
		// 기본 스킬 공격
		else if (null != m_Target)
		{
			// 내부 데이터 정리
			m_DevilInfo.AttackTimer_Default -= m_DevilInfo.AttackSpeed_Default;

			Attack();
		}
		#endregion
	}
	// 마왕 스킬 쿨타임 감소
	protected void ReduceSkillCooldown()
	{
		ReduceSkill01Cooldown(Time.deltaTime);
		ReduceSkill02Cooldown(Time.deltaTime);
	}

	protected void Attack()
	{
		m_DevilAnimator.SetTrigger("Attack");
	}
	protected void Skill01()
	{
		m_DevilAnimator.SetTrigger("Skill01");
	}
	protected void Skill02()
	{
		m_DevilAnimator.SetTrigger("Skill02");
	}
	protected void Die()
	{
		m_DevilAnimator.SetTrigger("Die");
	}

	protected abstract void DoSkill01(DevilSkillArg arg);
	protected abstract void DoSkill02(DevilSkillArg arg);
	#endregion

	#region 외부 함수
	// 스킬01 쿨타임 감소
	public void ReduceSkill01Cooldown(float time)
	{
		if (m_DevilInfo.m_Skill01.m_CurrentCharge < m_DevilInfo.m_Skill01.m_MaxCharge)
		{
			m_DevilInfo.m_Skill01.m_CooltimeTimer -= time;

			if (m_DevilInfo.m_Skill01.m_CooltimeTimer <= 0f)
			{
				m_DevilInfo.m_Skill01.m_CooltimeTimer += m_DevilInfo.m_Skill01.m_Cooltime;

				++m_DevilInfo.m_Skill01.m_CurrentCharge;
			}
		}
	}
	// 스킬02 쿨타임 감소
	public void ReduceSkill02Cooldown(float time)
	{
		if (m_DevilInfo.m_Skill02.m_CurrentCharge < m_DevilInfo.m_Skill02.m_MaxCharge)
		{
			m_DevilInfo.m_Skill02.m_CooltimeTimer -= time;

			if (m_DevilInfo.m_Skill02.m_CooltimeTimer <= 0f)
			{
				m_DevilInfo.m_Skill02.m_CooltimeTimer += m_DevilInfo.m_Skill02.m_Cooltime;

				++m_DevilInfo.m_Skill02.m_CurrentCharge;
			}
		}
	}
	public void OnSkill01(DevilSkillArg arg)
	{
		if (m_DevilInfo.m_Skill01.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill01.m_CurrentCharge;

			Skill01Event?.Invoke(arg);
		}
	}
	public void OnSkill02(DevilSkillArg arg)
	{
		if (m_DevilInfo.m_Skill02.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill02.m_CurrentCharge;

			Skill02Event?.Invoke(arg);
		}
	}
	public void GetDamage(float damage)
	{
		if (IsDie)
			return;

		float Damage = damage - m_DevilInfo.m_Def;
		if (Damage < 1f)
			Damage = 1f;

		m_DevilInfo.m_HP -= Damage;

		UpdateHPEvent?.Invoke(MaxHP, HP);

		if (m_DevilInfo.m_HP <= 0f)
		{
			m_DevilInfo.isDie = true;
			Die();
		}
	}

	public void CallAttack()
	{
		// 내부 데이터 정리
		m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
		m_DevilInfo.ShouldFindTarget = true;

		// 기본 스킬 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
		SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

		// 기본 대미지 설정
		statData.Dmg *= m_DevilInfo_Excel.Atk;
		statData.Dmg += statData.Dmg_plus;

		// 기본 스킬 투사체 생성
		int DefaultSkillCode = conditionData.projectile_prefab;
		if ((E_TargetType)m_DevilInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList();

			for (int i = 0; i < EnemyList.Count; ++i)
			{
				Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);
				DefaultSkill.transform.position = m_DevilInfo.AttackPivot.position;
				DefaultSkill.enabled = true;
				DefaultSkill.gameObject.SetActive(true);

				// 기본 스킬 데이터 설정
				DefaultSkill.InitializeSkill(EnemyList[i], conditionData, statData);
			}
		}
		else
		{
			Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);
			DefaultSkill.transform.position = m_DevilInfo.AttackPivot.position;
			DefaultSkill.enabled = true;
			DefaultSkill.gameObject.SetActive(true);

			// 기본 스킬 데이터 설정
			DefaultSkill.InitializeSkill(m_Target, conditionData, statData);
		}
	}
	public void CallDie()
	{
		DeathEvent?.Invoke(new GameEndData()
		{
			IsWin = false
		});
	}
	#endregion

	[System.Serializable]
	public struct S_DevilData
	{
		// 회전 속도
		public float RotateSpeed;
		// 초기 바라볼 방향
		public Vector3 LookingDir;
		// 적 감지 여부
		public bool ShouldFindTarget;
		// 공격 피벗
		public Transform AttackPivot;
		// 피격 피벗
		public Transform HitPivot;
		// 사망 여부
		public bool isDie;

		// 기본 스킬 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;

		// 스킬
		public S_DevilSkillData m_Skill01;
		public S_DevilSkillData m_Skill02;

		public float m_HP;
		public float m_Def;
		public float m_DefaultSkill_LifeSteal;
	}
	// 마왕 스킬 정보
	[System.Serializable]
	public struct S_DevilSkillData
	{
		public SkillCondition_TableExcel m_ConditionData;
		public SkillStat_TableExcel m_StatData;

		public E_SkillType m_SkillType;
		public E_SkillRangeType m_SkillRangeType;
		public int m_MaxCharge;
		public int m_CurrentCharge;
		public float m_Cooltime;
		public float m_CooltimeTimer;
	}

	public struct DevilSkillArg
	{
		public S_DevilSkillData skillData;
		public E_Direction dir;
	}

	public enum E_SkillType
	{
		None,

		Active,
		Passive,
	}
	public enum E_SkillRangeType
	{
		None,

		Fix,
		Direction,
		All
	}
}
