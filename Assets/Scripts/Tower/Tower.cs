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

	// 공격 가능 여부
	protected bool CanAttack_Skill => CanAttack_Default && CanAttack_Skill01 && CanAttack_Skill02;
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
	public bool CanAttack_Default { get => m_TowerInfo.CanAttack_Default; set => m_TowerInfo.CanAttack_Default = value; }
	public bool CanAttack_Skill01 { get => m_TowerInfo.CanAttack_Skill01; set => m_TowerInfo.CanAttack_Skill01 = value; }
	public bool CanAttack_Skill02 { get => m_TowerInfo.CanAttack_Skill02; set => m_TowerInfo.CanAttack_Skill02 = value; }
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
				if (m_TowerInfo.Berserker)
				{
					m_TowerInfo.BerserkerStack = 0;

					UpdateSynergyBuff();
				}

				// 타겟 변경 기준에 따라
				switch ((E_TargetType)m_TowerInfo.Condition_Default.Target_type)
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
								m_Target_Default.IsDead ||
								DistanceToTarget_Default > m_TowerInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
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
					CanAttack_Default = true;
					m_TowerAnimator.ResetTrigger("Attack");
				}
			}
		}
		#endregion
		#region 스킬01
		if (m_TowerInfo_Excel.Skill1Code != 0)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type != E_TargetType.TileTarget)
			{
				if (IsTargetDead_Skill01 || LostTarget_Skill01)
				{
					// 타겟 변경 기준에 따라
					switch ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type)
					{
						case E_TargetType.CloseTarget:
							{
								m_Target_Skill01 = m_AttackRange_Skill01.GetNearTarget();
							}
							break;
						case E_TargetType.RandTarget:
							{
								m_Target_Skill01 = m_AttackRange_Skill01.GetRandomTarget();
							}
							break;
						// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
						case E_TargetType.FixTarget:
							{
								if (null == m_Target_Skill01 || // 예외처리
									m_Target_Skill01.IsDead ||
									DistanceToTarget_Skill01 > m_TowerInfo.Stat_Skill01.Range) // 타겟이 사거리를 벗어난 경우
								{
									m_Target_Skill01 = m_AttackRange_Skill01.GetNearTarget();
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
						CanAttack_Skill01 = true;
						m_TowerAnimator.ResetTrigger("Skill01");
					}
				}
			}
		}
		#endregion
		#region 스킬02
		if (m_TowerInfo_Excel.Skill2Code != 0)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type != E_TargetType.TileTarget)
			{
				if (IsTargetDead_Skill02 || LostTarget_Skill02)
				{
					// 타겟 변경 기준에 따라
					switch ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type)
					{
						case E_TargetType.CloseTarget:
							{
								m_Target_Skill02 = m_AttackRange_Skill02.GetNearTarget();
							}
							break;
						case E_TargetType.RandTarget:
							{
								m_Target_Skill02 = m_AttackRange_Skill02.GetRandomTarget();
							}
							break;
						// FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
						case E_TargetType.FixTarget:
							{
								if (null == m_Target_Skill02 || // 예외처리
									m_Target_Skill02.IsDead ||
									DistanceToTarget_Skill02 > m_TowerInfo.Stat_Skill02.Range) // 타겟이 사거리를 벗어난 경우
								{
									m_Target_Skill02 = m_AttackRange_Skill02.GetNearTarget();
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
						CanAttack_Skill02 = true;
						m_TowerAnimator.ResetTrigger("Skill02");
					}
				}
			}
		}
		#endregion
	}
	// 타워 공격 타이머
	protected void UpdateAttackTimer()
	{
		// 스킬01 타이머
		if (m_TowerInfo_Excel.Skill1Code != 0)
		{
			if (m_TowerInfo.AttackTimer_Skill01 < m_TowerInfo.AttackSpeed_Skill01)
			{
				m_TowerInfo.AttackTimer_Skill01 += Time.deltaTime;
			}
		}

		// 스킬02 타이머
		if (m_TowerInfo_Excel.Skill2Code != 0)
		{
			if (m_TowerInfo.AttackTimer_Skill02 < m_TowerInfo.AttackSpeed_Skill02)
			{
				m_TowerInfo.AttackTimer_Skill02 += Time.deltaTime;
			}
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
		void Skill01()
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Skill01 -= m_TowerInfo.AttackSpeed_Skill01;
			m_TowerInfo.CanAttack_Skill01 = false;

			// 스킬01 애니메이션 재생
			SetSkill01Trigger();
			return;
		}

		// 스킬01 공격
		if (m_TowerInfo_Excel.Skill1Code != 0)
		{
			if (CheckAttackTimer_Skill01 &&
				m_TowerInfo.CanAttack_Node && CanAttack_Skill)
			{
				if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget &&
					M_Enemy.GetEnemyList(Direction).Count > 0)
				{
					Skill01();
					return;
				}

				if (!IsTargetDead_Skill01 && !LostTarget_Skill01)
				{
					Skill01();
					return;
				}
			}
		}
		#endregion

		#region 스킬02
		void Skill02()
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Skill02 -= m_TowerInfo.AttackSpeed_Skill02;
			m_TowerInfo.CanAttack_Skill02 = false;

			// 스킬02 애니메이션 재생
			SetSkill02Trigger();
		}

		// 스킬02 공격
		if (m_TowerInfo_Excel.Skill2Code != 0)
		{
			if (CheckAttackTimer_Skill02 &&
				m_TowerInfo.CanAttack_Node && CanAttack_Skill)
			{
				if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget &&
					M_Enemy.GetEnemyList(Direction).Count > 0)
				{
					Skill02();
					return;
				}

				if (!IsTargetDead_Skill02 && !LostTarget_Skill02)
				{
					Skill02();
					return;
				}
			}
		}
		#endregion

		#region 기본 스킬
		void Attack()
		{
			// 내부 데이터 정리
			m_TowerInfo.AttackTimer_Default -= m_TowerInfo.AttackSpeed_Default;
			m_TowerInfo.CanAttack_Default = false;

			// 기본 공격 애니메이션 재생
			SetAttackTrigger();
		}

		// 기본 스킬 공격
		if (CheckAttackTimer_Default &&
			m_TowerInfo.CanAttack_Node && CanAttack_Skill)
		{
			if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type == E_TargetType.TileTarget &&
				M_Enemy.GetEnemyList(Direction).Count > 0)
			{
				Attack();
				return;
			}

			if (!IsTargetDead_Default && !LostTarget_Default)
			{
				Attack();
				return;
			}
		}
		#endregion
	}

	protected void ClearSynergyBuff()
	{
		m_TowerInfo.SynergyList.Clear();

		m_TowerInfo.Buff_AttackType = E_AttackType.None;
		m_TowerInfo.Target_num = m_TowerInfo.Stat_Default.Target_num;

		m_TowerInfo.Berserker = false;
		m_TowerInfo.BerserkerStack = 0;
		m_TowerInfo.BerserkerMaxStack = 0;

		#region 마왕 스킬 버프만 적용
		List<S_Buff> BuffList_Fix = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
		List<S_Buff> BuffList_Percent = new List<S_Buff>(m_TowerInfo.SkillBuffList_Percent);

		float Range_Fix = 0f;
		float Range_Percent = 1f;
		float Atk_spd_Fix = 0f;
		float Atk_spd_Percent = 1f;

		float Range = m_TowerInfo.Stat_Default.Range;
		float AttackSpeed = m_TowerInfo_Excel.Atk_Speed;

		foreach (var item in BuffList_Fix)
		{
			switch (item.BuffType)
			{
				case E_BuffType.Range:
					Range_Fix += item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					Atk_spd_Fix += item.BuffAmount;
					break;
			}
		}
		foreach (var item in BuffList_Percent)
		{
			switch (item.BuffType)
			{
				case E_BuffType.Range:
					Range_Percent *= item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					Atk_spd_Percent *= item.BuffAmount;
					break;
			}
		}

		Range += Range_Fix;
		Range *= Range_Percent;
		m_AttackRange_Default.Range = Range;

		AttackSpeed += Atk_spd_Fix;
		AttackSpeed *= Atk_spd_Percent;
		m_TowerAnimator.SetFloat("AttackSpeed", AttackSpeed);
		#endregion
	}
	protected void UpdateSynergyBuff()
	{
		// 마왕 스킬 버프 추가
		List<S_Buff> BuffList_Fix = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
		List<S_Buff> BuffList_Percent = new List<S_Buff>(m_TowerInfo.SkillBuffList_Percent);

		float Range_Fix = 0f;
		float Range_Percent = 1f;
		float Atk_spd_Fix = 0f;
		float Atk_spd_Percent = 1f;

		float Range = m_TowerInfo.Stat_Default.Range;
		float AttackSpeed = m_TowerInfo_Excel.Atk_Speed;

		// 시너지 버프, 버서커 버프 추가
		#region 시너지
		foreach (var item in m_TowerInfo.SynergyList)
		{
			switch ((E_SynergyEffectType)item.EffectType1)
			{
				case E_SynergyEffectType.Buff:
					{
						BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								if (buff.AddType == E_AddType.Fix)
									BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									BuffList_Percent.Add(buff);

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										if (buff.AddType == E_AddType.Fix)
											BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											BuffList_Percent.Add(buff);

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												if (buff.AddType == E_AddType.Fix)
													BuffList_Fix.Add(buff);
												else if (buff.AddType == E_AddType.Percent)
													BuffList_Percent.Add(buff);
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}
					break;
				case E_SynergyEffectType.ChangeAtkType:
					{
						m_TowerInfo.Buff_AttackType = (E_AttackType)item.EffectChange1;

						if (m_TowerInfo.Buff_AttackType == E_AttackType.BounceFire)
						{
							m_TowerInfo.Target_num = item.EffectReq1;
						}
					}
					break;
				case E_SynergyEffectType.Berserker:
					{
						m_TowerInfo.Berserker = true;
						m_TowerInfo.BerserkerMaxStack = item.EffectReq1;

						BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								if (buff.AddType == E_AddType.Fix)
									BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
								{
									buff.BuffAmount += buffData.BuffAmount1;
									BuffList_Percent.Add(buff);
								}

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										if (buff.AddType == E_AddType.Fix)
											BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
										{
											buff.BuffAmount += buffData.BuffAmount2;
											BuffList_Percent.Add(buff);
										}

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												if (buff.AddType == E_AddType.Fix)
													BuffList_Fix.Add(buff);
												else if (buff.AddType == E_AddType.Percent)
												{
													buff.BuffAmount += buffData.BuffAmount3;
													BuffList_Percent.Add(buff);
												}
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}
					break;
			}

			switch ((E_SynergyEffectType)item.EffectType2)
			{
				case E_SynergyEffectType.Buff:
					{
						BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								if (buff.AddType == E_AddType.Fix)
									BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
									BuffList_Percent.Add(buff);

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										if (buff.AddType == E_AddType.Fix)
											BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
											BuffList_Percent.Add(buff);

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												if (buff.AddType == E_AddType.Fix)
													BuffList_Fix.Add(buff);
												else if (buff.AddType == E_AddType.Percent)
													BuffList_Percent.Add(buff);
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}
					break;
				case E_SynergyEffectType.ChangeAtkType:
					{
						m_TowerInfo.Buff_AttackType = (E_AttackType)item.EffectChange2;

						if (m_TowerInfo.Buff_AttackType == E_AttackType.BounceFire)
						{
							m_TowerInfo.Target_num = item.EffectReq2;
						}
					}
					break;
				case E_SynergyEffectType.Berserker:
					{
						m_TowerInfo.Berserker = true;
						m_TowerInfo.BerserkerMaxStack = item.EffectReq2;

						BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								if (buff.AddType == E_AddType.Fix)
									BuffList_Fix.Add(buff);
								else if (buff.AddType == E_AddType.Percent)
								{
									buff.BuffAmount += buffData.BuffAmount1;
									BuffList_Percent.Add(buff);
								}

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										if (buff.AddType == E_AddType.Fix)
											BuffList_Fix.Add(buff);
										else if (buff.AddType == E_AddType.Percent)
										{
											buff.BuffAmount += buffData.BuffAmount2;
											BuffList_Percent.Add(buff);
										}

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												if (buff.AddType == E_AddType.Fix)
													BuffList_Fix.Add(buff);
												else if (buff.AddType == E_AddType.Percent)
												{
													buff.BuffAmount += buffData.BuffAmount3;
													BuffList_Percent.Add(buff);
												}
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}
					break;
			}
		}
		#endregion

		foreach (var item in BuffList_Fix)
		{
			switch (item.BuffType)
			{
				case E_BuffType.Range:
					Range_Fix += item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					Atk_spd_Fix += item.BuffAmount;
					break;
			}
		}
		foreach (var item in BuffList_Percent)
		{
			switch (item.BuffType)
			{
				case E_BuffType.Range:
					Range_Percent *= item.BuffAmount;
					break;
				case E_BuffType.Atk_spd:
					Atk_spd_Percent *= item.BuffAmount;
					break;
			}
		}

		Range += Range_Fix;
		Range *= Range_Percent;
		m_AttackRange_Default.Range = Range;

		AttackSpeed += Atk_spd_Fix;
		AttackSpeed *= Atk_spd_Percent;
		m_TowerAnimator.SetFloat("AttackSpeed", AttackSpeed);
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

	protected IEnumerator Co_SkillBuff_Fix(S_Buff buff, float time)
	{
		if (buff.AddType != E_AddType.Fix)
			yield break;

		m_TowerInfo.SkillBuffList_Fix.AddLast(buff);

		float BuffAmount = buff.BuffAmount;

		switch (buff.BuffType)
		{
			case E_BuffType.Range:
				{
					float Range = m_AttackRange_Default.Range + BuffAmount;
					m_AttackRange_Default.Range = Range;
				}
				break;
			case E_BuffType.Atk_spd:
				{
					float AttackSpeed = m_TowerAnimator.GetFloat("AttackSpeed") + BuffAmount;
					m_TowerAnimator.SetFloat("AttackSpeed", AttackSpeed);
				}
				break;
		}

		yield return new WaitForSeconds(time);

		m_TowerInfo.SkillBuffList_Fix.Remove(buff);

		UpdateSynergyBuff();
	}
	protected IEnumerator Co_SkillBuff_Percent(S_Buff buff, float time)
	{
		if (buff.AddType != E_AddType.Percent)
			yield break;

		m_TowerInfo.SkillBuffList_Percent.AddLast(buff);

		float BuffAmount = buff.BuffAmount;

		switch (buff.BuffType)
		{
			case E_BuffType.Range:
				{
					float Range = m_AttackRange_Default.Range * BuffAmount;
					m_AttackRange_Default.Range = Range;
				}
				break;
			case E_BuffType.Atk_spd:
				{
					float AttackSpeed = m_TowerAnimator.GetFloat("AttackSpeed") * BuffAmount;
					m_TowerAnimator.SetFloat("AttackSpeed", AttackSpeed);
				}
				break;
		}

		yield return new WaitForSeconds(time);

		m_TowerInfo.SkillBuffList_Percent.Remove(buff);

		UpdateSynergyBuff();
	}
	#endregion
	#region 외부 함수
	// 타워 초기화
	public void InitializeTower(int code)
	{
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
			m_AttackRange_Default.InitializeAttackRange();
		}
		m_AttackRange_Default.Range = m_TowerInfo.Stat_Default.Range;
		m_AttackRange_Default.CanFindTarget = true;

		//m_AttackRange_Skill01 ??= transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
		if (m_AttackRange_Skill01 == null)
		{
			m_AttackRange_Skill01 = transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
			m_AttackRange_Skill01.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill01.InitializeAttackRange();
		}
		m_AttackRange_Skill01.Range = m_TowerInfo.Stat_Skill01.Range;
		m_AttackRange_Skill01.CanFindTarget = true;

		//m_AttackRange_Skill02 ??= transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
		if (m_AttackRange_Skill02 == null)
		{
			m_AttackRange_Skill02 = transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
			m_AttackRange_Skill02.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
			m_AttackRange_Skill02.InitializeAttackRange();
		}
		m_AttackRange_Skill02.Range = m_TowerInfo.Stat_Skill02.Range;
		m_AttackRange_Skill02.CanFindTarget = true;
		#endregion

		#region 엑셀 데이터
		m_TowerInfo_Excel = M_Tower.GetData(code);
		#endregion

		#region 내부 데이터
		m_TowerInfo.RotateSpeed = 5f;
		m_TowerInfo.CanAttack_Node = false;
		m_TowerInfo.CanAttack_Default = true;
		m_TowerInfo.CanAttack_Skill01 = true;
		m_TowerInfo.CanAttack_Skill02 = true;

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
		m_TowerInfo.Condition_Default = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
		m_TowerInfo.Stat_Default = M_Skill.GetStatData(m_TowerInfo.Condition_Default.PassiveCode);
		// 기본 스킬
		m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
		m_TowerInfo.AttackTimer_Default = m_TowerInfo.Stat_Default.CoolTime;
		#endregion
		#region 스킬01
		// 스킬01 데이터
		m_TowerInfo.Condition_Skill01 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
		m_TowerInfo.Stat_Skill01 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill01.PassiveCode);
		// 스킬01
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
		m_TowerInfo.AttackTimer_Skill01 = 0f;
		#endregion
		#region 스킬02
		// 스킬02 데이터
		m_TowerInfo.Condition_Skill02 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
		m_TowerInfo.Stat_Skill02 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill02.PassiveCode);
		// 스킬02
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
		m_TowerInfo.AttackTimer_Skill02 = 0f;
		#endregion

		#region 시너지
		// 시너지
		if (null == m_TowerInfo.SynergyList)
			m_TowerInfo.SynergyList = new List<Synergy_TableExcel>();
		else if (m_TowerInfo.SynergyList.Count > 0)
			m_TowerInfo.SynergyList.Clear();

		// 버프 (합연산)
		if (null == m_TowerInfo.SkillBuffList_Fix)
			m_TowerInfo.SkillBuffList_Fix = new LinkedList<S_Buff>();
		else if (m_TowerInfo.SkillBuffList_Fix.Count > 0)
			m_TowerInfo.SkillBuffList_Fix.Clear();
		// 버프 (곱연산)
		if (null == m_TowerInfo.SkillBuffList_Percent)
			m_TowerInfo.SkillBuffList_Percent = new LinkedList<S_Buff>();
		else if (m_TowerInfo.SkillBuffList_Percent.Count > 0)
			m_TowerInfo.SkillBuffList_Percent.Clear();

		ClearSynergyBuff();
		#endregion
		#endregion

		#region 이벤트 링크
		M_Synergy.OnUpdateSynergyStartEvent += ClearSynergyBuff;
		M_Synergy.OnUpdateSynergyEndEvent += UpdateSynergyBuff;

		M_Node.OnRotateStartEvent += () =>
		{
			m_AttackRange_Default.CanFindTarget = false;
			m_AttackRange_Skill01.CanFindTarget = false;
			m_AttackRange_Skill02.CanFindTarget = false;

			m_AttackRange_Default.Clear();
			m_AttackRange_Skill01.Clear();
			m_AttackRange_Skill02.Clear();
		};
		M_Node.OnRotateEndEvent += () =>
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
		m_TowerInfo.SkillBuffList_Fix?.Clear();
		m_TowerInfo.SkillBuffList_Percent?.Clear();
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
		m_TowerInfo.CanAttack_Default = true;

		switch (m_TowerInfo.Condition_Default.Ally)
		{
			case 1:
				{
					Debug.LogError("평타 공격 Ally 에러");
				}
				break;
			case 2:
				{
					if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
					{
						if (M_Enemy.GetEnemyList(Direction).Count <= 0)
							return;
					}
					else if (IsTargetDead_Default || LostTarget_Default)
						return;

					SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Default;
					SkillStat_TableExcel statData = m_TowerInfo.Stat_Default;
					statData.Dmg_Fix += m_TowerInfo_Excel.Atk;

					#region 시너지
					if (m_TowerInfo.Buff_AttackType != E_AttackType.None)
					{
						conditionData.Atk_type = (int)m_TowerInfo.Buff_AttackType;
						if (m_TowerInfo.Buff_AttackType == E_AttackType.BounceFire)
						{
							statData.Target_num = m_TowerInfo.Target_num;
						}
					}

					if (m_TowerInfo.Berserker &&
						m_TowerInfo.BerserkerStack < m_TowerInfo.BerserkerMaxStack)
					{
						++m_TowerInfo.BerserkerStack;
					}
					#endregion

					#region 버프
					// 마왕 버프 추가
					List<S_Buff> BuffList_Fix = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					List<S_Buff> BuffList_Percent = new List<S_Buff>(m_TowerInfo.SkillBuffList_Percent);

					// 적 디버프 리스트
					List<S_Buff> debuffList = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					debuffList.AddRange(m_TowerInfo.SkillBuffList_Percent);

					float Range_Fix = 0f;
					float Range_Percent = 1f;
					float Atk_spd_Fix = 0f;
					float Atk_spd_Percent = 1f;
					float Crit_rate_Fix = 0f;
					float Crit_rate_Percent = 1f;
					float Crit_Dmg_Fix = 0f;
					float Crit_Dmg_Percent = 1f;

					#region 시너지 버프, 버서커 버프 추가
					foreach (var item in m_TowerInfo.SynergyList)
					{
						switch ((E_SynergyEffectType)item.EffectType1)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}

						switch ((E_SynergyEffectType)item.EffectType2)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}
					}
					#endregion

					#region 합연산
					foreach (var item in BuffList_Fix)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Fix += BuffAmount;
								break;
							case E_BuffType.Range:
								Range_Fix += BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								Atk_spd_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Fix += BuffAmount;
								break;
						}
					}
					#endregion
					#region 곱연산
					foreach (var item in BuffList_Percent)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Percent *= BuffAmount;
								break;
							case E_BuffType.Range:
								Range_Percent *= BuffAmount;
								break;
							case E_BuffType.Atk_spd:
								Atk_spd_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Percent *= BuffAmount;
								break;
						}
					}
					#endregion

					#region 버프 적용
					statData.Range += Range_Fix;
					statData.Range *= Range_Percent;
					m_AttackRange_Default.Range = statData.Range;

					float Atk_Speed = m_TowerInfo_Excel.Atk_Speed;
					Atk_Speed += Atk_spd_Fix;
					Atk_Speed *= Atk_spd_Percent;
					m_TowerAnimator.SetFloat("AttackSpeed", Atk_Speed);
					#endregion
					#endregion

					#region 크리티컬
					float Crit_rate = (m_TowerInfo_Excel.Crit_rate + Crit_rate_Fix) * Crit_rate_Percent;
					float Crit_Dmg = (m_TowerInfo_Excel.Crit_Dmg + Crit_Dmg_Fix) * Crit_Dmg_Percent;
					S_Critical critical = new S_Critical(Crit_rate, Crit_Dmg);
					#endregion

					#region 공격
					// 기본 스킬 투사체 코드
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
							critical,
							debuffList
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
					#endregion

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			case 3:
				{
					Debug.LogError("평타 공격 Ally 에러");
				}
				break;
			default:
				{
					Debug.LogError("평타 공격 Ally 에러");
				}
				break;
		}
	}
	public void CallSkill01()
	{
		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
		m_TowerInfo.CanAttack_Skill01 = true;

		switch (m_TowerInfo.Condition_Skill01.Ally)
		{
			case 1:
				{
					float range = m_TowerInfo.Stat_Skill01.Range;
					int layerMask = LayerMask.GetMask("Tower");

					Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);

					int buffCode = m_TowerInfo.Stat_Skill01.Buff_CC;
					BuffCC_TableExcel buffData = M_Buff.GetData(buffCode);
					float buffDuration = buffData.Duration;

					for (int i = 0; i < colliders.Length; ++i)
					{
						Tower tower = colliders[i].GetComponent<Tower>();

						if (this == tower)
							continue;

						if (null == tower)
						{
							Debug.Log(m_TowerInfo_Excel.Name_KR + "의 버프 스킬1 오류");
							continue;
						}

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								tower.AddSkillBuff(buff, buffDuration);

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										tower.AddSkillBuff(buff, buffDuration);

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												tower.AddSkillBuff(buff, buffDuration);
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(m_TowerInfo.Condition_Skill01.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			case 2:
				{
					if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget)
					{
						if (M_Enemy.GetEnemyList(Direction).Count <= 0)
							return;
					}
					else if (IsTargetDead_Skill01 || LostTarget_Skill01)
						return;

					// 스킬01 데이터 불러오기
					SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill01;
					SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill01;
					statData.Dmg_Fix += m_TowerInfo_Excel.Atk;

					#region 버프
					// 마왕 버프 추가
					List<S_Buff> BuffList_Fix = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					List<S_Buff> BuffList_Percent = new List<S_Buff>(m_TowerInfo.SkillBuffList_Percent);

					// 적 디버프 리스트
					List<S_Buff> debuffList = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					debuffList.AddRange(m_TowerInfo.SkillBuffList_Percent);

					float Crit_rate_Fix = 0f;
					float Crit_rate_Percent = 1f;
					float Crit_Dmg_Fix = 0f;
					float Crit_Dmg_Percent = 1f;

					#region 시너지 버프, 버서커 버프 추가
					foreach (var item in m_TowerInfo.SynergyList)
					{
						switch ((E_SynergyEffectType)item.EffectType1)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}

						switch ((E_SynergyEffectType)item.EffectType2)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}
					}
					#endregion

					#region 합연산
					foreach (var item in BuffList_Fix)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Fix += BuffAmount;
								break;
						}
					}
					#endregion
					#region 곱연산
					foreach (var item in BuffList_Percent)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Percent *= BuffAmount;
								break;
						}
					}
					#endregion
					#endregion

					#region 크리티컬
					float Crit_rate = (m_TowerInfo_Excel.Crit_rate + Crit_rate_Fix) * Crit_rate_Percent;
					float Crit_Dmg = (m_TowerInfo_Excel.Crit_Dmg + Crit_Dmg_Fix) * Crit_Dmg_Percent;
					S_Critical critical = new S_Critical(Crit_rate, Crit_Dmg);
					#endregion

					#region 공격
					// 스킬01 투사체 코드
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
							critical,
							debuffList
							);
					}

					if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget)
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
					#endregion

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			case 3:
				{
					int buffCode = m_TowerInfo.Stat_Skill01.Buff_CC;
					BuffCC_TableExcel buffData = M_Buff.GetData(buffCode);
					float buffDuration = buffData.Duration;

					#region 버프1
					if (buffData.BuffType1 != 0)
					{
						float buffRand1 = Random.Range(0.00001f, 1f);
						bool buffApply1 = buffRand1 <= buffData.BuffRand1;

						if (buffApply1)
						{
							S_Buff buff = new S_Buff(
								buffData.Name_KR + "_1",
								buffData.BuffType1,
								buffData.AddType1,
								buffData.BuffAmount1,
								buffData.BuffRand1,
								buffData.Duration,
								buffData.Prefab
								);

							AddSkillBuff(buff, buffDuration);

							#region 버프2
							if (buffData.BuffType2 != 0)
							{
								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2,
										buffData.BuffRand2,
										buffData.Duration,
										buffData.Prefab
										);

									AddSkillBuff(buff, buffDuration);

									#region 버프3
									if (buffData.BuffType3 != 0)
									{
										float buffRand3 = Random.Range(0.00001f, 1f);
										bool buffApply3 = buffRand3 <= buffData.BuffRand3;

										if (buffApply3)
										{
											buff = new S_Buff(
												buffData.Name_KR + "_3",
												buffData.BuffType3,
												buffData.AddType3,
												buffData.BuffAmount3,
												buffData.BuffRand3,
												buffData.Duration,
												buffData.Prefab
												);

											AddSkillBuff(buff, buffDuration);
										}
									}
									#endregion
								}
							}
							#endregion
						}
					}
					#endregion

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(m_TowerInfo.Condition_Skill01.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			default:
				{
					Debug.LogError("스킬01 Ally 에러");
				}
				break;
		}
	}
	public void CallSkill02()
	{
		// 내부 데이터 정리
		m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
		m_TowerInfo.CanAttack_Skill02 = true;

		switch (m_TowerInfo.Condition_Skill02.Ally)
		{
			case 1:
				{
					float range = m_TowerInfo.Stat_Skill02.Range;
					int layerMask = LayerMask.GetMask("Tower");

					Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);

					int buffCode = m_TowerInfo.Stat_Skill02.Buff_CC;
					BuffCC_TableExcel buffData = M_Buff.GetData(buffCode);
					float buffDuration = buffData.Duration;

					for (int i = 0; i < colliders.Length; ++i)
					{
						Tower tower = colliders[i].GetComponent<Tower>();

						if (this == tower)
							continue;

						if (null == tower)
						{
							Debug.Log(m_TowerInfo_Excel.Name_KR + "의 버프 스킬2 오류");
							continue;
						}

						#region 버프1
						if (buffData.BuffType1 != 0)
						{
							float buffRand1 = Random.Range(0.00001f, 1f);
							bool buffApply1 = buffRand1 <= buffData.BuffRand1;

							if (buffApply1)
							{
								S_Buff buff = new S_Buff(
									buffData.Name_KR + "_1",
									buffData.BuffType1,
									buffData.AddType1,
									buffData.BuffAmount1,
									buffData.BuffRand1,
									buffData.Duration,
									buffData.Prefab
									);

								tower.AddSkillBuff(buff, buffDuration);

								#region 버프2
								if (buffData.BuffType2 != 0)
								{
									float buffRand2 = Random.Range(0.00001f, 1f);
									bool buffApply2 = buffRand2 <= buffData.BuffRand2;

									if (buffApply2)
									{
										buff = new S_Buff(
											buffData.Name_KR + "_2",
											buffData.BuffType2,
											buffData.AddType2,
											buffData.BuffAmount2,
											buffData.BuffRand2,
											buffData.Duration,
											buffData.Prefab
											);

										tower.AddSkillBuff(buff, buffDuration);

										#region 버프3
										if (buffData.BuffType3 != 0)
										{
											float buffRand3 = Random.Range(0.00001f, 1f);
											bool buffApply3 = buffRand3 <= buffData.BuffRand3;

											if (buffApply3)
											{
												buff = new S_Buff(
													buffData.Name_KR + "_3",
													buffData.BuffType3,
													buffData.AddType3,
													buffData.BuffAmount3,
													buffData.BuffRand3,
													buffData.Duration,
													buffData.Prefab
													);

												tower.AddSkillBuff(buff, buffDuration);
											}
										}
										#endregion
									}
								}
								#endregion
							}
						}
						#endregion
					}

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(m_TowerInfo.Condition_Skill02.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			case 2:
				{
					if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget)
					{
						if (M_Enemy.GetEnemyList(Direction).Count <= 0)
							return;
					}

					if (null == m_Target_Skill02)
						return;

					// 스킬02 데이터 불러오기
					SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill02;
					SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill02;
					statData.Dmg_Fix += m_TowerInfo_Excel.Atk;

					#region 버프
					// 마왕 버프 추가
					List<S_Buff> BuffList_Fix = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					List<S_Buff> BuffList_Percent = new List<S_Buff>(m_TowerInfo.SkillBuffList_Percent);

					// 적 디버프 리스트
					List<S_Buff> debuffList = new List<S_Buff>(m_TowerInfo.SkillBuffList_Fix);
					debuffList.AddRange(m_TowerInfo.SkillBuffList_Percent);

					float Crit_rate_Fix = 0f;
					float Crit_rate_Percent = 1f;
					float Crit_Dmg_Fix = 0f;
					float Crit_Dmg_Percent = 1f;

					#region 시너지 버프, 버서커 버프 추가
					foreach (var item in m_TowerInfo.SynergyList)
					{
						switch ((E_SynergyEffectType)item.EffectType1)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode1);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}

						switch ((E_SynergyEffectType)item.EffectType2)
						{
							case E_SynergyEffectType.Buff:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
												BuffList_Percent.Add(buff);

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
														BuffList_Percent.Add(buff);

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
																BuffList_Percent.Add(buff);
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
							case E_SynergyEffectType.Berserker:
								{
									BuffCC_TableExcel buffData = M_Buff.GetData(item.EffectCode2);

									#region 버프1
									if (buffData.BuffType1 != 0)
									{
										float buffRand1 = Random.Range(0.00001f, 1f);
										bool buffApply1 = buffRand1 <= buffData.BuffRand1;

										if (buffApply1)
										{
											S_Buff buff = new S_Buff(
												buffData.Name_KR + "_1",
												buffData.BuffType1,
												buffData.AddType1,
												buffData.BuffAmount1 * m_TowerInfo.BerserkerStack,
												buffData.BuffRand1,
												buffData.Duration,
												buffData.Prefab
												);

											if (buff.AddType == E_AddType.Fix)
												BuffList_Fix.Add(buff);
											else if (buff.AddType == E_AddType.Percent)
											{
												buff.BuffAmount += buffData.BuffAmount1;
												BuffList_Percent.Add(buff);
											}

											#region 버프2
											if (buffData.BuffType2 != 0)
											{
												float buffRand2 = Random.Range(0.00001f, 1f);
												bool buffApply2 = buffRand2 <= buffData.BuffRand2;

												if (buffApply2)
												{
													buff = new S_Buff(
														buffData.Name_KR + "_2",
														buffData.BuffType2,
														buffData.AddType2,
														buffData.BuffAmount2 * m_TowerInfo.BerserkerStack,
														buffData.BuffRand2,
														buffData.Duration,
														buffData.Prefab
														);

													if (buff.AddType == E_AddType.Fix)
														BuffList_Fix.Add(buff);
													else if (buff.AddType == E_AddType.Percent)
													{
														buff.BuffAmount += buffData.BuffAmount2;
														BuffList_Percent.Add(buff);
													}

													#region 버프3
													if (buffData.BuffType3 != 0)
													{
														float buffRand3 = Random.Range(0.00001f, 1f);
														bool buffApply3 = buffRand3 <= buffData.BuffRand3;

														if (buffApply3)
														{
															buff = new S_Buff(
																buffData.Name_KR + "_3",
																buffData.BuffType3,
																buffData.AddType3,
																buffData.BuffAmount3 * m_TowerInfo.BerserkerStack,
																buffData.BuffRand3,
																buffData.Duration,
																buffData.Prefab
																);

															if (buff.AddType == E_AddType.Fix)
																BuffList_Fix.Add(buff);
															else if (buff.AddType == E_AddType.Percent)
															{
																buff.BuffAmount += buffData.BuffAmount3;
																BuffList_Percent.Add(buff);
															}
														}
													}
													#endregion
												}
											}
											#endregion
										}
									}
									#endregion
								}
								break;
						}
					}
					#endregion

					#region 합연산
					foreach (var item in BuffList_Fix)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Fix += BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Fix += BuffAmount;
								break;
						}
					}
					#endregion
					#region 곱연산
					foreach (var item in BuffList_Percent)
					{
						float BuffAmount = item.BuffAmount;

						switch (item.BuffType)
						{
							case E_BuffType.Atk:
								statData.Dmg_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_rate:
								Crit_rate_Percent *= BuffAmount;
								break;
							case E_BuffType.Crit_Dmg:
								Crit_Dmg_Percent *= BuffAmount;
								break;
						}
					}
					#endregion
					#endregion

					#region 크리티컬
					float Crit_rate = (m_TowerInfo_Excel.Crit_rate + Crit_rate_Fix) * Crit_rate_Percent;
					float Crit_Dmg = (m_TowerInfo_Excel.Crit_Dmg + Crit_Dmg_Fix) * Crit_Dmg_Percent;
					S_Critical critical = new S_Critical(Crit_rate, Crit_Dmg);
					#endregion

					#region 공격
					// 스킬02 투사체 코드
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
							critical,
							debuffList
							);
					}

					if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget)
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
					#endregion

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(conditionData.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			case 3:
				{
					int buffCode = m_TowerInfo.Stat_Skill02.Buff_CC;
					BuffCC_TableExcel buffData = M_Buff.GetData(buffCode);
					float buffDuration = buffData.Duration;

					#region 버프1
					if (buffData.BuffType1 != 0)
					{
						float buffRand1 = Random.Range(0.00001f, 1f);
						bool buffApply1 = buffRand1 <= buffData.BuffRand1;

						if (buffApply1)
						{
							S_Buff buff = new S_Buff(
								buffData.Name_KR + "_1",
								buffData.BuffType1,
								buffData.AddType1,
								buffData.BuffAmount1,
								buffData.BuffRand1,
								buffData.Duration,
								buffData.Prefab
								);

							AddSkillBuff(buff, buffDuration);

							#region 버프2
							if (buffData.BuffType2 != 0)
							{
								float buffRand2 = Random.Range(0.00001f, 1f);
								bool buffApply2 = buffRand2 <= buffData.BuffRand2;

								if (buffApply2)
								{
									buff = new S_Buff(
										buffData.Name_KR + "_2",
										buffData.BuffType2,
										buffData.AddType2,
										buffData.BuffAmount2,
										buffData.BuffRand2,
										buffData.Duration,
										buffData.Prefab
										);

									AddSkillBuff(buff, buffDuration);

									#region 버프3
									if (buffData.BuffType3 != 0)
									{
										float buffRand3 = Random.Range(0.00001f, 1f);
										bool buffApply3 = buffRand3 <= buffData.BuffRand3;

										if (buffApply3)
										{
											buff = new S_Buff(
												buffData.Name_KR + "_3",
												buffData.BuffType3,
												buffData.AddType3,
												buffData.BuffAmount3,
												buffData.BuffRand3,
												buffData.Duration,
												buffData.Prefab
												);

											AddSkillBuff(buff, buffDuration);
										}
									}
									#endregion
								}
							}
							#endregion
						}
					}
					#endregion

					#region 공격 이펙트
					Effect atkEffect = M_Effect.SpawnEffect(m_TowerInfo.Condition_Skill02.Atk_prefab);
					if (null != atkEffect)
					{
						atkEffect.transform.position = m_TowerInfo.AttackPivot.position;
						atkEffect.gameObject.SetActive(true);
					}
					#endregion
				}
				break;
			default:
				{
					Debug.LogError("스킬01 Ally 에러");
				}
				break;
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
	public void AddSkillBuff(S_Buff buff, float time)
	{
		if (buff.AddType == E_AddType.Fix)
			StartCoroutine(Co_SkillBuff_Fix(buff, time));
		else if (buff.AddType == E_AddType.Percent)
			StartCoroutine(Co_SkillBuff_Percent(buff, time));
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
		public bool CanAttack_Default;
		public bool CanAttack_Skill01;
		public bool CanAttack_Skill02;

		#region 기본 스킬
		// 기본 스킬 엑셀 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;
		#endregion
		#region 스킬01
		// 스킬01 엑셀 데이터
		public SkillCondition_TableExcel Condition_Skill01;
		public SkillStat_TableExcel Stat_Skill01;
		// 스킬01 공격 속도
		public float AttackSpeed_Skill01;
		// 스킬01 타이머
		public float AttackTimer_Skill01;
		#endregion
		#region 스킬02
		// 스킬02 엑셀 데이터
		public SkillCondition_TableExcel Condition_Skill02;
		public SkillStat_TableExcel Stat_Skill02;
		// 스킬02 공격 속도
		public float AttackSpeed_Skill02;
		// 스킬02 타이머
		public float AttackTimer_Skill02;
		#endregion

		#region 시너지
		[Space(10f)]
		// 시너지 리스트
		public List<Synergy_TableExcel> SynergyList;

		#region 버프
		// 합버프 리스트
		public LinkedList<S_Buff> SkillBuffList_Fix;
		// 곱버프 리스트
		public LinkedList<S_Buff> SkillBuffList_Percent;
		#endregion

		#region 공격 타입 변경
		public E_AttackType Buff_AttackType;
		public int Target_num;
		#endregion

		#region 버서커
		public bool Berserker;
		public int BerserkerStack;
		public int BerserkerMaxStack;
		#endregion
		#endregion
	}
}
