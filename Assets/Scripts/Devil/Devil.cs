using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Devil : MonoBehaviour
{
	// 마왕 정보(엑셀)
	[SerializeField]
	protected Devil_TableExcel m_DevilInfo_Excel;
	// 마왕 정보
	[SerializeField]
	protected S_DevilData m_DevilInfo;

	// 타겟
	public Enemy m_Target_Default;

	public event Action OnLostDefaultTargetEvent;

	protected delegate void DevilSkillHandler(DevilSkillArg arg);
	protected event DevilSkillHandler Skill01Event;
	protected event DevilSkillHandler Skill02Event;

	public delegate void DevilUpdateHPHandler(float max, float current);
	public event DevilUpdateHPHandler UpdateHPEvent;

	public event GameOverHandler OnGameEndEvent;

	#region 내부 컴포넌트
	[SerializeField, ReadOnly]
	protected DevilAnimator m_DevilAnimator;

	[SerializeField]
	protected AttackRange m_AttackRange_Default;
	#endregion
	#region 내부 프로퍼티
	#region 매니저
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
	#endregion

	// 마왕 회전 속도
	protected float RotateSpeed => m_DevilInfo.RotateSpeed * Time.deltaTime;
	// 타겟까지의 거리
	protected float DistanceToTarget_Default => Vector3.Distance(transform.position, m_Target_Default.transform.position);
	// 타겟 생존 여부
	protected bool IsTargetDead_Default => null == m_Target_Default || m_Target_Default.IsDead;
	// 타겟 놓쳤는 지
	protected bool LostTarget_Default => m_AttackRange_Default.Range < DistanceToTarget_Default;
	#endregion
	#region 외부 프로퍼티
	public float MaxHP => m_DevilInfo_Excel.HP;
	public float HP => m_DevilInfo.m_HP;
	public bool IsDead => m_DevilInfo.IsDead;

	public Devil_TableExcel ExcelData => m_DevilInfo_Excel;
	public Transform HitPivot => m_DevilInfo.HitPivot;
	public E_Devil GetBossType => m_DevilInfo.Boss_type;

	#endregion

	#region 내부 함수
	// 마왕 초기화
	protected void InitializeDevil(E_Devil no)
	{
		#region 엑셀 데이터 정리
		m_DevilInfo_Excel = M_Devil.GetData(no);
		#endregion

		#region 내부 데이터 정리
		m_DevilInfo.Boss_type = no;
		m_DevilInfo.RotateSpeed = 5f;
		m_DevilInfo.LookingDir = Vector3.back;

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
		m_DevilInfo.Stat_Default = M_Skill.GetStatData(m_DevilInfo.Condition_Default.PassiveCode);
		// 기본 스킬
		m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
		m_DevilInfo.AttackTimer_Default = m_DevilInfo.Stat_Default.CoolTime;

		// 현재 체력
		m_DevilInfo.m_HP = m_DevilInfo_Excel.HP;
		m_DevilInfo.m_originalHP = m_DevilInfo_Excel.HP;
		m_DevilInfo.m_halfHP = m_DevilInfo_Excel.HP * 0.5f;
		// 현재 방어력
		m_DevilInfo.m_Def = m_DevilInfo_Excel.Def;

		m_DevilInfo.m_Atk = m_DevilInfo_Excel.Atk;

		//여기서 스킬 데이터 넣기
		//스킬 1
		m_DevilInfo.m_Skill01.m_ConditionData = M_Skill.GetConditionData(m_DevilInfo_Excel.Skill1Code);
		m_DevilInfo.m_Skill01.m_StatData = M_Skill.GetStatData(m_DevilInfo.m_Skill01.m_ConditionData.PassiveCode);
		SkillCondition_TableExcel skill_codition = m_DevilInfo.m_Skill01.m_ConditionData;
		SkillStat_TableExcel skill_stat = m_DevilInfo.m_Skill01.m_StatData;
		//테이블이 없어서 임의로 값을 넣음.
		m_DevilInfo.m_Skill01.m_SkillType = E_SkillType.Active;
		m_DevilInfo.m_Skill01.m_SkillRangeType = E_SkillRangeType.Direction;
		m_DevilInfo.m_Skill01.m_MaxCharge = skill_stat.Max_Charge;
		m_DevilInfo.m_Skill01.m_Cooltime = skill_stat.CoolTime;
		m_DevilInfo.m_Skill01.m_CooltimeTimer = skill_stat.CoolTime;
		m_DevilInfo.m_Skill01.m_Dmg_Fix = m_DevilInfo.m_Atk;
		m_DevilInfo.m_Skill01.m_Dmg_Fix += m_DevilInfo.m_Skill01.m_StatData.Dmg_Fix;
		m_DevilInfo.m_Skill01.m_Dmg_Percent = m_DevilInfo.m_Skill01.m_StatData.Dmg_Percent;
		m_DevilInfo.m_Size = m_DevilInfo.m_Skill01.m_StatData.Size;

		//스킬 2
		m_DevilInfo.m_Skill02.m_ConditionData = M_Skill.GetConditionData(m_DevilInfo_Excel.Skill1Code);
		m_DevilInfo.m_Skill02.m_StatData = M_Skill.GetStatData(m_DevilInfo.m_Skill02.m_ConditionData.PassiveCode);
		skill_codition = m_DevilInfo.m_Skill02.m_ConditionData;
		skill_stat = m_DevilInfo.m_Skill02.m_StatData;
		//테이블이 없어서 임의로 값을 넣음.
		m_DevilInfo.m_Skill02.m_SkillType = E_SkillType.Active;
		m_DevilInfo.m_Skill02.m_SkillRangeType = E_SkillRangeType.All;
		m_DevilInfo.m_Skill02.m_MaxCharge = skill_stat.Max_Charge;
		m_DevilInfo.m_Skill02.m_Cooltime = skill_stat.CoolTime;
		m_DevilInfo.m_Skill02.m_CooltimeTimer = skill_stat.CoolTime;
		m_DevilInfo.m_Skill02.m_Dmg_Fix = m_DevilInfo.m_Atk;
		m_DevilInfo.m_Skill02.m_Dmg_Fix += m_DevilInfo.m_Skill02.m_StatData.Dmg_Fix;
		m_DevilInfo.m_Skill01.m_Dmg_Percent = m_DevilInfo.m_Skill02.m_StatData.Dmg_Percent;
		m_DevilInfo.m_Size = m_DevilInfo.m_Skill02.m_StatData.Size;
		#endregion

		#region 내부 컴포넌트
		if (null == m_DevilAnimator)
		{
			m_DevilAnimator = GetComponentInChildren<DevilAnimator>(true);
			m_DevilAnimator.Initialize(this);
		}

		if (null == m_AttackRange_Default)
		{
			m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
			m_AttackRange_Default.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Default.InitializeAttackRange();
		}
		m_AttackRange_Default.Range = m_DevilInfo.Stat_Default.Range;
		m_AttackRange_Default.Direction = E_Direction.None;
		m_AttackRange_Default.CanFindTarget = true;
		#endregion

		#region 마왕 스킬 정리
		Skill01Event += DoSkill01;
		Skill02Event += DoSkill02;
		#endregion
	}
	// 마왕 회전
	protected void RotateToTarget()
	{
		if (!IsTargetDead_Default && !LostTarget_Default)
		{
			RotateToTarget(m_Target_Default);
		}
		else
		{
			RotateToTarget(null);
		}
	}
	protected void RotateToTarget(Enemy enemy)
	{
		// 바라볼 방향
		Vector3 lookingDir = m_DevilInfo.LookingDir;

		// 타겟이 있는 경우
		if (null != enemy)
		{
			// 바라볼 방향 수정
			lookingDir = enemy.transform.position - transform.position;
		}

		// 바라볼 방향의 각도
		Vector3 angle = Quaternion.LookRotation(lookingDir).eulerAngles;

		// y축 회전만 하도록 초기화
		angle.x = 0f; angle.z = 0f;

		// 회전
		transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, angle, RotateSpeed);
	}
	// 타겟 업데이트
	protected void UpdateTarget()
	{
		#region 기본 스킬
		if (IsTargetDead_Default || LostTarget_Default)
		{
			// 타겟 변경 기준에 따라
			switch ((E_TargetType)m_DevilInfo.Condition_Default.Target_type)
			{
				case E_TargetType.CloseTarget:
					{
						m_Target_Default = m_AttackRange_Default.GetNearTarget();
					}
					break;
				case E_TargetType.RandTarget:
					{
						m_Target_Default = m_AttackRange_Default.GetRandomTarget();
					}
					break;
				// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
				case E_TargetType.FixTarget:
					{
						if (null == m_Target_Default || // 예외처리
							DistanceToTarget_Default > m_DevilInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
						{
							m_Target_Default = m_AttackRange_Default.GetNearTarget();
						}
					}
					break;
				case E_TargetType.TileTarget:
					{
						m_Target_Default = m_AttackRange_Default.GetNearTarget();
					}
					break;
			}

			if (IsTargetDead_Default)
			{
				m_AttackRange_Default.RemoveTarget(m_Target_Default);
				m_Target_Default = null;
				OnLostDefaultTargetEvent?.Invoke();
			}
		}
		#endregion
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
		else if (!IsTargetDead_Default && !LostTarget_Default)
		{
			// 내부 데이터 정리
			m_DevilInfo.AttackTimer_Default -= m_DevilInfo.AttackSpeed_Default;

			SetAttackTrigger();
		}
		#endregion
	}
	// 마왕 스킬 쿨타임 감소
	protected void ReduceSkillCooldown()
	{
		ReduceSkill01Cooldown(Time.deltaTime);
		ReduceSkill02Cooldown(Time.deltaTime);
	}

	protected void SetAttackTrigger()
	{
		m_DevilAnimator.SetTrigger("Attack");
	}
	protected void SetSkill01Trigger()
	{
		m_DevilAnimator.SetTrigger("Skill01");
	}
	protected void SetSkill02Trigger()
	{
		m_DevilAnimator.SetTrigger("Skill02");
	}
	protected void SetDieTrigger()
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
	Vector3 mos = Vector3.one * 1000000f;

	RaycastHit hit = new RaycastHit();

	public void ActiveRange(E_SkillNumber number)
	{
		mos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(mos);
		mos = Camera.main.ScreenToWorldPoint(mos);

		LayerMask mask = LayerMask.GetMask("DevilSkillCollider");
		//스킬 1
		if (number == E_SkillNumber.Skill1)
		{
			Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1f);
			DevilSkillArg skillarg = GetDevilSkillArg(Devil.E_SkillNumber.Skill1);
			if (m_DevilInfo.m_Skill01.m_CurrentCharge <= 0)
			{
				M_Devil.Is_GizmoDraw = false;
				M_Devil.UseSkill = false;
				return;
			}
			//범위표시
			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
			{
				M_Devil.Is_GizmoDraw = true;
				if (Input.GetMouseButtonDown(0))
				{
					if (!Enum.TryParse<E_Direction>(hit.transform.tag, out skillarg.dir))
					{
						skillarg.dir = E_Direction.None;
					}

					m_DevilInfo.m_Skill01.mousepos = hit.point;
					OnSkill01(skillarg);
					M_Devil.Is_GizmoDraw = false;
					M_Devil.UseSkill = false;
				}
			}
		}//스킬2
		else if (number == E_SkillNumber.Skill2)
		{
			if (m_DevilInfo.Boss_type == E_Devil.HellLord)
			{
				return;
			}
			//범위표시
			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
			{
				M_Devil.Is_GizmoDraw = true;
				//M_Devil.Is_GizmoDraw = true;
				if (Input.GetMouseButton(0))
				{
					m_DevilInfo.m_Skill02.mousepos = hit.point;
					OnSkill02(GetDevilSkillArg(Devil.E_SkillNumber.Skill2));
					M_Devil.Is_GizmoDraw = false;
					M_Devil.UseSkill = false;
				}
			}
		}

	}
	public void OnSkill01(DevilSkillArg arg)
	{
		arg.mousepos = m_DevilInfo.m_Skill01.mousepos;
		if (m_DevilInfo.m_Skill01.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill01.m_CurrentCharge;

			m_DevilAnimator.SetTrigger("Skill01");
			Skill01Event?.Invoke(arg);
		}
	}
	public void OnSkill02(DevilSkillArg arg)
	{
		arg.mousepos = m_DevilInfo.m_Skill02.mousepos;
		if (m_DevilInfo.m_Skill02.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill02.m_CurrentCharge;

			m_DevilAnimator.SetTrigger("Skill02");
			Skill02Event?.Invoke(arg);
		}
	}
	public DevilSkillArg GetDevilSkillArg(E_SkillNumber number)
	{
		DevilSkillArg temp = new DevilSkillArg();
		if (number == E_SkillNumber.Skill1)
			temp.skillData = m_DevilInfo.m_Skill01;
		else if (number == E_SkillNumber.Skill2)
			temp.skillData = m_DevilInfo.m_Skill02;
		return temp;
	}
	public void GetDamage(float damage)
	{
		if (IsDead)
			return;

		float Damage = damage - m_DevilInfo.m_Def;
		if (Damage < 1f)
			Damage = 1f;

		m_DevilInfo.m_HP -= Damage;

		UpdateHPEvent?.Invoke(MaxHP, HP);

		if (m_DevilInfo.m_HP <= 0f)
		{
			m_DevilInfo.IsDead = true;
			SetDieTrigger();
		}
	}

	virtual public void CallAttack()
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

			// 기본 스킬 데이터 설정
			skill.InitializeSkill(
				target,
				conditionData,
				statData,
				new S_Critical(
					m_DevilInfo_Excel.Crit_rate,
					m_DevilInfo_Excel.Crit_Dmg
					),
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
	public void CallDie()
	{
		OnGameEndEvent?.Invoke(new GameEndData()
		{
			IsWin = false
		});
	}

	#endregion
	#region 유니티 콜백 함수
	protected void Update()
	{
		if (IsDead)
			return;
		UpdateTarget();
		RotateToTarget();
		AttackTarget();
		ReduceSkillCooldown();
	}
	private void OnDrawGizmos()
	{

		if (M_Devil.Is_GizmoDraw && m_DevilInfo.m_Skill01.m_CurrentCharge > 0)
		{
			if (hit.collider != null)
			{
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("DevilSkillCollider"))
				{
					Color color = Gizmos.color;
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(hit.point, m_DevilInfo.m_Size);
					Gizmos.color = color;
				}
			}
		}
	}

	#endregion

	[System.Serializable]
	public struct S_DevilData
	{
		public E_Devil Boss_type;
		// 회전 속도
		public float RotateSpeed;
		// 초기 바라볼 방향
		public Vector3 LookingDir;
		// 공격 피벗
		public Transform AttackPivot;
		// 피격 피벗
		public Transform HitPivot;
		// 사망 여부
		public bool IsDead;

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
		public float m_originalHP;
		public float m_halfHP;
		public float m_HP;
		public float m_Def;
		public float m_DefaultSkill_LifeSteal;
		public float m_Atk;
		public float m_Size;
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
		public Vector3 mousepos;//범위스킬 생성할 마우스 위치.
		public float m_Dmg_Fix;
		public float m_Dmg_Percent;
		public float m_total_Dmg;
	}

	public struct DevilSkillArg
	{
		public S_DevilSkillData skillData;
		public E_Direction dir;
		public Vector3 mousepos;//범위스킬 생성할 마우스 위치.
	}
	public enum E_SkillNumber
	{
		None,
		Skill1,
		Skill2,
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
