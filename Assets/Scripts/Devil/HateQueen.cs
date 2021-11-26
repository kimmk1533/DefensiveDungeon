using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HateQueen : Devil
{
	#region 내부 함수
	protected override void DoSkill01(DevilSkillArg arg)
	{
		StartCoroutine(SK001(arg));
	}
	protected override void DoSkill02(DevilSkillArg arg)
	{
		StartCoroutine(SK002(arg));
	}
	//이거 스킬 구현된거 지우기
	protected IEnumerator SK001(DevilSkillArg arg)
	{
		BuffCC_TableExcel buffData = M_Buff.GetData(m_DevilInfo.m_Skill01.m_StatData.Buff_CC);
		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffData.BuffRand1;
		//클릭한 곳 방향 넣기
		List<Tower> dir_towerlist = M_Tower.GetTowerList(arg.dir);
		if(buffApply)
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

			for (int i = 0; i < dir_towerlist.Count; ++i)
			{
				dir_towerlist[i].AddSkillBuff(buff, buffData.Duration);
			}
		}
		yield break;
	}
	protected IEnumerator SK002(DevilSkillArg arg)
	{
		BuffCC_TableExcel buffData = M_Buff.GetData(m_DevilInfo.m_Skill02.m_StatData.Buff_CC);
		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffData.BuffRand1;
		List<Tower> towerlist = M_Tower.GetTowerList();
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

			for (int i = 0; i < towerlist.Count; ++i)
			{
				towerlist[i].AddSkillBuff(buff, buffData.Duration);
			}
		}
		yield break;
	}
	#endregion

	#region 유니티 콜백 함수
	private void Awake()
	{
		InitializeDevil(E_Devil.HateQueen);
	}
	#endregion
}
