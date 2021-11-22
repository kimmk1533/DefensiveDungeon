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

	[Space(10)]

	// 타워 정보
	[SerializeField]
	protected S_TowerData m_TowerInfo;

	[Space(10)]

	// 타겟
	[SerializeField]
	protected Enemy m_Target_Default;
	[SerializeField]
	protected Enemy m_Target_Skill01;
	[SerializeField]
	protected Enemy m_Target_Skill02;

	public event Action OnLostDefaultTargetEvent;
	public event Action OnLostSkill01TargetEvent;
	public event Action OnLostSkill02TargetEvent;

	#region 내부 컴포넌트
	// 타워 애니메이터
	[SerializeField, ReadOnly]
	protected TowerAnimator m_TowerAnimator;

	protected AttackRange m_AttackRange_Default;
	protected AttackRange m_AttackRange_Skill01;
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
	// 시너지 매니져
	protected SynergyManager M_Synergy => SynergyManager.Instance;
	// 버프 매니져
	protected BuffManager M_Buff => BuffManager.Instance;
	// 노드 매니저
	protected NodeManager M_Node => NodeManager.Instance;
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
	// 공격 타이머 확인
	protected bool CheckAttackTimer_Default => m_TowerInfo.AttackTimer_Default >= m_TowerInfo.AttackSpeed_Default;
	protected bool CheckAttackTimer_Skill01 => m_TowerInfo.AttackTimer_Skill01 >= m_TowerInfo.AttackSpeed_Skill01;
	protected bool CheckAttackTimer_Skill02 => m_TowerInfo.AttackTimer_Skill02 >= m_TowerInfo.AttackSpeed_Skill02;
	#endregion
	#region 외부 프로퍼티
	public Tower_TableExcel ExcelData => m_TowerInfo_Excel; // cha

	public E_Direction Direction
	{
		get => m_TowerInfo.Direction;
		set
		{
			m_TowerInfo.Direction = value;
			m_AttackRange_Default.Direction = value;
			m_AttackRange_Skill01.Direction = value;
			m_AttackRange_Skill02.Direction = value;
		}
	}
	public Vector3 LookingDir { get => m_TowerInfo.LookingDir; set => m_TowerInfo.LookingDir = value; }
	public Node Node { get => m_TowerInfo.node; set => m_TowerInfo.node = value; }
	public bool CanAttack_Node { get => m_TowerInfo.CanAttack_Node; set => m_TowerInfo.CanAttack_Node = value; }
	public bool CanAttack_Skill { get => m_TowerInfo.CanAttack_Skill; set => m_TowerInfo.CanAttack_Skill = value; }
	public bool IsOnInventory { get => m_TowerInfo.IsOnInventory; set => m_TowerInfo.IsOnInventory = value; }

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
		else if (!IsTargetDead_Skill01 && !LostTarget_Skill01 &&
			CheckAttackTimer_Skill01)
		{
			RotateToTarget(m_Target_Skill01);
		}
		else if (!IsTargetDead_Skill02 && !LostTarget_Skill02 &&
			CheckAttackTimer_Skill02)
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
		if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type != E_TargetType.TileTarget)
		{
			if (IsTargetDead_Default || LostTarget_Default)
			{
				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Default_Origin.Target_type)
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
								DistanceToTarget_Default > m_TowerInfo.Stat_Default_Origin.Range) // 타겟이 사거리를 벗어난 경우
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
		}
		#endregion
		#region 스킬01
		if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type != E_TargetType.TileTarget)
		{
			if (IsTargetDead_Skill01 || LostTarget_Skill01)
			{
				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Skill01_Origin.Target_type)
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
								DistanceToTarget_Skill01 > m_TowerInfo.Stat_Skill01_Origin.Range) // 타겟이 사거리를 벗어난 경우
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
		if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type != E_TargetType.TileTarget)
		{
			if (IsTargetDead_Skill02 || LostTarget_Skill02)
			{
				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Skill02_Origin.Target_type)
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
								DistanceToTarget_Skill02 > m_TowerInfo.Stat_Skill02_Origin.Range) // 타겟이 사거리를 벗어난 경우
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
	// 타워 공격 타이머
	protected void UpdateAttackTimer()
	{
		// 스킬01 타이머
		if (m_TowerInfo.AttackTimer_Skill01 < m_TowerInfo.AttackSpeed_Skill01)
		{
			m_TowerInfo.AttackTimer_Skill01 += Time.deltaTime;
		}

		// 스킬02 타이머
		if (m_TowerInfo.AttackTimer_Skill02 < m_TowerInfo.AttackSpeed_Skill02)
		{
			m_TowerInfo.AttackTimer_Skill02 += Time.deltaTime;
		}

		// 기본 스킬 타이머
		if (m_TowerInfo.AttackTimer_Default < m_TowerInfo.AttackSpeed_Default)
		{
			m_TowerInfo.AttackTimer_Default += Time.deltaTime;
		}
	}
	// 타워 공격
	protected void AttackTarget()
	{
		#region 스킬01
		// 스킬01 공격
		if (CheckAttackTimer_Skill01 &&
			m_TowerInfo.CanAttack_Node && m_TowerInfo.CanAttack_Skill)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget ||
				!(IsTargetDead_Skill01 || LostTarget_Skill01))
			{
				// 내부 데이터 정리
				m_TowerInfo.AttackTimer_Skill01 -= m_TowerInfo.AttackSpeed_Skill01;
				m_TowerInfo.CanAttack_Skill = false;

				// 스킬01 애니메이션 재생
				SetSkill01Trigger();
				return;
			}
		}
		#endregion

		#region 스킬02
		// 스킬02 공격
		if (CheckAttackTimer_Skill02 &&
			m_TowerInfo.CanAttack_Node && m_TowerInfo.CanAttack_Skill)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget ||
				!(IsTargetDead_Skill02 || LostTarget_Skill02))
			{
				// 내부 데이터 정리
				m_TowerInfo.AttackTimer_Skill02 -= m_TowerInfo.AttackSpeed_Skill02;
				m_TowerInfo.CanAttack_Skill = false;

				// 스킬02 애니메이션 재생
				SetSkill02Trigger();
				return;
			}
		}
		#endregion

		#region 기본 스킬
		// 기본 스킬 공격
		if (CheckAttackTimer_Default &&
			m_TowerInfo.CanAttack_Node && m_TowerInfo.CanAttack_Skill)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type == E_TargetType.TileTarget ||
				!(IsTargetDead_Default || LostTarget_Default))
			{
				// 내부 데이터 정리
				m_TowerInfo.AttackTimer_Default -= m_TowerInfo.AttackSpeed_Default;
				m_TowerInfo.CanAttack_Skill = false;

				// 기본 공격 애니메이션 재생
				SetAttackTrigger();
				return;
			}
		}
		#endregion
	}

	protected void ClearSynergyBuff()
	{
		m_TowerInfo.Crit_Rate_Percent = m_TowerInfo_Excel.Crit_rate;
		m_TowerInfo.Crit_Dmg_Percent = m_TowerInfo_Excel.Crit_Dmg;

		// 기본 스킬 데이터 초기화
		m_TowerInfo.Condition_Default = m_TowerInfo.Condition_Default_Origin;
		m_TowerInfo.Stat_Default = m_TowerInfo.Stat_Default_Origin;
		m_TowerInfo.Stat_Default.Dmg_Fix += m_TowerInfo_Excel.Atk;
		// 스킬01 데이터 초기화
		m_TowerInfo.Condition_Skill01 = m_TowerInfo.Condition_Skill01_Origin;
		m_TowerInfo.Stat_Skill01 = m_TowerInfo.Stat_Skill01_Origin;
		m_TowerInfo.Stat_Skill01.Dmg_Fix += m_TowerInfo_Excel.Atk;
		// 스킬02 데이터 초기화
		m_TowerInfo.Condition_Skill02 = m_TowerInfo.Condition_Skill02_Origin;
		m_TowerInfo.Stat_Skill02 = m_TowerInfo.Stat_Skill02_Origin;
		m_TowerInfo.Stat_Skill02.Dmg_Fix += m_TowerInfo_Excel.Atk;

		m_TowerInfo.SynergyList.Clear();
		m_TowerInfo.BuffList_Fix.Clear();
		m_TowerInfo.BuffList_Percent.Clear();

		m_TowerInfo.Berserker = false;
		m_TowerInfo.BerserkerStack = 0;
		m_TowerInfo.BerserkerMaxStack = 0;

		m_TowerInfo.ReduceCooldown = false;
		m_TowerInfo.ReduceCooldownSec = 0f;
	}
	protected void UpdateSynergyBuff()
	{
		// 시너지 적용
		foreach (var item in m_TowerInfo.SynergyList)
		{
			float synergyRand = Random.Range(0.00001f, 1f);
			bool synergyApply = synergyRand <= item.EffectRand1;

			if (synergyApply)
			{
				switch ((E_SynergyEffectType)item.EffectType1)
				{
					case E_SynergyEffectType.Buff:
						{
							BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1
									);

								if (buff.AddType == E_AddType.Fix)
									m_TowerInfo.BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									m_TowerInfo.BuffList_Percent.Add(buff);

								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2
										);

									if (buff.AddType == E_AddType.Fix)
										m_TowerInfo.BuffList_Fix.Add(buff);
									else if (buff.AddType == E_AddType.Percent)
										m_TowerInfo.BuffList_Percent.Add(buff);

									float buffRand3 = Random.Range(0.00001f, 1f);
									bool buffApply3 = buffRand3 <= buffData.BuffRand3;

									if (buffApply3)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_3",
											buffData.BuffType3,
											buffData.AddType3,
											buffData.BuffAmount3
											);

										if (buff.AddType == E_AddType.Fix)
											m_TowerInfo.BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											m_TowerInfo.BuffList_Percent.Add(buff);
									}
								}
							}
						}
						break;
					case E_SynergyEffectType.ChangeAtkType:
						{
							m_TowerInfo.Condition_Default.Atk_type = item.EffectChange1;

							if ((E_AttackType)item.EffectChange1 == E_AttackType.BounceFire)
							{
								m_TowerInfo.Stat_Default.Target_num = item.EffectReq1;
							}
						}
						break;
					case E_SynergyEffectType.ReduceCooldown:
						{
							m_TowerInfo.ReduceCooldown = true;
							m_TowerInfo.ReduceCooldownSec = 1f;
						}
						break;
					case E_SynergyEffectType.Berserker:
						{
							if (!m_TowerInfo.Berserker)
							{
								m_TowerInfo.Berserker = true;
								m_TowerInfo.BerserkerMaxStack = item.EffectReq1;
							}

							BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1
									);

								if (buff.AddType == E_AddType.Fix)
									m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									m_TowerInfo.BerserkerBuffList_Percent.Add(buff);

								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2
										);

									if (buff.AddType == E_AddType.Fix)
										m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
									else if (buff.AddType == E_AddType.Percent)
										m_TowerInfo.BerserkerBuffList_Percent.Add(buff);

									float buffRand3 = Random.Range(0.00001f, 1f);
									bool buffApply3 = buffRand3 <= buffData.BuffRand3;

									if (buffApply3)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_3",
											buffData.BuffType3,
											buffData.AddType3,
											buffData.BuffAmount3
											);

										if (buff.AddType == E_AddType.Fix)
											m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											m_TowerInfo.BerserkerBuffList_Percent.Add(buff);
									}
								}
							}
						}
						break;
				}
			}

			synergyRand = Random.Range(0.00001f, 1f);
			synergyApply = synergyRand <= item.EffectRand2;

			if (synergyApply)
			{
				switch ((E_SynergyEffectType)item.EffectType2)
				{
					case E_SynergyEffectType.Buff:
						{
							BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1
									);

								if (buff.AddType == E_AddType.Fix)
									m_TowerInfo.BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									m_TowerInfo.BuffList_Percent.Add(buff);

								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2
										);

									if (buff.AddType == E_AddType.Fix)
										m_TowerInfo.BuffList_Fix.Add(buff);
									else if (buff.AddType == E_AddType.Percent)
										m_TowerInfo.BuffList_Percent.Add(buff);

									float buffRand3 = Random.Range(0.00001f, 1f);
									bool buffApply3 = buffRand3 <= buffData.BuffRand3;

									if (buffApply3)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_3",
											buffData.BuffType3,
											buffData.AddType3,
											buffData.BuffAmount3
											);

										if (buff.AddType == E_AddType.Fix)
											m_TowerInfo.BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											m_TowerInfo.BuffList_Percent.Add(buff);
									}
								}
							}
						}
						break;
					case E_SynergyEffectType.ChangeAtkType:
						{
							m_TowerInfo.Condition_Default.Atk_type = item.EffectChange2;

							if ((E_AttackType)item.EffectChange2 == E_AttackType.BounceFire)
							{
								m_TowerInfo.Stat_Default.Target_num = item.EffectReq2;
							}
						}
						break;
					case E_SynergyEffectType.ReduceCooldown:
						{
							m_TowerInfo.ReduceCooldown = true;
							m_TowerInfo.ReduceCooldownSec = 1f;
						}
						break;
					case E_SynergyEffectType.Berserker:
						{
							if (!m_TowerInfo.Berserker)
							{
								m_TowerInfo.Berserker = true;
								m_TowerInfo.BerserkerMaxStack = item.EffectReq2;
							}

							BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1
									);

								if (buff.AddType == E_AddType.Fix)
									m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									m_TowerInfo.BerserkerBuffList_Percent.Add(buff);

								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2
										);

									if (buff.AddType == E_AddType.Fix)
										m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
									else if (buff.AddType == E_AddType.Percent)
										m_TowerInfo.BerserkerBuffList_Percent.Add(buff);

									float buffRand3 = Random.Range(0.00001f, 1f);
									bool buffApply3 = buffRand3 <= buffData.BuffRand3;

									if (buffApply3)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_3",
											buffData.BuffType3,
											buffData.AddType3,
											buffData.BuffAmount3
											);

										if (buff.AddType == E_AddType.Fix)
											m_TowerInfo.BerserkerBuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											m_TowerInfo.BerserkerBuffList_Percent.Add(buff);
									}
								}
							}
						}
						break;
				}
			}
		}

		#region 버프 합연산
		foreach (var item in m_TowerInfo.BuffList_Fix)
		{
			switch (item.BuffType)
			{
				#region 타워 버프
				case E_BuffType.Atk:
					m_TowerInfo.Stat_Default.Dmg_Fix += item.BuffAmount;
					break;
				case E_BuffType.Range:
					m_TowerInfo.Stat_Default.Range += item.BuffAmount;
					m_TowerInfo.Stat_Skill01.Range += item.BuffAmount;
					m_TowerInfo.Stat_Skill02.Range += item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					m_TowerInfo.AttackSpeed_Default -= item.BuffAmount;
					break;
				case E_BuffType.Crit_rate:
					m_TowerInfo.Crit_Rate_Fix += item.BuffAmount;
					break;
				case E_BuffType.Crit_Dmg:
					m_TowerInfo.Crit_Dmg_Fix += item.BuffAmount;
					break;
				#endregion

				#region 적 디버프
				case E_BuffType.Def:
					break;
				case E_BuffType.Move_spd:
					break;
				case E_BuffType.Stun:
					break;
				case E_BuffType.Dot_Dmg:
					break;
				case E_BuffType.Insta_Kill:
					break;
				case E_BuffType.CritDmg_less:
					break;
				case E_BuffType.CritDmg_more:
					break;
					#endregion
			}
		}
		#endregion
		#region 버프 곱연산
		foreach (var item in m_TowerInfo.BuffList_Percent)
		{
			switch (item.BuffType)
			{
				#region 타워 버프
				case E_BuffType.Atk:
					m_TowerInfo.Stat_Default.Dmg_Percent *= item.BuffAmount;
					break;
				case E_BuffType.Range:
					m_TowerInfo.Stat_Default.Range *= item.BuffAmount;
					m_TowerInfo.Stat_Skill01.Range *= item.BuffAmount;
					m_TowerInfo.Stat_Skill02.Range *= item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					m_TowerInfo.AttackSpeed_Default *= item.BuffAmount;
					break;
				case E_BuffType.Crit_rate:
					m_TowerInfo.Crit_Rate_Percent *= item.BuffAmount;
					break;
				case E_BuffType.Crit_Dmg:
					m_TowerInfo.Crit_Dmg_Percent *= item.BuffAmount;
					break;
				#endregion

				#region 적 디버프
				case E_BuffType.Def:
					break;
				case E_BuffType.Move_spd:
					break;
				case E_BuffType.Stun:
					break;
				case E_BuffType.Dot_Dmg:
					break;
				case E_BuffType.Insta_Kill:
					break;
				case E_BuffType.CritDmg_less:
					break;
				case E_BuffType.CritDmg_more:
					break;
					#endregion
			}
		}
		#endregion

		// 공격속도 적용
		m_TowerAnimator.SetFloat("AttackSpeed", m_TowerInfo_Excel.Atk_Speed);
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

	protected IEnumerator Co_DevilSkillBuff_Fix(S_Buff buff, float time)
	{
		m_TowerInfo.DevilSkillBuffList_Fix.Add(buff);

		yield return new WaitForSeconds(time);

		m_TowerInfo.DevilSkillBuffList_Fix.Remove(buff);
	}
	protected IEnumerator Co_DevilSkillBuff_Percent(S_Buff buff, float time)
	{
		m_TowerInfo.DevilSkillBuffList_Percent.Add(buff);

		yield return new WaitForSeconds(time);

		m_TowerInfo.DevilSkillBuffList_Percent.Remove(buff);
	}
	#endregion
	#region 외부 함수
	// 타워 초기화
	public void InitializeTower(int code)
	{
		#region 엑셀 데이터
		m_TowerInfo_Excel = M_Tower.GetData(code);
		#endregion

		#region 내부 데이터
		m_TowerInfo.RotateSpeed = 5f;
		m_TowerInfo.CanAttack_Node = false;
		m_TowerInfo.CanAttack_Skill = true;

		// null 병합 연산자 안되는 이유
		// https://overworks.github.io/unity/2019/07/22/null-of-unity-object-part-2.html
		// m_TowerInfo.AttackPivot ??= transform.Find("Mesh").GetChild("AttackPivot");
		// 공격 피벗
		if (m_TowerInfo.AttackPivot == null)
		{
			m_TowerInfo.AttackPivot = transform.Find("Mesh").GetChild("AttackPivot");
		}

		#region 기본 스킬
		// 기본 스킬 데이터
		m_TowerInfo.Condition_Default_Origin = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
		m_TowerInfo.Stat_Default_Origin = M_Skill.GetStatData(m_TowerInfo.Condition_Default_Origin.PassiveCode);
		// 기본 스킬
		m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default_Origin.CoolTime;
		m_TowerInfo.AttackTimer_Default = m_TowerInfo.Stat_Default_Origin.CoolTime;
		#endregion

		#region 스킬01
		// 스킬01 데이터
		m_TowerInfo.Condition_Skill01_Origin = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
		m_TowerInfo.Stat_Skill01_Origin = M_Skill.GetStatData(m_TowerInfo.Condition_Skill01_Origin.PassiveCode);
		// 스킬01
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01_Origin.CoolTime;
		m_TowerInfo.AttackTimer_Skill01 = 0f;
		#endregion

		#region 스킬02
		// 스킬02 데이터
		m_TowerInfo.Condition_Skill02_Origin = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
		m_TowerInfo.Stat_Skill02_Origin = M_Skill.GetStatData(m_TowerInfo.Condition_Skill02_Origin.PassiveCode);
		// 스킬02
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02_Origin.CoolTime;
		m_TowerInfo.AttackTimer_Skill02 = 0f;
		#endregion

		#region 시너지
		// 시너지
		if (null == m_TowerInfo.SynergyList)
			m_TowerInfo.SynergyList = new List<Synergy_TableExcel>();
		else if (m_TowerInfo.SynergyList.Count > 0)
			m_TowerInfo.SynergyList.Clear();

		// 버프 (합연산)
		if (null == m_TowerInfo.BuffList_Fix)
			m_TowerInfo.BuffList_Fix = new List<S_Buff>();
		else if (m_TowerInfo.BuffList_Fix.Count > 0)
			m_TowerInfo.BuffList_Fix.Clear();
		// 버프 (곱연산)
		if (null == m_TowerInfo.BuffList_Percent)
			m_TowerInfo.BuffList_Percent = new List<S_Buff>();
		else if (m_TowerInfo.BuffList_Percent.Count > 0)
			m_TowerInfo.BuffList_Percent.Clear();

		// 버서커 버프 (합연산)
		if (null == m_TowerInfo.BerserkerBuffList_Fix)
			m_TowerInfo.BerserkerBuffList_Fix = new List<S_Buff>();
		else if (m_TowerInfo.BerserkerBuffList_Fix.Count > 0)
			m_TowerInfo.BerserkerBuffList_Fix.Clear();
		// 버서커 버프 (곱연산)
		if (null == m_TowerInfo.BerserkerBuffList_Percent)
			m_TowerInfo.BerserkerBuffList_Percent = new List<S_Buff>();
		else if (m_TowerInfo.BerserkerBuffList_Percent.Count > 0)
			m_TowerInfo.BerserkerBuffList_Percent.Clear();

		ClearSynergyBuff();
		#endregion
		#endregion

		#region 내부 컴포넌트
		// m_TowerAnimator ??= GetComponentInChildren<TowerAnimator>(true);
		if (null == m_TowerAnimator)
		{
			m_TowerAnimator = transform.Find("Mesh").GetComponent<TowerAnimator>();
			m_TowerAnimator.Initialize(this);
		}
		m_TowerAnimator.SetFloat("AttackSpeed", m_TowerInfo_Excel.Atk_Speed);

		// m_AttackRange_Default ??= transform.Find("AttackRange_Default").AddComponent<AttackRange>();
		if (null == m_AttackRange_Default)
		{
			m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
			m_AttackRange_Default.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Default.Initialize();
		}
		m_AttackRange_Default.Range = m_TowerInfo.Stat_Default_Origin.Range;
		m_AttackRange_Default.CanFindTarget = true;

		//m_AttackRange_Skill01 ??= transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
		if (m_AttackRange_Skill01 == null)
		{
			m_AttackRange_Skill01 = transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
			m_AttackRange_Skill01.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill01.Initialize();
		}
		m_AttackRange_Skill01.Range = m_TowerInfo.Stat_Skill01_Origin.Range;
		m_AttackRange_Skill01.CanFindTarget = true;

		//m_AttackRange_Skill02 ??= transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
		if (m_AttackRange_Skill02 == null)
		{
			m_AttackRange_Skill02 = transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
			m_AttackRange_Skill02.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill02.Initialize();
		}
		m_AttackRange_Skill02.Range = m_TowerInfo.Stat_Skill02_Origin.Range;
		m_AttackRange_Skill02.CanFindTarget = true;
		#endregion

		#region 이벤트 링크
		M_Synergy.OnUpdateSynergyStartEvent += ClearSynergyBuff;
		M_Synergy.OnUpdateSynergyEndEvent += UpdateSynergyBuff;

		M_Node.m_RotateStartEvent += () =>
		{
			m_AttackRange_Default.CanFindTarget = false;
			m_AttackRange_Skill01.CanFindTarget = false;
			m_AttackRange_Skill02.CanFindTarget = false;

			m_AttackRange_Default.Clear();
			m_AttackRange_Skill01.Clear();
			m_AttackRange_Skill02.Clear();
		};

		M_Node.m_RotateEndEvent += () =>
		{
			m_AttackRange_Default.CanFindTarget = true;
			m_AttackRange_Skill01.CanFindTarget = true;
			m_AttackRange_Skill02.CanFindTarget = true;

			m_AttackRange_Default.Direction = Direction;
			m_AttackRange_Skill01.Direction = Direction;
			m_AttackRange_Skill02.Direction = Direction;
		};
		#endregion
	}
	public void FinializeTower()
	{
		#region 내부 데이터
		m_TowerInfo.node?.ClearNode();

		m_TowerInfo.SynergyList?.Clear();
		m_TowerInfo.BuffList_Fix?.Clear();
		m_TowerInfo.BuffList_Percent?.Clear();
		m_TowerInfo.BerserkerBuffList_Fix?.Clear();
		m_TowerInfo.BerserkerBuffList_Percent?.Clear();
		#endregion

		#region 내부 컴포넌트
		m_AttackRange_Default?.Clear();
		m_AttackRange_Skill01?.Clear();
		m_AttackRange_Skill02?.Clear();
		#endregion

		#region 이벤트 링크
		M_Synergy.OnUpdateSynergyStartEvent -= ClearSynergyBuff;
		M_Synergy.OnUpdateSynergyEndEvent -= UpdateSynergyBuff;
		#endregion
	}

	public void CallAttack()
	{
		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
		m_TowerInfo.CanAttack_Skill = true;

		if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type != E_TargetType.TileTarget &&
			null == m_Target_Default)
			return;

		// 마왕 스킬 쿨타임 변경
		if (m_TowerInfo.ReduceCooldown)
		{
			float ReduceRand = Random.Range(0.00001f, 1f);

			if (ReduceRand <= 0.5f)
			{
				M_Devil.Devil.ReduceSkill01Cooldown(m_TowerInfo.ReduceCooldownSec);
			}
			else
			{
				M_Devil.Devil.ReduceSkill02Cooldown(m_TowerInfo.ReduceCooldownSec);
			}
		}

		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Default;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Default;

		#region 버서커 버프 합연산
		if (m_TowerInfo.Berserker)
		{
			if (m_TowerInfo.BerserkerStack < m_TowerInfo.BerserkerMaxStack)
				++m_TowerInfo.BerserkerStack;

			foreach (var item in m_TowerInfo.BerserkerBuffList_Fix)
			{
				float BuffAmount = item.BuffAmount * m_TowerInfo.BerserkerStack;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix += BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range += BuffAmount;
						m_TowerInfo.Stat_Skill01.Range += BuffAmount;
						m_TowerInfo.Stat_Skill02.Range += BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default -= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix += BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix += BuffAmount;
						break;
				}
			}
		}
		#endregion
		#region 버서커 버프 곱연산
		if (m_TowerInfo.Berserker)
		{
			foreach (var item in m_TowerInfo.BerserkerBuffList_Percent)
			{
				float BuffAmount = item.BuffAmount * m_TowerInfo.BerserkerStack;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Percent *= BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill01.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill02.Range *= BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default *= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Percent *= BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Percent *= BuffAmount;
						break;
				}
			}
		}
		#endregion

		#region 마왕 스킬 버프 합연산
		if (m_TowerInfo.DevilSkillBuffList_Fix.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Fix)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix += BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range += BuffAmount;
						m_TowerInfo.Stat_Skill01.Range += BuffAmount;
						m_TowerInfo.Stat_Skill02.Range += BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default -= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix += BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix += BuffAmount;
						break;
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		if (m_TowerInfo.DevilSkillBuffList_Percent.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Percent)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix *= BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill01.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill02.Range *= BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default *= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix *= BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix *= BuffAmount;
						break;
				}
			}
		}
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
					skill.transform.position = m_TowerInfo.AttackPivot.position;
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
					(m_TowerInfo_Excel.Crit_rate + m_TowerInfo.Crit_Rate_Fix) * m_TowerInfo.Crit_Rate_Percent,
					(m_TowerInfo_Excel.Crit_Dmg + m_TowerInfo.Crit_Dmg_Fix) * m_TowerInfo.Crit_Dmg_Percent
					)
				);
		}

		if ((E_TargetType)conditionData.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			foreach (var item in EnemyList)
			{
				Attack(item);
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
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}
	}
	public void CallSkill01()
	{
		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01_Origin.CoolTime;
		m_TowerInfo.CanAttack_Skill = true;

		if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type != E_TargetType.TileTarget &&
			null == m_Target_Skill01)
			return;

		// 스킬01 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill01;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill01;

		#region 마왕 스킬 버프 합연산
		if (m_TowerInfo.DevilSkillBuffList_Fix.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Fix)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix += BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range += BuffAmount;
						m_TowerInfo.Stat_Skill01.Range += BuffAmount;
						m_TowerInfo.Stat_Skill02.Range += BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default -= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix += BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix += BuffAmount;
						break;
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		if (m_TowerInfo.DevilSkillBuffList_Percent.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Percent)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix *= BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill01.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill02.Range *= BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default *= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix *= BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix *= BuffAmount;
						break;
				}
			}
		}
		#endregion

		// 스킬01 투사체 생성
		int Skill01Code = conditionData.projectile_prefab;

		void Skill01(Enemy target)
		{
			Skill skill = M_Skill.SpawnProjectileSkill(Skill01Code);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					skill.transform.position = m_TowerInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					skill.transform.position = m_Target_Skill01.HitPivot.position;
					break;
			}

			skill.enabled = true;
			skill.gameObject.SetActive(true);

			// 스킬01 데이터 설정
			skill.InitializeSkill(
				target,
				conditionData,
				statData,
				new S_Critical(
					(m_TowerInfo_Excel.Crit_rate + m_TowerInfo.Crit_Rate_Fix) * m_TowerInfo.Crit_Rate_Percent,
					(m_TowerInfo_Excel.Crit_Dmg + m_TowerInfo.Crit_Dmg_Fix) * m_TowerInfo.Crit_Dmg_Percent
					)
				);
		}

		if ((E_TargetType)m_TowerInfo.Condition_Skill01_Origin.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			foreach (var item in EnemyList)
			{
				Skill01(item);
			}
		}
		else
		{
			Skill01(m_Target_Skill01);
		}

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}
	}
	public void CallSkill02()
	{
		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02_Origin.CoolTime;
		m_TowerInfo.CanAttack_Skill = true;

		if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type != E_TargetType.TileTarget &&
			null == m_Target_Skill02)
			return;

		// 스킬02 데이터 불러오기
		SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill02;
		SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill02;

		#region 마왕 스킬 버프 합연산
		if (m_TowerInfo.DevilSkillBuffList_Fix.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Fix)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix += BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range += BuffAmount;
						m_TowerInfo.Stat_Skill01.Range += BuffAmount;
						m_TowerInfo.Stat_Skill02.Range += BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default -= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix += BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix += BuffAmount;
						break;
				}
			}
		}
		#endregion
		#region 마왕 스킬 버프 곱연산
		if (m_TowerInfo.DevilSkillBuffList_Percent.Count > 0)
		{
			foreach (var item in m_TowerInfo.DevilSkillBuffList_Percent)
			{
				float BuffAmount = item.BuffAmount;

				switch (item.BuffType)
				{
					case E_BuffType.Atk:
						m_TowerInfo.Stat_Default.Dmg_Fix *= BuffAmount;
						break;
					case E_BuffType.Range:
						m_TowerInfo.Stat_Default.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill01.Range *= BuffAmount;
						m_TowerInfo.Stat_Skill02.Range *= BuffAmount;
						break;
					case E_BuffType.Atk_spd:
						m_TowerInfo.AttackSpeed_Default *= BuffAmount;
						break;
					case E_BuffType.Crit_rate:
						m_TowerInfo.Crit_Rate_Fix *= BuffAmount;
						break;
					case E_BuffType.Crit_Dmg:
						m_TowerInfo.Crit_Dmg_Fix *= BuffAmount;
						break;
				}
			}
		}
		#endregion

		// 스킬02 투사체 생성
		int Skill02Code = conditionData.projectile_prefab;

		void Skill02(Enemy target)
		{
			Skill skill = M_Skill.SpawnProjectileSkill(Skill02Code);

			switch ((E_FireType)conditionData.Atk_pick)
			{
				case E_FireType.Select_point:
					break;
				case E_FireType.Select_self:
					skill.transform.position = m_TowerInfo.AttackPivot.position;
					break;
				case E_FireType.Select_enemy:
					skill.transform.position = m_Target_Skill02.HitPivot.position;
					break;
			}

			skill.enabled = true;
			skill.gameObject.SetActive(true);

			// 스킬02 데이터 설정
			skill.InitializeSkill(
				target,
				conditionData,
				statData,
				new S_Critical(
					(m_TowerInfo_Excel.Crit_rate + m_TowerInfo.Crit_Rate_Fix) * m_TowerInfo.Crit_Rate_Percent,
					(m_TowerInfo_Excel.Crit_Dmg + m_TowerInfo.Crit_Dmg_Fix) * m_TowerInfo.Crit_Dmg_Percent
					)
				);
		}

		if ((E_TargetType)m_TowerInfo.Condition_Skill02_Origin.Target_type == E_TargetType.TileTarget)
		{
			List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

			foreach (var item in EnemyList)
			{
				Skill02(item);
			}
		}
		else
		{
			Skill02(m_Target_Skill02);
		}

		// 이펙트 생성
		Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
		if (null != atkEffect)
		{
			atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
			atkEffect.gameObject.SetActive(true);
		}
	}

	public void AddSynergy(Synergy_TableExcel synergy)
	{
		// 시너지 추가
		m_TowerInfo.SynergyList.Add(synergy);
	}

	public void ClearTarget()
	{
		m_Target_Default = null;
		m_AttackRange_Default.Clear();
		m_Target_Skill01 = null;
		m_AttackRange_Skill01.Clear();
		m_Target_Skill02 = null;
		m_AttackRange_Skill02.Clear();
	}
	public void AddDevilSkillBuff_Fix(S_Buff buff, float time)
	{
		StartCoroutine(Co_DevilSkillBuff_Fix(buff, time));
	}
	public void AddDevilSkillBuff_Percent(S_Buff buff, float time)
	{
		StartCoroutine(Co_DevilSkillBuff_Percent(buff, time));
	}
	#endregion
	#region 유니티 콜백 함수
	private void Update()
	{
		UpdateTarget();
		RotateToTarget();
		UpdateAttackTimer();
		AttackTarget();
	}

	private void OnApplicationQuit()
	{
		FinializeTower();
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
		public bool CanAttack_Node;
		// 공격 가능 여부 (스킬 중복)
		public bool CanAttack_Skill;

		#region 기본 스킬
		// 기본 스킬 엑셀 데이터
		public SkillCondition_TableExcel Condition_Default_Origin;
		public SkillStat_TableExcel Stat_Default_Origin;
		// 기본 스킬 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;
		#endregion
		#region 스킬01
		// 스킬01 엑셀 데이터
		public SkillCondition_TableExcel Condition_Skill01_Origin;
		public SkillStat_TableExcel Stat_Skill01_Origin;
		// 스킬01 데이터
		public SkillCondition_TableExcel Condition_Skill01;
		public SkillStat_TableExcel Stat_Skill01;
		// 스킬01 공격 속도
		public float AttackSpeed_Skill01;
		// 스킬01 타이머
		public float AttackTimer_Skill01;
		#endregion
		#region 스킬02
		// 스킬02 엑셀 데이터
		public SkillCondition_TableExcel Condition_Skill02_Origin;
		public SkillStat_TableExcel Stat_Skill02_Origin;
		// 스킬02 데이터
		public SkillCondition_TableExcel Condition_Skill02;
		public SkillStat_TableExcel Stat_Skill02;
		// 스킬02 공격 속도
		public float AttackSpeed_Skill02;
		// 스킬02 타이머
		public float AttackTimer_Skill02;
		#endregion

		#region 크리티컬
		public float Crit_Rate_Fix;
		public float Crit_Rate_Percent;
		public float Crit_Dmg_Fix;
		public float Crit_Dmg_Percent;
		#endregion

		#region 시너지
		// 시너지 리스트
		public List<Synergy_TableExcel> SynergyList;

		#region 버프
		// 합버프 리스트
		public List<S_Buff> BuffList_Fix;
		// 곱버프 리스트
		public List<S_Buff> BuffList_Percent;
		#endregion

		#region 마왕 스킬 쿨타임 변경
		public bool ReduceCooldown;
		public float ReduceCooldownSec;
		#endregion

		#region 버서커
		public bool Berserker;
		public int BerserkerStack;
		public int BerserkerMaxStack;
		// 합버프 리스트
		public List<S_Buff> BerserkerBuffList_Fix;
		// 곱버프 리스트
		public List<S_Buff> BerserkerBuffList_Percent;
		#endregion
		#endregion

		public List<S_Buff> DevilSkillBuffList_Fix;
		public List<S_Buff> DevilSkillBuffList_Percent;
	}
}
