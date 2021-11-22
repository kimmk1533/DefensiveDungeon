using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
	// 스킬 정보 (엑셀)
	public SkillCondition_TableExcel m_ConditionInfo_Excel;
	public SkillStat_TableExcel m_StatInfo_Excel;
	// 스킬 정보
	public S_SkillData m_SkillInfo;

	// 타겟
	public Enemy m_Target;

	#region 내부 프로퍼티
	// 스킬 매니져
	protected SkillManager M_Skill => SkillManager.Instance;
	// 버프 매니져
	protected BuffManager M_Buff => BuffManager.Instance;
	// 이펙트 매니져
	protected EffectManager M_Effect => EffectManager.Instance;

	// 타겟 위치
	protected Vector3 TargetPos => (m_Target == null ? transform.position : m_Target.transform.position);
	// 스킬 이동 속도
	protected float MoveSpeed => m_StatInfo_Excel.Speed * Time.deltaTime;
	// 타겟까지의 방향
	protected Vector3 TargetDir => TargetPos - transform.position;
	// 타겟까지의 거리
	protected float DistanceToTarget => Vector3.Distance(transform.position, TargetPos);
	// 타겟 잃어버림
	protected bool LostTarget => m_Target == null || m_Target.IsDead;
	// 타겟에게 도착 여부
	protected bool ArrivedToTarget => DistanceToTarget <= m_StatInfo_Excel.Size;
	// 생존 시간 소진
	protected bool DepletedLifeTime => m_SkillInfo.LifeTime <= 0f;
	// 튕김 카운트 소진
	protected bool DepletedBounceCount => m_SkillInfo.BounceCount <= 0;
	#endregion

	#region 내부 함수
	protected bool CheckToDespawn()
	{
		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.NormalFire:
				return LostTarget || ArrivedToTarget;
			case E_AttackType.FixedFire:
			case E_AttackType.PenetrateFire:
				return DepletedLifeTime;
			case E_AttackType.BounceFire:
				return LostTarget || DepletedBounceCount;
		}

		return LostTarget || ArrivedToTarget;
	}
	protected void Despawn()
	{
		SkillCondition_TableExcel condition = M_Skill.GetConditionData(m_StatInfo_Excel.LoadCode);
		SkillStat_TableExcel stat = M_Skill.GetStatData(condition.PassiveCode);
		Skill skill = M_Skill.SpawnProjectileSkill(condition.projectile_prefab);

		if (null != skill)
		{
			switch ((E_FireType)condition.Atk_pick)
			{
				case E_FireType.Select_self:
					skill.transform.position = transform.position;
					break;
				case E_FireType.Select_enemy:
					skill.transform.position = m_Target.HitPivot.position;
					break;
			}

			skill.enabled = true;
			skill.gameObject.SetActive(true);

			skill.InitializeSkill(m_Target, condition, stat, m_SkillInfo.Critical);
		}

		m_Target = null;

		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.FixedFire:
				m_SkillInfo.FixedTargetList.Clear();
				break;
			case E_AttackType.PenetrateFire:
				m_SkillInfo.FixedTargetList.Clear();
				m_SkillInfo.PenetrateTargetList.Clear();
				break;
			case E_AttackType.BounceFire:
				m_SkillInfo.BounceTargetList.Clear();
				break;
		}

		m_SkillInfo.AttackRange.Clear();

		M_Skill.DespawnProjectileSkill(this);
	}
	protected bool CheckToUpdateTarget()
	{
		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.BounceFire:
				return ArrivedToTarget;
		}

		return false;
	}
	protected void UpdateTarget()
	{
		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.BounceFire:
				m_SkillInfo.InitPos = m_Target.transform.position;

				if (m_SkillInfo.CanOverlapBounce)
				{
					switch ((E_TargetType)m_ConditionInfo_Excel.Target_type)
					{
						case E_TargetType.CloseTarget:
							m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
							break;
						case E_TargetType.RandTarget:
							m_Target = m_SkillInfo.AttackRange.GetRandomTarget();
							break;
						case E_TargetType.FixTarget:
							m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
							break;
						case E_TargetType.TileTarget:
							m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
							break;
					}
				}
				else
				{
					do
					{
						m_SkillInfo.AttackRange.RemoveTarget(m_Target);

						switch ((E_TargetType)m_ConditionInfo_Excel.Target_type)
						{
							case E_TargetType.CloseTarget:
								m_Target = m_SkillInfo.AttackRange.GetNearTarget();
								break;
							case E_TargetType.RandTarget:
								m_Target = m_SkillInfo.AttackRange.GetRandomTarget();
								break;
							case E_TargetType.FixTarget:
								m_Target = m_SkillInfo.AttackRange.GetNearTarget();
								break;
							case E_TargetType.TileTarget:
								m_Target = m_SkillInfo.AttackRange.GetNearTarget();
								break;
						}

					} while (m_SkillInfo.BounceTargetList.Contains(m_Target));

					if (null != m_Target)
					{
						m_SkillInfo.BounceTargetList.Add(m_Target);
					}
				}
				break;
		}
	}
	protected void RotateSkill()
	{
		if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.FixedFire)
			return;

		switch ((E_MoveType)m_ConditionInfo_Excel.Move_type)
		{
			case E_MoveType.Straight:
				if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.PenetrateFire)
				{
					Vector3 dir = m_SkillInfo.TargetInitPos - m_SkillInfo.InitPos;
					transform.LookAt(transform.position + dir);
				}
				else
				{
					transform.LookAt(transform.position + TargetDir);
				}
				break;
			case E_MoveType.Curve:
				transform.LookAt(transform.position + GetCurveDir());
				break;
		}
	}
	protected void MoveSkill()
	{
		if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.FixedFire)
			return;

		switch ((E_MoveType)m_ConditionInfo_Excel.Move_type)
		{
			case E_MoveType.Straight:
				StraightMove();
				break;
			case E_MoveType.Curve:
				CurveMove();
				break;
		}
	}
	protected void StraightMove()
	{
		if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.PenetrateFire)
		{
			Vector3 dir = m_SkillInfo.TargetInitPos - m_SkillInfo.InitPos;
			transform.position += dir.normalized * MoveSpeed;
		}
		else
		{
			transform.position += TargetDir.normalized * MoveSpeed;
		}
	}
	private Vector3 GetCurveDir()
	{
		// 포물선 이동 제작
		// 출처: https://robatokim.tistory.com/entry/%EA%B2%8C%EC%9E%84%EC%88%98%ED%95%99-%EC%97%AD%ED%83%84%EB%8F%84%EA%B3%84%EC%82%B0%EC%9D%84-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EB%91%90%EC%A0%90-%EC%82%AC%EC%9D%98-%ED%8F%AC%EB%AC%BC%EC%84%A0-%EA%B5%AC%ED%95%98%EA%B8%B0
		Vector3 StartPos = m_SkillInfo.InitPos;
		Vector3 EndPos = m_Target.transform.position;
		Vector3 Pos = transform.position;

		// 최대 높이
		float MaxHeight = Mathf.Max(StartPos.y, EndPos.y) + 2.5f;
		// 최대 높이까지 가는 시간
		float MaxTime = (MaxHeight - StartPos.y) / m_StatInfo_Excel.Speed;

		float EndHeight = EndPos.y - StartPos.y;
		float Height = MaxHeight - StartPos.y;

		float g = 2 * Height / Mathf.Pow(MaxTime, 2f);

		float V_Y = Mathf.Sqrt(2 * g * Height);

		float a = g;
		float b = -2 * V_Y;
		float c = 2 * EndHeight;

		float EndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

		float V_X = -(StartPos.x - EndPos.x) / EndTime;
		float V_Z = -(StartPos.z - EndPos.z) / EndTime;

		Vector3 InitPos = StartPos;
		InitPos.y = 0f;
		EndPos.y = 0f;
		float Max = Vector3.Distance(InitPos, EndPos);

		Pos.y = 0f;
		float time = (1f + Time.deltaTime - Vector3.Distance(Pos, EndPos) / Max) * EndTime;

		Pos.x = StartPos.x + V_X * time;
		Pos.y = StartPos.y + (V_Y * time) - (g * time * time * 0.5f);
		Pos.z = StartPos.z + V_Z * time;

		Vector3 dir = Pos - transform.position;

		return dir;
	}
	protected void CurveMove()
	{
		transform.position += GetCurveDir().normalized * MoveSpeed;
	}
	protected void UpdateInfo()
	{
		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.NormalFire:
				break;
			case E_AttackType.FixedFire:
				if (m_SkillInfo.DotTimer <= 0f)
				{
					m_SkillInfo.DotTimer += 1f;
				}
				m_SkillInfo.DotTimer -= Time.deltaTime;
				m_SkillInfo.LifeTime -= Time.deltaTime;
				break;
			case E_AttackType.PenetrateFire:
				m_SkillInfo.LifeTime -= Time.deltaTime;
				break;
			case E_AttackType.BounceFire:
				break;
		}
	}
	protected bool CheckToAttack()
	{
		if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.PenetrateFire)
			return m_SkillInfo.FixedTargetList.Count > 0;

		if (LostTarget)
			return false;

		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.NormalFire:
				return ArrivedToTarget;
			case E_AttackType.FixedFire:
				return m_SkillInfo.DotTimer <= 0f;
			case E_AttackType.BounceFire:
				return ArrivedToTarget;
		}

		return ArrivedToTarget;
	}
	protected void Attack()
	{
		float damage = m_StatInfo_Excel.Dmg_Fix * m_StatInfo_Excel.Dmg_Percent;
		BuffCC_TableExcel buffData = M_Buff.GetData(m_StatInfo_Excel.Buff_CC);

		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.NormalFire:
				{
					if (buffData.Code != 0)
					{
						//m_Target.BuffList.Add(buffData);
					}

					float critRate = m_SkillInfo.Critical.critRate;
					float critDmg = m_SkillInfo.Critical.critDmg;

					bool isCrit = Random.Range(0.00001f, 1f) <= critRate;

					if (isCrit)
					{
						damage *= critDmg;
					}

					m_Target.On_DaMage(damage, isCrit);

					// 피격 이펙트 생성
					Effect hitEffect = M_Effect.SpawnEffect(m_ConditionInfo_Excel.damage_prefab);
					if (null != hitEffect)
					{
						hitEffect.transform.position = m_Target.HitPivot.transform.position;
						hitEffect.gameObject.SetActive(true);
					}
				}
				break;
			case E_AttackType.FixedFire:
				{
					for (int i = 0; i < m_SkillInfo.FixedTargetList.Count; ++i)
					{
						Enemy target = m_SkillInfo.FixedTargetList[i];

						if (null == target || target.IsDead)
							continue;

						if (buffData.Code != 0)
						{
							//target.BuffList.Add(buffData);
						}

						float critRate = m_SkillInfo.Critical.critRate;
						float critDmg = m_SkillInfo.Critical.critDmg;

						bool isCrit = Random.Range(0.00001f, 1f) <= critRate;

						if (isCrit)
						{
							damage *= critDmg;
						}

						m_Target.On_DaMage(damage, isCrit);

						// 피격 이펙트 생성
						Effect hitEffect = M_Effect.SpawnEffect(m_ConditionInfo_Excel.damage_prefab);
						if (null != hitEffect)
						{
							hitEffect.transform.position = target.HitPivot.transform.position;
							hitEffect.gameObject.SetActive(true);
						}
					}
				}
				break;
			case E_AttackType.PenetrateFire:
				{
					for (int i = 0; i < m_SkillInfo.FixedTargetList.Count; ++i)
					{
						Enemy target = m_SkillInfo.FixedTargetList[i];

						if (!m_SkillInfo.PenetrateTargetList.Contains(target))
						{
							if (buffData.Code != 0)
							{
								//target.BuffList.Add(buffData);
							}

							float critRate = m_SkillInfo.Critical.critRate;
							float critDmg = m_SkillInfo.Critical.critDmg;

							bool isCrit = Random.Range(0.00001f, 1f) <= critRate;

							if (isCrit)
							{
								damage *= critDmg;
							}

							m_Target.On_DaMage(damage, isCrit);

							m_SkillInfo.PenetrateTargetList.Add(target);

							// 피격 이펙트 생성
							Effect hitEffect = M_Effect.SpawnEffect(m_ConditionInfo_Excel.damage_prefab);
							if (null != hitEffect)
							{
								hitEffect.transform.position = target.HitPivot.transform.position;
								hitEffect.gameObject.SetActive(true);
							}
						}
					}
				}
				break;
			case E_AttackType.BounceFire:
				{
					if (buffData.Code != 0)
					{
						//m_Target.BuffList.Add(buffData);
					}

					float critRate = m_SkillInfo.Critical.critRate;
					float critDmg = m_SkillInfo.Critical.critDmg;

					bool isCrit = Random.Range(0.00001f, 1f) <= critRate;

					if (isCrit)
					{
						damage *= critDmg;
					}

					m_Target.On_DaMage(damage, isCrit);

					--m_SkillInfo.BounceCount;

					// 피격 이펙트 생성
					Effect hitEffect = M_Effect.SpawnEffect(m_ConditionInfo_Excel.damage_prefab);
					if (null != hitEffect)
					{
						hitEffect.transform.position = m_Target.HitPivot.transform.position;
						hitEffect.gameObject.SetActive(true);
					}
				}
				break;
		}
	}
	#endregion
	#region 외부 함수
	public void InitializeSkill(Enemy target,
		SkillCondition_TableExcel conditionData,
		SkillStat_TableExcel statData,
		S_Critical critical)
	{
		m_Target = target;
		m_SkillInfo.Critical = critical;

		m_ConditionInfo_Excel = conditionData;
		m_StatInfo_Excel = statData;

		switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
		{
			case E_AttackType.NormalFire:
				m_SkillInfo.AttackRange.Range = m_StatInfo_Excel.Size;
				break;
			case E_AttackType.FixedFire:
				m_SkillInfo.AttackRange.Range = m_StatInfo_Excel.Size;
				m_SkillInfo.FixedTargetList = m_SkillInfo.AttackRange.TargetList;
				break;
			case E_AttackType.PenetrateFire:
				m_SkillInfo.AttackRange.Range = m_StatInfo_Excel.Size;
				m_SkillInfo.FixedTargetList = m_SkillInfo.AttackRange.TargetList;
				break;
			case E_AttackType.BounceFire:
				// 다음 타겟 찾는 사거리 = 타워 사거리의 1 / 4
				m_SkillInfo.AttackRange.Range = m_StatInfo_Excel.Range * 0.25f;
				break;
		}

		m_SkillInfo.BounceCount = m_StatInfo_Excel.Target_num;
		m_SkillInfo.LifeTime = m_StatInfo_Excel.Life_Time;
		m_SkillInfo.InitPos = transform.position;
		m_SkillInfo.TargetInitPos = m_Target.transform.position;

		if (m_SkillInfo.FixedTargetList.Count > 0)
			m_SkillInfo.FixedTargetList.Clear();

		if (m_SkillInfo.PenetrateTargetList.Count > 0)
			m_SkillInfo.PenetrateTargetList.Clear();

		if (m_SkillInfo.BounceTargetList.Count > 0)
			m_SkillInfo.BounceTargetList.Clear();

		if (!m_SkillInfo.CanOverlapBounce &&
			null != m_Target)
		{
			m_SkillInfo.BounceTargetList.Add(m_Target);
		}
	}
	#endregion
	#region 유니티 콜백 함수
	private void Update()
	{
		if (CheckToDespawn())
		{
			Despawn();
			return;
		}

		UpdateInfo();

		RotateSkill();
		MoveSkill();

		if (CheckToAttack())
		{
			Attack();
		}

		if (CheckToUpdateTarget())
		{
			UpdateTarget();
		}
	}
	#endregion

	[System.Serializable]
	public struct S_SkillData
	{
		public List<S_Buff> DebuffList;
		public bool CanOverlapBounce;
		public int BounceCount;
		public List<Enemy> FixedTargetList;
		public List<Enemy> PenetrateTargetList;
		public List<Enemy> BounceTargetList;
		public float LifeTime;
		public float DotTimer;
		public AttackRange AttackRange;
		public Vector3 InitPos;
		public Vector3 TargetInitPos;

		public S_Critical Critical;
	}
}

public struct S_Critical
{
	private float m_CritRate;
	private float m_CritDmg;

	public float critRate => m_CritRate;
	public float critDmg => m_CritDmg;

	public S_Critical(float rate, float dmg)
	{
		m_CritRate = rate;
		m_CritDmg = dmg;
	}
}