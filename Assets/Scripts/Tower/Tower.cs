using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tower : MonoBehaviour
{
	// 타워 정보(엑셀)
	[SerializeField]
	protected Tower_TableExcel m_TowerInfo_Excel;
	// 타워 정보
	public S_TowerData m_TowerInfo;

	// 타겟
	public Enemy m_Target_Default;
	public Enemy m_Target_Skill01;
	public Enemy m_Target_Skill02;

	public event Action OnLostDefaultTargetEvent;
	public event Action OnLostSkill01TargetEvent;
	public event Action OnLostSkill02TargetEvent;

	#region 내부 컴포넌트
	// 타워 애니메이터
	[SerializeField, ReadOnly]
	protected TowerAnimator m_TowerAnimator;

	[SerializeField]
	protected AttackRange m_AttackRange_Default;
	[SerializeField]
	protected AttackRange m_AttackRange_Skill01;
	[SerializeField]
	protected AttackRange m_AttackRange_Skill02;
	#endregion
	#region 내부 프로퍼티

	#region 매니저
	// 타워 매니져
	protected TowerManager M_Tower => TowerManager.Instance;
	// 스킬 매니져
	protected SkillManager M_Skill => SkillManager.Instance;
	// 적 매니져
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	// 마왕 매니져
	protected DevilManager M_Devil => DevilManager.Instance;
	// 이펙트 매니져
	protected EffectManager M_Effect => EffectManager.Instance;
	#endregion

	// 타워 회전 속도
	protected float RotateSpeed => m_TowerInfo.RotateSpeed * Time.deltaTime;
	// 타겟까지의 거리
	protected float DistanceToTarget_Default => Vector3.Distance(transform.position, m_Target_Default.transform.position);
	protected float DistanceToTarget_Skill01 => Vector3.Distance(transform.position, m_Target_Skill01.transform.position);
	protected float DistanceToTarget_Skill02 => Vector3.Distance(transform.position, m_Target_Skill02.transform.position);
	// 타겟 생존 여부
	protected bool IsTargetDead_Default => null == m_Target_Default || m_Target_Default.IsDead;
	protected bool IsTargetDead_Skill01 => null == m_Target_Skill01 || m_Target_Skill01.IsDead;
	protected bool IsTargetDead_Skill02 => null == m_Target_Skill02 || m_Target_Skill02.IsDead;
	// 타겟 놓쳤는 지
	protected bool LostTarget_Default => m_AttackRange_Default.Range < DistanceToTarget_Default;
	protected bool LostTarget_Skill01 => m_AttackRange_Skill01.Range < DistanceToTarget_Skill01;
	protected bool LostTarget_Skill02 => m_AttackRange_Skill02.Range < DistanceToTarget_Skill02;
	#endregion
	#region 외부 프로퍼티
	public Tower_TableExcel ExcelData => m_TowerInfo_Excel; // cha
	public E_Direction Direction { get => m_TowerInfo.Direction; set => m_TowerInfo.Direction = value; }
	public Node Node { get => m_TowerInfo.node; set => m_TowerInfo.node = value; }
	public bool CanAttack { get => m_TowerInfo.CanAttack; set => m_TowerInfo.CanAttack = value; }
	public string Name => m_TowerInfo_Excel.Name_EN;
	public int TowerCode => m_TowerInfo_Excel.Code;
	public int TowerKind => m_TowerInfo_Excel.Tower_Kinds;
	public int SynergyCode1 => m_TowerInfo_Excel.Type1;
	public int SynergyCode2 => m_TowerInfo_Excel.Type2;
	#endregion

	#region 내부 함수
	// 타워 회전
	protected void RotateToTarget()
	{
		if (!IsTargetDead_Default && !LostTarget_Default)
		{
			RotateToTarget(m_Target_Default);
		}
		else if (!IsTargetDead_Skill01 && !LostTarget_Skill01)
		{
			RotateToTarget(m_Target_Skill01);
		}
		else if (!IsTargetDead_Skill02 && !LostTarget_Skill02)
		{
			RotateToTarget(m_Target_Skill02);
		}
		else
		{
			RotateToTarget(null);
		}
	}
	protected void RotateToTarget(Enemy target)
	{
		// 바라볼 방향
		Vector3 lookingDir = m_TowerInfo.LookingDir;

		// 타겟이 있는 경우
		if (null != target && !target.IsDead)
		{
			// 바라볼 방향 수정
			lookingDir = target.transform.position - transform.position;
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
			switch ((E_TargetType)m_TowerInfo.Condition_Default.Target_type)
			{
				case E_TargetType.CloseTarget:
					{
						m_Target_Default = m_AttackRange_Default.GetNearTarget();

						if (m_TowerInfo.Berserker)
						{
							m_TowerInfo.BerserkerStack = 0;
						}
					}
					break;
				case E_TargetType.RandTarget:
					{
						m_Target_Default = m_AttackRange_Default.GetRandomTarget();

						if (m_TowerInfo.Berserker)
						{
							m_TowerInfo.BerserkerStack = 0;
						}
					}
					break;
				// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
				case E_TargetType.FixTarget:
					{
						if (null == m_Target_Default || // 예외처리
							DistanceToTarget_Default > m_TowerInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
						{
							m_Target_Default = m_AttackRange_Default.GetNearTarget();

							if (m_TowerInfo.Berserker)
							{
								m_TowerInfo.BerserkerStack = 0;
							}
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
		#region 스킬01
		if (IsTargetDead_Skill01 || LostTarget_Skill01)
		{
			if (m_TowerInfo.AttackTimer_Skill01 >= m_TowerInfo.AttackSpeed_Skill01 &&
				m_TowerInfo.CanAttack && null != m_Target_Skill01)
			{
				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type)
				{
					case E_TargetType.CloseTarget:
						{
							m_Target_Skill01 = m_AttackRange_Skill01.GetNearTarget();

							if (m_TowerInfo.Berserker)
							{
								m_TowerInfo.BerserkerStack = 0;
							}
						}
						break;
					case E_TargetType.RandTarget:
						{
							m_Target_Skill01 = m_AttackRange_Skill01.GetRandomTarget();

							if (m_TowerInfo.Berserker)
							{
								m_TowerInfo.BerserkerStack = 0;
							}
						}
						break;
					// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
					case E_TargetType.FixTarget:
						{
							if (null == m_Target_Skill01 || // 예외처리
								DistanceToTarget_Skill01 > m_TowerInfo.Stat_Skill01.Range) // 타겟이 사거리를 벗어난 경우
							{
								m_Target_Skill01 = m_AttackRange_Skill01.GetNearTarget();

								if (m_TowerInfo.Berserker)
								{
									m_TowerInfo.BerserkerStack = 0;
								}
							}
						}
						break;
					case E_TargetType.TileTarget:
						{
							m_Target_Skill01 = m_AttackRange_Skill01.GetNearTarget();
						}
						break;
				}

				if (IsTargetDead_Skill01)
				{
					m_AttackRange_Skill01.RemoveTarget(m_Target_Skill01);
					m_Target_Skill01 = null;
					OnLostSkill01TargetEvent?.Invoke();
				}
			}
		}
		#endregion
		#region 스킬02
		if (IsTargetDead_Skill02 || LostTarget_Skill02)
		{
			if (m_TowerInfo.AttackTimer_Skill02 >= m_TowerInfo.AttackSpeed_Skill02 &&
				m_TowerInfo.CanAttack && null != m_Target_Skill02)
			{
				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type)
				{
					case E_TargetType.CloseTarget:
						{
							m_Target_Skill02 = m_AttackRange_Skill02.GetNearTarget();

							if (m_TowerInfo.Berserker)
							{
								m_TowerInfo.BerserkerStack = 0;
							}
						}
						break;
					case E_TargetType.RandTarget:
						{
							m_Target_Skill02 = m_AttackRange_Skill02.GetRandomTarget();

							if (m_TowerInfo.Berserker)
							{
								m_TowerInfo.BerserkerStack = 0;
							}
						}
						break;
					// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
					case E_TargetType.FixTarget:
						{
							if (null == m_Target_Skill02 || // 예외처리
								DistanceToTarget_Skill02 > m_TowerInfo.Stat_Skill02.Range) // 타겟이 사거리를 벗어난 경우
							{
								m_Target_Skill02 = m_AttackRange_Skill02.GetNearTarget();

								if (m_TowerInfo.Berserker)
								{
									m_TowerInfo.BerserkerStack = 0;
								}
							}
						}
						break;
					case E_TargetType.TileTarget:
						{
							m_Target_Skill02 = m_AttackRange_Skill02.GetNearTarget();
						}
						break;
				}

				if (IsTargetDead_Skill02)
				{
					m_AttackRange_Skill02.RemoveTarget(m_Target_Skill02);
					m_Target_Skill02 = null;
					OnLostSkill02TargetEvent?.Invoke();
				}
			}
		}
		#endregion
	}
	// 타워 공격
	protected void AttackTarget()
	{
		#region 기본 스킬
		// 기본 스킬 타이머
		if (m_TowerInfo.AttackTimer_Default < m_TowerInfo.AttackSpeed_Default)
		{
			m_TowerInfo.AttackTimer_Default += Time.deltaTime;
		}
		// 기본 스킬 공격
		else if (m_TowerInfo.CanAttack && !IsTargetDead_Default && !LostTarget_Default)
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Default -= m_TowerInfo.AttackSpeed_Default;

			// 기본 공격 애니메이션 재생
			SetAttackTrigger();
		}
		#endregion
		#region 스킬01
		// 스킬01 타이머
		if (m_TowerInfo.AttackTimer_Skill01 < m_TowerInfo.AttackSpeed_Skill01)
		{
			m_TowerInfo.AttackTimer_Skill01 += Time.deltaTime;
		}
		// 스킬01 공격
		else if (m_TowerInfo.CanAttack && !IsTargetDead_Skill01 && !LostTarget_Skill01)
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Skill01 -= m_TowerInfo.AttackSpeed_Skill01;

			// 스킬01 애니메이션 재생
			SetSkill01Trigger();
		}
		#endregion
		#region 스킬02
		// 스킬02 타이머
		if (m_TowerInfo.AttackTimer_Skill02 < m_TowerInfo.AttackSpeed_Skill02)
		{
			m_TowerInfo.AttackTimer_Skill02 += Time.deltaTime;
		}
		// 스킬02 공격
		else if (m_TowerInfo.CanAttack && !IsTargetDead_Skill02 && !LostTarget_Skill02)
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Skill02 -= m_TowerInfo.AttackSpeed_Skill02;

			// 스킬02 애니메이션 재생
			SetSkill02Trigger();
		}
		#endregion
	}

	protected void SetAttackTrigger()
	{
		m_TowerAnimator.SetTrigger("Attack");
	}
	protected void SetSkill01Trigger()
	{
		m_TowerAnimator.SetTrigger("Skill01");
	}
	protected void SetSkill02Trigger()
	{
		m_TowerAnimator.SetTrigger("Skill02");
	}
	#endregion
	#region 외부 함수
	// 타워 초기화
	public void InitializeTower(int code)
	{
		#region 엑셀 데이터 정리
		m_TowerInfo_Excel = M_Tower.GetData(code);
		#endregion

		#region 내부 데이터 정리
		m_TowerInfo.RotateSpeed = 5f;
		m_TowerInfo.CanAttack = false;

		// null 병합 연산자 안되는 이유
		// https://overworks.github.io/unity/2019/07/22/null-of-unity-object-part-2.html
		// m_TowerInfo.AttackPivot ??= transform.Find("Mesh").GetChild("AttackPivot");
		// 공격 피벗
		if (m_TowerInfo.AttackPivot == null)
		{
			m_TowerInfo.AttackPivot = transform.Find("Mesh").GetChild("AttackPivot");
		}

		// 기본 스킬 데이터
		m_TowerInfo.Condition_Default = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
		m_TowerInfo.Stat_Default = M_Skill.GetStatData(m_TowerInfo.Condition_Default.PassiveCode);
		// 기본 스킬
		m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
		m_TowerInfo.AttackTimer_Default = m_TowerInfo.Stat_Default.CoolTime;

		// 스킬1 데이터
		m_TowerInfo.Condition_Skill01 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
		m_TowerInfo.Stat_Skill01 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill01.PassiveCode);
		// 스킬1
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
		m_TowerInfo.AttackTimer_Skill01 = 0f;

		// 스킬2 데이터
		m_TowerInfo.Condition_Skill02 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
		m_TowerInfo.Stat_Skill02 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill02.PassiveCode);
		// 스킬2
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
		m_TowerInfo.AttackTimer_Skill02 = 0f;

		// 시너지
		m_TowerInfo.Synergy_Atk_type = E_AttackType.None;

		//m_TowerInfo.BuffList ??= new List<BuffCC_TableExcel>();
		if (m_TowerInfo.BuffList == null)
			m_TowerInfo.BuffList = new List<BuffCC_TableExcel>();
		else if (m_TowerInfo.BuffList.Count > 0)
			m_TowerInfo.BuffList.Clear();

		//m_TowerInfo.BerserkerBuffList ??= new List<BuffCC_TableExcel>();
		if (m_TowerInfo.BerserkerBuffList == null)
			m_TowerInfo.BerserkerBuffList = new List<BuffCC_TableExcel>();
		else if (m_TowerInfo.BerserkerBuffList.Count > 0)
			m_TowerInfo.BerserkerBuffList.Clear();

		// 마왕 스킬
		//m_TowerInfo.DevilSkillBuffList ??= new List<BuffCC_TableExcel>();
		if (m_TowerInfo.DevilSkillBuffList == null)
			m_TowerInfo.DevilSkillBuffList = new List<BuffCC_TableExcel>();
		else if (m_TowerInfo.DevilSkillBuffList.Count > 0)
			m_TowerInfo.DevilSkillBuffList.Clear();
		#endregion

		#region 내부 컴포넌트
		// m_TowerAnimator ??= GetComponentInChildren<TowerAnimator>(true);
		if (null == m_TowerAnimator)
		{
			m_TowerAnimator = GetComponentInChildren<TowerAnimator>(true);
			m_TowerAnimator.Initialize(this);
		}
		m_TowerAnimator.SetFloat("Attack", m_TowerInfo_Excel.Atk_Speed);

		// m_AttackRange_Default ??= transform.Find("AttackRange_Default").AddComponent<AttackRange>();
		if (null == m_AttackRange_Default)
		{
			m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
			m_AttackRange_Default.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Default.Initialize();
		}
		m_AttackRange_Default.Range = m_TowerInfo.Stat_Default.Range;

		//m_AttackRange_Skill01 ??= transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
		if (m_AttackRange_Skill01 == null)
		{
			m_AttackRange_Skill01 = transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
			m_AttackRange_Skill01.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill01.Initialize();
		}
		m_AttackRange_Skill01.Range = m_TowerInfo.Stat_Skill01.Range;

		//m_AttackRange_Skill02 ??= transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
		if (m_AttackRange_Skill02 == null)
		{
			m_AttackRange_Skill02 = transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
			m_AttackRange_Skill02.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill02.Initialize();
		}
		m_AttackRange_Skill02.Range = m_TowerInfo.Stat_Skill02.Range;
		#endregion
	}
	public void FinializeTower()
	{
		m_TowerInfo.node?.ClearNode();

		m_TowerInfo.BuffList?.Clear();
		m_TowerInfo.BerserkerBuffList?.Clear();
		m_TowerInfo.DevilSkillBuffList?.Clear();

		m_AttackRange_Default?.Clear();
		m_AttackRange_Skill01?.Clear();
		m_AttackRange_Skill02?.Clear();
	}

	public void CallAttack()
	{
		if (null == m_Target_Default)
		{
			return;
		}

		m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;

		// 기본 스킬 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Default;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Default;

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}

		// 기본 대미지 설정
		statData.Dmg *= m_TowerInfo_Excel.Atk;
		statData.Dmg += statData.Dmg_plus;

		#region 버프
		// 적용할 버프 리스트
		List<BuffCC_TableExcel> BuffList;

		// 시너지 버프
		BuffList = m_TowerInfo.BuffList;
		// 버프 적용 확률
		List<float> BuffRand = new List<float>();
		// 버프 적용 여부
		List<bool> BuffApply = new List<bool>();
		// 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 버서커 버프
		BuffList = m_TowerInfo.BerserkerBuffList;
		// 버서커 버프 적용 확률
		List<float> BerserkerBuffRand = new List<float>();
		// 버서커 버프 적용 여부
		List<bool> BerserkerBuffApply = new List<bool>();
		// 버서커 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 마왕 스킬 버프
		BuffList = m_TowerInfo.DevilSkillBuffList;
		// 마왕 스킬 버프 적용 확률
		List<float> DevilSkillBuffRand = new List<float>();
		// 마왕 스킬 버프 적용 여부
		List<bool> DevilSkillBuffApply = new List<bool>();
		// 마왕 스킬 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 적용할 버프 담아둘 변수
		S_Buff buff;

		// 버프 적용
		#region 버프 합연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 합연산
				if (buff.AddType == E_AddType.Fix)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg += BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range += BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							m_TowerInfo.AttackSpeed_Default -= BuffAmount;
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								m_TowerInfo.AttackSpeed_Default -= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									m_TowerInfo.AttackSpeed_Default -= BuffAmount;
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 합연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								m_TowerInfo.AttackSpeed_Default -= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									m_TowerInfo.AttackSpeed_Default -= BuffAmount;
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 합연산
							if (buff.AddType == E_AddType.Fix)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg += BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range += BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										m_TowerInfo.AttackSpeed_Default -= BuffAmount;
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버프 곱연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							m_TowerInfo.AttackSpeed_Default *= BuffAmount;
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								m_TowerInfo.AttackSpeed_Default *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									m_TowerInfo.AttackSpeed_Default *= BuffAmount;
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 곱연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								m_TowerInfo.AttackSpeed_Default *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									m_TowerInfo.AttackSpeed_Default *= BuffAmount;
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 곱연산
							if (buff.AddType == E_AddType.Percent)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg *= BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range *= BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										m_TowerInfo.AttackSpeed_Default *= BuffAmount;
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		BuffList = m_TowerInfo.DevilSkillBuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 마왕 스킬 버프1 체크
			if (DevilSkillBuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 마왕 스킬 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							m_TowerInfo.AttackSpeed_Default *= BuffAmount;
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 마왕 스킬 버프2 체크
				if (DevilSkillBuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 마왕 스킬 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								m_TowerInfo.AttackSpeed_Default *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 마왕 스킬 버프3 체크
					if (DevilSkillBuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 마왕 스킬 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									m_TowerInfo.AttackSpeed_Default *= BuffAmount;
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion

		// 사거리 업데이트
		m_AttackRange_Default.Range = statData.Range;
		#endregion

		#region 시너지
		// 버서커
		if (m_TowerInfo.Berserker)
		{
			// 버서커 스택 증가
			if (m_TowerInfo.BerserkerStack < m_TowerInfo.BerserkerMaxStack)
			{
				++m_TowerInfo.BerserkerStack;
			}
		}

		// 공격 타입 변경
		if (m_TowerInfo.Synergy_Atk_type != E_AttackType.None)
		{
			conditionData.Atk_type = (int)m_TowerInfo.Synergy_Atk_type;
			statData.Target_num = m_TowerInfo.BounceCount;
		}

		// 마왕 쿨타임 감소
		if (m_TowerInfo.ReduceCooldown)
		{
			float ReduceCooldownRand = Random.Range(0.00001f, 1f);
			bool ReduceCooldownApply = (ReduceCooldownRand <= m_TowerInfo.ReduceCooldownRand);

			if (ReduceCooldownApply)
			{
				M_Devil.Devil.ReduceSkill01Cooldown(1f);
				M_Devil.Devil.ReduceSkill02Cooldown(1f);
			}
		}
		#endregion

		// 기본 스킬 투사체 생성
		int DefaultSkillCode = conditionData.projectile_prefab;
		if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			for (int i = 0; i < EnemyList.Count; ++i)
			{
				Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

				switch ((E_FireType)conditionData.Atk_pick)
				{
					case E_FireType.Select_point:
						break;
					case E_FireType.Select_self:
						DefaultSkill.transform.position = m_TowerInfo.AttackPivot.position;
						break;
					case E_FireType.Select_enemy:
						DefaultSkill.transform.position = EnemyList[i].HitPivot.position;
						break;
				}

				DefaultSkill.enabled = true;
				DefaultSkill.gameObject.SetActive(true);

				// 기본 스킬 데이터 설정
				DefaultSkill.InitializeSkill(EnemyList[i], conditionData, statData);
			}
		}
		else
		{
			Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					DefaultSkill.transform.position = m_TowerInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					DefaultSkill.transform.position = m_Target_Default.HitPivot.position;
					break;
			}

			DefaultSkill.enabled = true;
			DefaultSkill.gameObject.SetActive(true);

			// 기본 스킬 데이터 설정
			DefaultSkill.InitializeSkill(m_Target_Default, conditionData, statData);
		}
	}
	public void CallSkill01()
	{
		if (null == m_Target_Skill01)
		{
			return;
		}

		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;

		// 스킬01 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill01;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill01;

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}

		// 기본 대미지 설정
		statData.Dmg *= m_TowerInfo_Excel.Atk;
		statData.Dmg += statData.Dmg_plus;

		#region 버프
		// 적용할 버프 리스트
		List<BuffCC_TableExcel> BuffList;

		// 시너지 버프
		BuffList = m_TowerInfo.BuffList;
		// 버프 적용 확률
		List<float> BuffRand = new List<float>();
		// 버프 적용 여부
		List<bool> BuffApply = new List<bool>();
		// 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 버서커 버프
		BuffList = m_TowerInfo.BerserkerBuffList;
		// 버서커 버프 적용 확률
		List<float> BerserkerBuffRand = new List<float>();
		// 버서커 버프 적용 여부
		List<bool> BerserkerBuffApply = new List<bool>();
		// 버서커 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 마왕 스킬 버프
		BuffList = m_TowerInfo.DevilSkillBuffList;
		// 마왕 스킬 버프 적용 확률
		List<float> DevilSkillBuffRand = new List<float>();
		// 마왕 스킬 버프 적용 여부
		List<bool> DevilSkillBuffApply = new List<bool>();
		// 마왕 스킬 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 적용할 버프 담아둘 변수
		S_Buff buff;

		// 버프 적용
		#region 버프 합연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 합연산
				if (buff.AddType == E_AddType.Fix)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg += BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range += BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 합연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 합연산
							if (buff.AddType == E_AddType.Fix)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg += BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range += BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버프 곱연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 곱연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 곱연산
							if (buff.AddType == E_AddType.Percent)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg *= BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range *= BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		BuffList = m_TowerInfo.DevilSkillBuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 마왕 스킬 버프1 체크
			if (DevilSkillBuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 마왕 스킬 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 마왕 스킬 버프2 체크
				if (DevilSkillBuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 마왕 스킬 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 마왕 스킬 버프3 체크
					if (DevilSkillBuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 마왕 스킬 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion

		// 사거리 업데이트
		m_AttackRange_Skill01.Range = statData.Range;
		#endregion

		// 스킬01 투사체 생성
		int Skill01Code = conditionData.projectile_prefab;

		if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			for (int i = 0; i < EnemyList.Count; ++i)
			{
				Skill Skill01 = M_Skill.SpawnProjectileSkill(Skill01Code);

				switch ((E_FireType)conditionData.Atk_pick)
				{
					case E_FireType.Select_point:
						break;
					case E_FireType.Select_self:
						Skill01.transform.position = m_TowerInfo.AttackPivot.position;
						break;
					case E_FireType.Select_enemy:
						Skill01.transform.position = EnemyList[i].HitPivot.position;
						break;
				}

				Skill01.enabled = true;
				Skill01.gameObject.SetActive(true);

				// 스킬01 데이터 설정
				Skill01.InitializeSkill(EnemyList[i], conditionData, statData);
			}
		}
		else
		{
			Skill Skill01 = M_Skill.SpawnProjectileSkill(Skill01Code);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					Skill01.transform.position = m_TowerInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					Skill01.transform.position = m_Target_Skill01.HitPivot.position;
					break;
			}

			Skill01.enabled = true;
			Skill01.gameObject.SetActive(true);

			// 스킬01 데이터 설정
			Skill01.InitializeSkill(m_Target_Default, conditionData, statData);
		}
	}
	public void CallSkill02()
	{
		if (null == m_Target_Skill02)
		{
			return;
		}

		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;

		// 스킬02 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill02;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill02;

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}

		// 기본 대미지 설정
		statData.Dmg *= m_TowerInfo_Excel.Atk;
		statData.Dmg += statData.Dmg_plus;

		#region 버프
		// 적용할 버프 리스트
		List<BuffCC_TableExcel> BuffList;

		// 시너지 버프
		BuffList = m_TowerInfo.BuffList;
		// 버프 적용 확률
		List<float> BuffRand = new List<float>();
		// 버프 적용 여부
		List<bool> BuffApply = new List<bool>();
		// 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BuffRand.Add(Random.Range(0.00001f, 1f));
			BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 버서커 버프
		BuffList = m_TowerInfo.BerserkerBuffList;
		// 버서커 버프 적용 확률
		List<float> BerserkerBuffRand = new List<float>();
		// 버서커 버프 적용 여부
		List<bool> BerserkerBuffApply = new List<bool>();
		// 버서커 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
			BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 마왕 스킬 버프
		BuffList = m_TowerInfo.DevilSkillBuffList;
		// 마왕 스킬 버프 적용 확률
		List<float> DevilSkillBuffRand = new List<float>();
		// 마왕 스킬 버프 적용 여부
		List<bool> DevilSkillBuffApply = new List<bool>();
		// 마왕 스킬 버프 적용 계산
		for (int i = 0; i < BuffList.Count; ++i)
		{
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
			DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
			DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
		}

		// 적용할 버프 담아둘 변수
		S_Buff buff;

		// 버프 적용
		#region 버프 합연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 합연산
				if (buff.AddType == E_AddType.Fix)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg += BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range += BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 합연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 합연산
					if (buff.AddType == E_AddType.Fix)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg += BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 합연산
						if (buff.AddType == E_AddType.Fix)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg += BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range += BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 합연산
							if (buff.AddType == E_AddType.Fix)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg += BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range += BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버프 곱연산
		BuffList = m_TowerInfo.BuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 버프1 체크
			if (BuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 버프2 체크
				if (BuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버프3 체크
					if (BuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region 버서커 버프 곱연산
		BuffList = m_TowerInfo.BerserkerBuffList;
		if (m_TowerInfo.Berserker)
		{
			for (int i = 0; i < BuffList.Count; ++i)
			{
				// 버서커 버프1 체크
				if (BerserkerBuffApply[i * 3])
				{
					buff = new S_Buff(
						BuffList[i].BuffType1,
						BuffList[i].AddType1,
						BuffList[i].BuffAmount1,
						BuffList[i].BuffRand1,
						BuffList[i].Summon1
						);
					float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

					// 버서커 버프1 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 버서커 버프2 체크
					if (BerserkerBuffApply[i * 3 + 1])
					{
						buff = new S_Buff(
							BuffList[i].BuffType2,
							BuffList[i].AddType2,
							BuffList[i].BuffAmount2,
							BuffList[i].BuffRand2,
							BuffList[i].Summon2
							);
						BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

						// 버서커 버프2 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}

						// 버서커 버프3 체크
						if (BerserkerBuffApply[i * 3 + 2])
						{
							buff = new S_Buff(
								BuffList[i].BuffType3,
								BuffList[i].AddType3,
								BuffList[i].BuffAmount3,
								BuffList[i].BuffRand3,
								BuffList[i].Summon3
								);
							BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

							// 버서커 버프3 곱연산
							if (buff.AddType == E_AddType.Percent)
							{
								switch (buff.BuffType)
								{
									case E_BuffType.Atk:
										statData.Dmg *= BuffAmount;
										break;
									case E_BuffType.Range:
										statData.Range *= BuffAmount;
										break;
									case E_BuffType.Atk_spd:
										break;
									case E_BuffType.Crit_rate:
										break;
									case E_BuffType.Crit_Dmg:
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		BuffList = m_TowerInfo.DevilSkillBuffList;
		for (int i = 0; i < BuffList.Count; ++i)
		{
			// 마왕 스킬 버프1 체크
			if (DevilSkillBuffApply[i * 3])
			{
				buff = new S_Buff(
					BuffList[i].BuffType1,
					BuffList[i].AddType1,
					BuffList[i].BuffAmount1,
					BuffList[i].BuffRand1,
					BuffList[i].Summon1
					);
				float BuffAmount = buff.BuffAmount;

				// 마왕 스킬 버프1 곱연산
				if (buff.AddType == E_AddType.Percent)
				{
					switch (buff.BuffType)
					{
						case E_BuffType.Atk:
							statData.Dmg *= BuffAmount;
							break;
						case E_BuffType.Range:
							statData.Range *= BuffAmount;
							break;
						case E_BuffType.Atk_spd:
							break;
						case E_BuffType.Crit_rate:
							break;
						case E_BuffType.Crit_Dmg:
							break;
					}
				}

				// 마왕 스킬 버프2 체크
				if (DevilSkillBuffApply[i * 3 + 1])
				{
					buff = new S_Buff(
						BuffList[i].BuffType2,
						BuffList[i].AddType2,
						BuffList[i].BuffAmount2,
						BuffList[i].BuffRand2,
						BuffList[i].Summon2
						);
					BuffAmount = buff.BuffAmount;

					// 마왕 스킬 버프2 곱연산
					if (buff.AddType == E_AddType.Percent)
					{
						switch (buff.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg *= BuffAmount;
								break;
							case E_BuffType.Range:
								statData.Range *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								break;
							case E_BuffType.Crit_rate:
								break;
							case E_BuffType.Crit_Dmg:
								break;
						}
					}

					// 마왕 스킬 버프3 체크
					if (DevilSkillBuffApply[i * 3 + 2])
					{
						buff = new S_Buff(
							BuffList[i].BuffType3,
							BuffList[i].AddType3,
							BuffList[i].BuffAmount3,
							BuffList[i].BuffRand3,
							BuffList[i].Summon3
							);
						BuffAmount = buff.BuffAmount;

						// 마왕 스킬 버프3 곱연산
						if (buff.AddType == E_AddType.Percent)
						{
							switch (buff.BuffType)
							{
								case E_BuffType.Atk:
									statData.Dmg *= BuffAmount;
									break;
								case E_BuffType.Range:
									statData.Range *= BuffAmount;
									break;
								case E_BuffType.Atk_spd:
									break;
								case E_BuffType.Crit_rate:
									break;
								case E_BuffType.Crit_Dmg:
									break;
							}
						}
					}
				}
			}
		}
		#endregion

		// 사거리 업데이트
		m_AttackRange_Skill02.Range = statData.Range;
		#endregion

		// 스킬02 투사체 생성
		int Skill02Code = conditionData.projectile_prefab;

		if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			for (int i = 0; i < EnemyList.Count; ++i)
			{
				Skill Skill02 = M_Skill.SpawnProjectileSkill(Skill02Code);

				switch ((E_FireType)conditionData.Atk_pick)
				{
					case E_FireType.Select_point:
						break;
					case E_FireType.Select_self:
						Skill02.transform.position = m_TowerInfo.AttackPivot.position;
						break;
					case E_FireType.Select_enemy:
						Skill02.transform.position = EnemyList[i].HitPivot.position;
						break;
				}

				Skill02.enabled = true;
				Skill02.gameObject.SetActive(true);

				// 스킬02 데이터 설정
				Skill02.InitializeSkill(EnemyList[i], conditionData, statData);
			}
		}
		else
		{
			Skill Skill02 = M_Skill.SpawnProjectileSkill(Skill02Code);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					Skill02.transform.position = m_TowerInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					Skill02.transform.position = m_Target_Skill02.HitPivot.position;
					break;
			}

			Skill02.enabled = true;
			Skill02.gameObject.SetActive(true);

			// 스킬02 데이터 설정
			Skill02.InitializeSkill(m_Target_Skill02, conditionData, statData);
		}
	}
	#endregion
	#region 유니티 콜백 함수
	private void OnApplicationQuit()
	{
		FinializeTower();
	}

	private void Update()
	{
		UpdateTarget();
		RotateToTarget();
		AttackTarget();
	}
	#endregion

	// 타워 정보
	[System.Serializable]
	public struct S_TowerData
	{
		// cha
		// if this tower is located in Inventory => true
		// in NODE => false
		public bool IsOnInventory;
		// 노드
		public Node node;
		// 타워 방향
		public E_Direction Direction;
		// 회전 속도
		public float RotateSpeed;
		// 초기 바라볼 방향
		public Vector3 LookingDir;
		// 공격 피벗
		public Transform AttackPivot;
		// 공격 가능 여부 (노드 회전)
		public bool CanAttack;

		#region 기본 스킬
		// 기본 스킬 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;
		#endregion
		#region 스킬01
		// 스킬01 데이터
		public SkillCondition_TableExcel Condition_Skill01;
		public SkillStat_TableExcel Stat_Skill01;
		// 스킬01 공격 속도
		public float AttackSpeed_Skill01;
		// 스킬01 타이머
		public float AttackTimer_Skill01;
		#endregion
		#region 스킬02
		// 스킬02 데이터
		public SkillCondition_TableExcel Condition_Skill02;
		public SkillStat_TableExcel Stat_Skill02;
		// 스킬02 공격 속도
		public float AttackSpeed_Skill02;
		// 스킬02 타이머
		public float AttackTimer_Skill02;
		#endregion

		#region 시너지
		// 버프
		public List<BuffCC_TableExcel> BuffList;

		// 공격 타입 변경
		public E_AttackType Synergy_Atk_type;
		public int BounceCount;

		// 버서커
		public bool Berserker;
		public int BerserkerStack;
		public int BerserkerMaxStack;
		public List<BuffCC_TableExcel> BerserkerBuffList;

		// 마왕 쿨타임 감소
		public bool ReduceCooldown;
		public float ReduceCooldownRand;
		#endregion

		// 마왕 스킬 버프
		public List<BuffCC_TableExcel> DevilSkillBuffList;
	}
}
