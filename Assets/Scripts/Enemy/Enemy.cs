using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
	[SerializeField]
	protected Enemy_TableExcel m_EnemyInfo_Excel;
	[SerializeField]
	protected S_EnemyData m_EnemyInfo;

	//현재 바라보고 있는 waypoint
	[SerializeField]
	protected WayPoint m_WayPoint;

	#region 내부 컴포넌트
	// 애니메이터
	protected EnemyAnimator m_Animator;
	// 범위
	protected SphereCollider m_RangeCollider;
	// 체력바
	protected EnemyHPBar m_HPBar;
	#endregion
	#region 내부 프로퍼티
	#region 매니저
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected WayPointManager M_WayPoint => WayPointManager.Instance;
	protected EffectManager M_Effect => EffectManager.Instance;
	protected EnemySkillManager M_EnemySkill => EnemySkillManager.Instance;
	protected EnemyHPBarManager M_EnemyHPBar => EnemyHPBarManager.Instance;

	protected FloatingTextManager M_DamageText => FloatingTextManager.Instance;
	#endregion

	protected float MoveSpeed
	{
		get
		{
			float moveSpeed = (m_EnemyInfo.Move_spd - m_EnemyInfo.Debuff_Move_spd_Fix) * m_EnemyInfo.Debuff_Move_spd_Percent * Time.deltaTime;
			return moveSpeed < 0f ? 0f : moveSpeed;
		}
	}
	#endregion
	#region 외부 프로퍼티
	public string Get_EnemyName_EN => m_EnemyInfo_Excel.Name_EN;
	public float Get_EnemyDef => (m_EnemyInfo.Def - m_EnemyInfo.Debuff_Def_Fix) * m_EnemyInfo.Debuff_Def_Percent;

	public E_Direction Direction { get => m_EnemyInfo.Direction; set => m_EnemyInfo.Direction = value; }
	public Transform HitPivot => m_EnemyInfo.HitPivot;
	public bool IsDead => m_EnemyInfo.IsDead;
	#endregion

	#region 내부 함수
	private void StartSkill()
	{
		m_Animator.SetTrigger("Skill");
	}
	private void GetNextWayPoint()
	{
		transform.position = m_WayPoint.transform.position;

		m_WayPoint = m_WayPoint.next;

		if (null != m_WayPoint)
			transform.LookAt(m_WayPoint.transform);
		else
			transform.LookAt(Vector3.zero);
	}

	private bool ApplyDamage(float damage, bool isCrit)
	{
		// 대미지 적용
		m_EnemyInfo.HP -= damage;

		// 대미지 텍스트
		Vector3 text_position = transform.position + Vector3.forward * 2.5f;
		M_DamageText.SpawnDamageText(((int)damage).ToString(), new FloatingTextFilter()
		{
			position = text_position,
			postionType = FloatingTextFilter.E_PostionType.Screen,
			outlineColor = isCrit ? M_Enemy.criticalDamageColor : M_Enemy.normalDamageColor,
			outlineWidth = 0.3f,
			width = isCrit ? 0.9f : 0.7f,
			height = isCrit ? 0.9f : 0.7f,
		});

		// 체력바 UI
		if (!m_HPBar.gameObject.activeSelf)
		{
			m_HPBar.gameObject.SetActive(true);
		}
		m_HPBar.fillAmount = m_EnemyInfo.HP / m_EnemyInfo_Excel.HP;

		// 사망 확인
		if (m_EnemyInfo.HP <= 0)
		{
			SetAnimation_Death();
			m_EnemyInfo.IsDead = true;
			return true;
		}

		return false;
	}
	private void InstantKill()
	{
		// 대미지 적용
		m_EnemyInfo.HP -= m_EnemyInfo.HP;

		// 대미지 텍스트
		Vector3 text_position = transform.position + Vector3.forward * 2.5f;
		M_DamageText.SpawnDamageText("Death", new FloatingTextFilter()
		{
			position = text_position,
			postionType = FloatingTextFilter.E_PostionType.Screen,
			outlineColor = M_Enemy.criticalDamageColor,
			outlineWidth = 0.3f,
			width = 0.9f,
			height = 0.9f,
		});

		// 사망
		SetAnimation_Death();
		m_EnemyInfo.IsDead = true;
	}

	private IEnumerator Co_Dot_Damage(S_Buff buff)
	{
		if (buff.BuffType != E_BuffType.Dot_Dmg)
		{
			Debug.LogError("도트 대미지 오류");
			yield break;
		}

		float tickDamage = buff.BuffAmount;
		float duration = buff.Duration;

		while (true)
		{
			if (duration <= 0f)
				yield break;

			if (ApplyDamage(tickDamage, false))
				yield break;

			duration -= Time.deltaTime;

			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator Co_Debuff_Fix(S_Buff buff)
	{
		switch (buff.BuffType)
		{
			case E_BuffType.Atk:
				{
					m_EnemyInfo.Debuff_Atk_Fix += buff.BuffAmount;
				}
				break;
			case E_BuffType.Def:
				{
					m_EnemyInfo.Debuff_Def_Fix += buff.BuffAmount;
				}
				break;
			case E_BuffType.Move_spd:
				{
					m_EnemyInfo.Debuff_Move_spd_Fix += buff.BuffAmount;
				}
				break;
			case E_BuffType.Stun:
				{
					m_EnemyInfo.Debuff_UseStun = true;
				}
				break;
			case E_BuffType.Dot_Dmg:
				{
					StartCoroutine(Co_Dot_Damage(buff));
				}
				yield break;
			case E_BuffType.Insta_Kill:
				{
					InstantKill();
				}
				yield break;
			case E_BuffType.CritDmg_less:
				{
					m_EnemyInfo.Debuff_UseCrit = true;
				}
				yield break;
			case E_BuffType.Shield:
				{
					m_EnemyInfo.Debuff_UseShield = true;
				}
				break;
			default:
				Debug.LogError("몬스터 디버프 타입 에러");
				break;
		}

		yield return new WaitForSeconds(buff.Duration);

		switch (buff.BuffType)
		{
			case E_BuffType.Atk:
				{
					m_EnemyInfo.Debuff_Atk_Fix -= buff.BuffAmount;
				}
				break;
			case E_BuffType.Def:
				{
					m_EnemyInfo.Debuff_Def_Fix -= buff.BuffAmount;
				}
				break;
			case E_BuffType.Move_spd:
				{
					m_EnemyInfo.Debuff_Move_spd_Fix -= buff.BuffAmount;
				}
				break;
			case E_BuffType.Stun:
				{
					m_EnemyInfo.Debuff_UseStun = false;
				}
				break;
			case E_BuffType.CritDmg_less:
				{
					m_EnemyInfo.Debuff_UseCrit = false;
				}
				break;
			case E_BuffType.Shield:
				{
					m_EnemyInfo.Debuff_UseShield = false;
				}
				break;
			default:
				Debug.LogError("몬스터 디버프 타입 에러");
				break;
		}
	}
	private IEnumerator Co_Debuff_Percent(S_Buff buff)
	{
		switch (buff.BuffType)
		{
			case E_BuffType.Atk:
				{
					m_EnemyInfo.Debuff_Atk_Percent *= buff.BuffAmount;
				}
				break;
			case E_BuffType.Def:
				{
					m_EnemyInfo.Debuff_Def_Percent *= buff.BuffAmount;
				}
				break;
			case E_BuffType.Move_spd:
				{
					m_EnemyInfo.Debuff_Move_spd_Percent *= buff.BuffAmount;
				}
				break;
			case E_BuffType.Stun:
				{
					m_EnemyInfo.Debuff_UseStun = true;
				}
				break;
			case E_BuffType.Dot_Dmg:
				{
					StartCoroutine(Co_Dot_Damage(buff));
				}
				yield break;
			case E_BuffType.Insta_Kill:
				{
					InstantKill();
				}
				yield break;
			case E_BuffType.CritDmg_less:
				{
					float hp_rate = m_EnemyInfo.HP / m_EnemyInfo_Excel.HP;

					if (hp_rate <= buff.BuffAmount)
					{
						m_EnemyInfo.Debuff_UseCrit = true;
					}
				}
				break;
			case E_BuffType.Shield:
				{
					m_EnemyInfo.Debuff_UseShield = true;
				}
				break;
			default:
				Debug.LogError("몬스터 디버프 타입 에러");
				break;
		}

		yield return new WaitForSeconds(buff.Duration);

		switch (buff.BuffType)
		{
			case E_BuffType.Atk:
				{
					m_EnemyInfo.Debuff_Atk_Percent /= buff.BuffAmount;
				}
				break;
			case E_BuffType.Def:
				{
					m_EnemyInfo.Debuff_Def_Percent /= buff.BuffAmount;
				}
				break;
			case E_BuffType.Move_spd:
				{
					m_EnemyInfo.Debuff_Move_spd_Percent /= buff.BuffAmount;
				}
				break;
			case E_BuffType.Stun:
				{
					m_EnemyInfo.Debuff_UseStun = false;
				}
				break;
			case E_BuffType.CritDmg_less:
				{
					m_EnemyInfo.Debuff_UseCrit = false;
				}
				break;
			case E_BuffType.Shield:
				{
					m_EnemyInfo.Debuff_UseShield = false;
				}
				break;
			default:
				Debug.LogError("몬스터 디버프 타입 에러");
				break;
		}
	}
	#endregion
	#region 외부 함수
	public void InitializeEnemy(int code, E_Direction dir)
	{
		#region 내부 컴포넌트
		if (null == m_Animator)
		{
			m_Animator = transform.Find("Mesh").GetComponent<EnemyAnimator>();
			m_Animator.Initialize(this);
		}

		// 공격 사거리
		if (null == m_RangeCollider)
		{
			m_RangeCollider = transform.Find("EnemySkillRange").GetComponent<SphereCollider>();
			m_RangeCollider.gameObject.layer = LayerMask.NameToLayer("EnemySkillRange");
			m_RangeCollider.isTrigger = true;
		}

		// 체력바
		if (null == m_HPBar)
		{
			m_HPBar = M_EnemyHPBar.SpawnHPBar();
		}
		m_HPBar.fillAmount = 1f;
		m_HPBar.m_EnemyTransform = transform;
		m_HPBar.transform.position = M_EnemyHPBar.m_HPBarCanvas.worldCamera.WorldToScreenPoint(transform.position) + M_EnemyHPBar.Distance;
		#endregion

		#region 엑셀 데이터
		m_EnemyInfo_Excel = M_Enemy.GetData(code);
		#endregion

		#region 내부 데이터
		m_EnemyInfo.Atk = m_EnemyInfo_Excel.Atk;
		m_EnemyInfo.HP = m_EnemyInfo_Excel.HP;
		m_EnemyInfo.Def = m_EnemyInfo_Excel.Def;
		m_EnemyInfo.Move_spd = m_EnemyInfo_Excel.Move_spd;

		// 방향 설정
		m_EnemyInfo.Direction = dir;
		// 스폰 포인트 설정
		m_WayPoint = M_WayPoint.GetFirstWayPoint(dir);
		// 위치 설정
		transform.position = m_WayPoint.transform.position;
		// 다음 웨이 포인트 설정
		m_WayPoint = m_WayPoint.next;
		// 방향 설정
		transform.LookAt(m_WayPoint.transform);
		// 사망 여부
		m_EnemyInfo.IsDead = false;

		// 공격 피벗
		if (null == m_EnemyInfo.AttackPivot)
		{
			m_EnemyInfo.AttackPivot = transform.GetChild("AttackPivot");
		}
		// 피격 피벗
		if (null == m_EnemyInfo.HitPivot)
		{
			m_EnemyInfo.HitPivot = transform.GetChild("HitPivot");
		}

		#region 기본 스킬
		// 기본 스킬 데이터
		m_EnemyInfo.Condition_Default = M_EnemySkill.GetConditionData(m_EnemyInfo_Excel.Atk_Code);
		m_EnemyInfo.Stat_Default = M_EnemySkill.GetStatData(m_EnemyInfo.Condition_Default.PassiveCode);
		// 기본 스킬 공격 속도
		m_EnemyInfo.AttackSpeed_Default = m_EnemyInfo.Stat_Default.CoolTime;
		// 기본 스킬 타이머
		m_EnemyInfo.AttackTimer_Default = 0f;
		#endregion

		#region 디버프
		m_EnemyInfo.Debuff_Atk_Fix = 0f;
		m_EnemyInfo.Debuff_Def_Fix = 0f;
		m_EnemyInfo.Debuff_Move_spd_Fix = 0f;

		m_EnemyInfo.Debuff_Atk_Percent = 1f;
		m_EnemyInfo.Debuff_Def_Percent = 1f;
		m_EnemyInfo.Debuff_Move_spd_Percent = 1f;

		m_EnemyInfo.Debuff_UseStun = false;
		m_EnemyInfo.Debuff_Dot_Damage = 0f;
		m_EnemyInfo.Debuff_Dot_Duration = 0f;
		m_EnemyInfo.Debuff_UseCrit = false;
		m_EnemyInfo.Debuff_UseShield = false;
		#endregion
		#endregion
	}
	public void FinializeEnemy()
	{
		M_EnemyHPBar.DespawnHPBar(m_HPBar);
		m_HPBar = null;
	}

	public void AddDebuff(S_Buff buff)
	{
		if (buff.BuffType == E_BuffType.None)
			return;
		if (Random.Range(0.0001f, 1f) > buff.BuffRand)
			return;

		if (buff.AddType == E_AddType.Fix)
			StartCoroutine(Co_Debuff_Fix(buff));
		else if (buff.AddType == E_AddType.Percent)
			StartCoroutine(Co_Debuff_Percent(buff));

		// 이펙트
		Effect effect = M_Effect.SpawnEffect(buff.Prefab, buff.Duration);
		effect.transform.SetParent(HitPivot);
		effect.transform.localPosition = Vector3.zero;
	}

	// 대미지
	public void On_Damage(float _damage, bool isCrit)
	{
		// 예외 처리
		if (m_EnemyInfo.IsDead)
			return;

		float damage = _damage;

		if (m_EnemyInfo.Debuff_UseShield)
			// 쉴드 체크
			damage = 1f;
		else
			// 방어력 계산
			damage -= m_EnemyInfo.Def;

		// 최소 대미지 적용
		if (damage <= 0f)
			damage = 1f;

		// 대미지 적용
		ApplyDamage(damage, isCrit);
	}
	public void On_Damage(float _damage, S_Critical critical)
	{
		float damage = _damage;

		#region 크리티컬
		float critRate = m_EnemyInfo.Debuff_UseCrit ? 1f : critical.critRate;
		float critDmg = critical.critDmg;

		bool isCrit = Random.Range(0.00001f, 1f) <= critRate;

		if (isCrit)
		{
			damage *= critDmg;
		}
		#endregion

		On_Damage(damage, isCrit);
	}

	// 사망
	public void SetAnimation_Death()
	{
		m_Animator.SetBool("IsDead", true);
		m_Animator.SetTrigger("Die");
		m_HPBar.gameObject.SetActive(false);
	}
	// 공격
	#endregion
	#region 유니티 콜백 함수
	private void Update()
	{
		if (m_EnemyInfo.IsDead)
			return;

		if (m_EnemyInfo.Debuff_UseStun)
			return;

		#region 공격
		if (null == m_WayPoint)
		{
			if (m_EnemyInfo.AttackTimer_Default >= m_EnemyInfo.Stat_Default.CoolTime)
			{
				m_Animator.SetTrigger("Attack");
				m_EnemyInfo.AttackTimer_Default = 0f;
			}
			else
			{
				m_EnemyInfo.AttackTimer_Default += Time.deltaTime;
			}

			return;
		}
		#endregion

		#region 이동
		Vector3 dir = m_WayPoint.transform.position - transform.position;
		transform.Translate(dir.normalized * MoveSpeed, Space.World);
		m_HPBar.transform.position = M_EnemyHPBar.m_HPBarCanvas.worldCamera.WorldToScreenPoint(transform.position) + M_EnemyHPBar.Distance;

		if (Vector3.Distance(transform.position, m_WayPoint.transform.position) <= 0.2f)
		{
			GetNextWayPoint();
		}
		#endregion
	}
	#endregion

	#region Call함수
	public void CallAttack()
	{
		m_EnemyInfo.Atk *= m_EnemyInfo.Stat_Default.Dmg_Percent;
		M_EnemySkill.SpawnProjectileSkill(m_EnemyInfo.Condition_Default.projectile_prefab, m_EnemyInfo.Atk, m_EnemyInfo.Condition_Default, m_EnemyInfo.Stat_Default, m_EnemyInfo.AttackPivot);
	}
	public void CallSkill()
	{

	}
	public void CallDie()
	{
		M_Enemy.Despawn(this);
		m_Animator.SetBool("IsDead", false);
	}
	#endregion

	[Serializable]
	public struct S_EnemyData
	{
		public float Atk;
		public float HP;
		public float Def;
		public float Move_spd;

		#region 디버프
		public float Debuff_Atk_Fix;
		public float Debuff_Atk_Percent;
		public float Debuff_Def_Fix;
		public float Debuff_Def_Percent;
		public float Debuff_Move_spd_Fix;
		public float Debuff_Move_spd_Percent;
		public bool Debuff_UseStun;
		public float Debuff_Dot_Damage;
		public float Debuff_Dot_Duration;
		public bool Debuff_UseCrit;
		public bool Debuff_UseShield;
		#endregion

		// 적 방향
		public E_Direction Direction;
		// 공격 피벗
		public Transform AttackPivot;
		// 피격 피벗
		public Transform HitPivot;
		// 사망 여부
		public bool IsDead;

		#region 기본 스킬
		// 기본 스킬 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;
		#endregion
	}
}