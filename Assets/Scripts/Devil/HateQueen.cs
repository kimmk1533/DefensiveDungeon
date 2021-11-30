using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HateQueen : Devil
{
	#region 내부 함수
	public override void CallSkill01()
	{
		BuffCC_TableExcel buffData = M_Buff.GetData(m_DevilInfo.m_Skill01.m_StatData.Buff_CC);

		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffData.BuffRand1;

		if (buffApply)
		{
			S_Buff buff = new S_Buff(
				buffData.Name_KR,
				buffData.BuffType1,
				buffData.AddType1,
				buffData.BuffAmount1,
				buffData.BuffRand1,
				buffData.Duration,
				buffData.Prefab
				);

			//클릭한 곳 방향 넣기
			List<Tower> dir_towerlist = M_Tower.GetTowerList(m_DevilInfo.m_Skill01.m_Direction);
			for (int i = 0; i < dir_towerlist.Count; ++i)
			{
				dir_towerlist[i].AddSkillBuff(buff, buffData.Duration);

				// 이펙트 생성
				Effect towerEffect = M_Effect.SpawnEffect(Skill01.m_ConditionData.damage_prefab);
				if (null != towerEffect)
				{
					towerEffect.transform.position = dir_towerlist[i].transform.position;
					towerEffect.gameObject.SetActive(true);
				}
			}
		}

		// 이펙트 생성
		Effect skillEffect = M_Effect.SpawnEffect(Skill01.m_ConditionData.Atk_prefab);
		if (null != skillEffect)
		{
			skillEffect.transform.position = transform.position;
			skillEffect.gameObject.SetActive(true);
		}

		m_DevilInfo.RotateSpeed = 5f;
	}
	public override void CallSkill02()
	{
		BuffCC_TableExcel buffData = M_Buff.GetData(m_DevilInfo.m_Skill02.m_StatData.Buff_CC);

		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffData.BuffRand1;

		if (buffApply)
		{
			S_Buff buff = new S_Buff(
				buffData.Name_KR,
				buffData.BuffType1,
				buffData.AddType1,
				buffData.BuffAmount1,
				buffData.BuffRand1,
				buffData.Duration,
				buffData.Prefab
				);

			List<Tower> towerlist = M_Tower.GetTowerList();
			for (int i = 0; i < towerlist.Count; ++i)
			{
				if (!towerlist[i].IsOnInventory)
				{
					towerlist[i].AddSkillBuff(buff, buffData.Duration);

					// 이펙트 생성
					Effect towerEffect = M_Effect.SpawnEffect(Skill01.m_ConditionData.damage_prefab);
					if (null != towerEffect)
					{
						towerEffect.transform.position = towerlist[i].transform.position;
						towerEffect.gameObject.SetActive(true);
					}
				}
			}
		}

		// 이펙트 생성
		Effect skillEffect = M_Effect.SpawnEffect(Skill02.m_ConditionData.Atk_prefab);
		if (null != skillEffect)
		{
			skillEffect.transform.position = transform.position;
			skillEffect.gameObject.SetActive(true);
		}

		m_DevilInfo.RotateSpeed = 5f;
	}
	#endregion

	#region 유니티 콜백 함수
	private void Awake()
	{
		InitializeDevil(E_Devil.HateQueen);
	}
	#endregion
}
