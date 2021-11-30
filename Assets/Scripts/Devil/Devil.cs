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

	protected Vector3 m_DrawGizmoPoint;
	protected Vector3 m_SkillRangePoint;
	protected Vector3 m_SkillSize;
	public event Action OnLostDefaultTargetEvent;

	public delegate void DevilUpdateHPHandler(float max, float current);
	public event DevilUpdateHPHandler UpdateHPEvent;

	public event GameOverHandler OnGameEndEvent;

	public event Action OnSkillCountChangedEvent;
	public event Action OnUseSkillEvent;

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
	// 음향 매니저
	protected MusicManager M_Music => MusicManager.Instance;
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
	public S_DevilSkillData Skill01 => m_DevilInfo.m_Skill01;
	public S_DevilSkillData Skill02 => m_DevilInfo.m_Skill02;
	public Transform HitPivot => m_DevilInfo.HitPivot;
	public E_Devil GetBossType => m_DevilInfo.Boss_type;
	public bool IsCastingSkill01 { get => Skill01.m_IsCasting; set => m_DevilInfo.m_Skill01.m_IsCasting = value; }
	public bool IsCastingSkill02 { get => Skill02.m_IsCasting; set => m_DevilInfo.m_Skill02.m_IsCasting = value; }
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
		SkillCondition_TableExcel skill_codition = m_DevilInfo.m_Skill01.m_ConditionData = M_Skill.GetConditionData(m_DevilInfo_Excel.Skill1Code);
		SkillStat_TableExcel skill_stat = m_DevilInfo.m_Skill01.m_StatData = M_Skill.GetStatData(m_DevilInfo.m_Skill01.m_ConditionData.PassiveCode);

		//테이블이 없어서 임의로 값을 넣음.
		switch (m_DevilInfo.Boss_type)
		{
			case E_Devil.HateQueen:
				m_DevilInfo.m_Skill01.m_SkillType = E_SkillType.Active;
				m_DevilInfo.m_Skill01.m_SkillRangeType = E_SkillRangeType.Direction;
				break;
			case E_Devil.HellLord:
				m_DevilInfo.m_Skill01.m_SkillType = E_SkillType.Active;
				m_DevilInfo.m_Skill01.m_SkillRangeType = E_SkillRangeType.Fix;
				break;
			case E_Devil.FrostLich:
				break;
		}

		m_DevilInfo.m_Skill01.m_MaxCharge = skill_stat.Max_Charge;
		m_DevilInfo.m_Skill01.m_Cooltime = skill_stat.CoolTime;
		m_DevilInfo.m_Skill01.m_CooltimeTimer = skill_stat.CoolTime;
		m_DevilInfo.m_Skill01.m_Dmg_Fix = m_DevilInfo.m_Atk + skill_stat.Dmg_Fix;
		m_DevilInfo.m_Skill01.m_Dmg_Percent = skill_stat.Dmg_Percent;
		m_DevilInfo.m_Skill01.m_Size = skill_stat.Size;

		//스킬 2
		skill_codition = m_DevilInfo.m_Skill02.m_ConditionData = M_Skill.GetConditionData(m_DevilInfo_Excel.Skill2Code);
		skill_stat = m_DevilInfo.m_Skill02.m_StatData = M_Skill.GetStatData(m_DevilInfo.m_Skill02.m_ConditionData.PassiveCode);

		//테이블이 없어서 임의로 값을 넣음.
		switch (m_DevilInfo.Boss_type)
		{
			case E_Devil.HateQueen:
				m_DevilInfo.m_Skill02.m_SkillType = E_SkillType.Active;
				m_DevilInfo.m_Skill02.m_SkillRangeType = E_SkillRangeType.All;
				break;
			case E_Devil.HellLord:
				m_DevilInfo.m_Skill02.m_SkillType = E_SkillType.Passive;
				m_DevilInfo.m_Skill02.m_SkillRangeType = E_SkillRangeType.None;
				break;
			case E_Devil.FrostLich:
				break;
		}

		m_DevilInfo.m_Skill02.m_MaxCharge = skill_stat.Max_Charge;
		m_DevilInfo.m_Skill02.m_Cooltime = skill_stat.CoolTime;
		m_DevilInfo.m_Skill02.m_CooltimeTimer = skill_stat.CoolTime;
		m_DevilInfo.m_Skill02.m_Dmg_Fix = m_DevilInfo.m_Atk + skill_stat.Dmg_Fix;
		m_DevilInfo.m_Skill02.m_Dmg_Percent = skill_stat.Dmg_Percent;
		m_DevilInfo.m_Skill02.m_Size = skill_stat.Size;
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
		m_SkillSize = M_Devil.SkillRangeObj.transform.localScale * m_DevilInfo.m_Skill01.m_Size;
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
			M_Music.PlaySound(m_DevilInfo_Excel.AtkSound);
		}
	}
	// 마왕 스킬 쿨타임 감소
	protected void ReduceSkillCooldown()
	{
		ReduceSkill01Cooldown(Time.deltaTime);
		ReduceSkill02Cooldown(Time.deltaTime);
	}
	protected void OnSkill01()
	{
		if (m_DevilInfo.m_Skill01.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill01.m_CurrentCharge;

			#region 회전
			m_DevilInfo.RotateSpeed = 0f;
			// 바라볼 방향
			Vector3 lookingDir = m_DevilInfo.m_Skill01.m_MousePos - transform.position;
			// 바라볼 방향의 각도
			Vector3 angle = Quaternion.LookRotation(lookingDir).eulerAngles;
			// y축 회전만 하도록 초기화
			angle.x = 0f; angle.z = 0f;
			// 회전
			transform.eulerAngles = angle;
			#endregion

			SetSkill01Trigger();

			if (GetBossType == E_Devil.HellLord)
			{
				// 이펙트 생성
				Effect skillEffect = M_Effect.SpawnEffect(Skill01.m_ConditionData.Atk_prefab);
				if (null != skillEffect)
				{
					skillEffect.transform.position = m_DevilInfo.m_Skill01.m_MousePos;
					skillEffect.gameObject.SetActive(true);
				}
			}

			OnUseSkillEvent?.Invoke();
		}
	}
	protected void OnSkill02()
	{
		if (m_DevilInfo.m_Skill02.m_CurrentCharge > 0)
		{
			--m_DevilInfo.m_Skill02.m_CurrentCharge;

			SetSkill02Trigger();

			OnUseSkillEvent?.Invoke();
		}
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
		m_DevilAnimator.SetBool("IsDead", true);
	}
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

				OnSkillCountChangedEvent?.Invoke();
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

				OnSkillCountChangedEvent?.Invoke();
			}
		}
	}

	public void DevilSkillCasting(E_SkillNumber number)
	{
		Vector3 mousePos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(mousePos);
		RaycastHit hit = new RaycastHit();
		LayerMask mask = LayerMask.GetMask("DevilSkillCollider");

		switch (number)
		{
			case E_SkillNumber.Skill1:
				{
					switch (m_DevilInfo.Boss_type)
					{
						case E_Devil.HateQueen:
							{
								// 기즈모 그리기
								Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1f);

								if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
								{
									// 기즈모 그리기 시작
									M_Devil.IsDrawGizmo = true;
									M_Devil.ActiveSkillRange = true;
									m_DrawGizmoPoint = hit.point;
									m_SkillRangePoint = hit.point;
									if (Input.GetMouseButtonDown(0))
									{
										if (!Enum.TryParse<E_Direction>(hit.transform.tag, out m_DevilInfo.m_Skill01.m_Direction))
										{
											m_DevilInfo.m_Skill01.m_Direction = E_Direction.None;
										}

										m_DevilInfo.m_Skill01.m_MousePos = hit.point;
										m_DevilInfo.m_Skill01.m_IsCasting = false;

										OnSkill01();
										M_Devil.ActiveSkillRange = false;
										// 기즈모 그리기 끝
										M_Devil.IsDrawGizmo = false;
									}
								}
								else
								{
									M_Devil.ActiveSkillRange = false;
								}
							}
							break;
						case E_Devil.HellLord:
							{
								// 기즈모 그리기
								Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1f);

								if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
								{
									// 기즈모 그리기 시작
									M_Devil.IsDrawGizmo = true;
									M_Devil.ActiveSkillRange = true;
									m_DrawGizmoPoint = hit.point;
									m_SkillRangePoint = hit.point;
									if (Input.GetMouseButtonDown(0))
									{
										if (!Enum.TryParse<E_Direction>(hit.transform.tag, out m_DevilInfo.m_Skill01.m_Direction))
										{
											m_DevilInfo.m_Skill01.m_Direction = E_Direction.None;
										}

										m_DevilInfo.m_Skill01.m_MousePos = hit.point;
										m_DevilInfo.m_Skill01.m_IsCasting = false;

										OnSkill01();

										// 기즈모 그리기 끝
										M_Devil.IsDrawGizmo = false;
										M_Devil.ActiveSkillRange = false;
									}
								}
								else
								{
									M_Devil.ActiveSkillRange = false;
								}
							}
							break;
						case E_Devil.FrostLich: // 미구현
							{
								m_DevilInfo.m_Skill01.m_IsCasting = false;
							}
							break;
						default:
							{
								Debug.LogError("보스 타입 에러");
							}
							break;
					}
				}
				break;
			case E_SkillNumber.Skill2:
				{
					switch (m_DevilInfo.Boss_type)
					{
						case E_Devil.HateQueen:
							{
								m_DevilInfo.m_Skill02.m_MousePos = hit.point;
								m_DevilInfo.m_Skill02.m_IsCasting = false;

								OnSkill02();
							}
							break;
						case E_Devil.HellLord: // 패시브 스킬
							{
								m_DevilInfo.m_Skill02.m_IsCasting = false;
							}
							break;
						case E_Devil.FrostLich: // 미구현
							{
								if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
								{
									// 기즈모 그리기 시작
									M_Devil.IsDrawGizmo = true;
									M_Devil.ActiveSkillRange = true;
									if (Input.GetMouseButton(0))
									{
										m_DevilInfo.m_Skill02.m_MousePos = hit.point;
										m_DevilInfo.m_Skill02.m_IsCasting = false;

										OnSkill02();

										// 기즈모 그리기 끝
										M_Devil.IsDrawGizmo = false;
										M_Devil.ActiveSkillRange = false;
									}
								}
								else
								{
									M_Devil.ActiveSkillRange = false;
								}
							}
							break;
						default:
							{
								Debug.LogError("보스 타입 에러");
							}
							break;
					}
				}
				break;
			default:
				{
					Debug.LogError("스킬 넘버 오류");
				}
				break;
		}
	}
	//public DevilSkillArg GetDevilSkillArg(E_SkillNumber number)
	//{
	//	DevilSkillArg temp = new DevilSkillArg();
	//	if (number == E_SkillNumber.Skill1)
	//		temp.skillData = m_DevilInfo.m_Skill01;
	//	else if (number == E_SkillNumber.Skill2)
	//		temp.skillData = m_DevilInfo.m_Skill02;
	//	return temp;
	//}
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
			M_Music.PlaySound(m_DevilInfo_Excel.DeathSound);
		}
	}

	public virtual void CallAttack()
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
	public abstract void CallSkill01();
	public abstract void CallSkill02();
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
		DrawSkillRange();
	}
	private void DrawSkillRange()
	{
		if(M_Devil.ActiveSkillRange)
		{
			M_Devil.SkillRangeObj.transform.position = m_SkillRangePoint;
			M_Devil.SkillRangeObj.transform.localScale = m_SkillSize;
			M_Devil.SkillRangeObj.SetActive(true);
		}
		else
		{
			M_Devil.SkillRangeObj.SetActive(false);
		}
	}
	private void OnDrawGizmos()
	{
		if (M_Devil.IsDrawGizmo)
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(m_DrawGizmoPoint, m_DevilInfo.m_Skill01.m_Size);
			Gizmos.color = color;
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
	}
	// 마왕 스킬 정보
	[System.Serializable]
	public struct S_DevilSkillData
	{
		public SkillCondition_TableExcel m_ConditionData;
		public SkillStat_TableExcel m_StatData;

		public E_Direction m_Direction;
		public E_SkillType m_SkillType;
		public E_SkillRangeType m_SkillRangeType;
		public int m_MaxCharge;
		public int m_CurrentCharge;
		public float m_Cooltime;
		public float m_CooltimeTimer;
		public Vector3 m_MousePos;//범위스킬 생성할 마우스 위치.
		public float m_Dmg_Fix;
		public float m_Dmg_Percent;
		public float m_Size;

		public bool m_IsCasting;
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
