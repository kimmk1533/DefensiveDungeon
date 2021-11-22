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
		BuffCC_TableExcel buffdata = M_Buff.GetData(arg.skillData.m_StatData.Buff_CC);
		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffdata.BuffRand1;
		List<Tower> dir_towerlist;
		//클릭한 곳 방향 넣기
		dir_towerlist = M_Tower.GetTowerList(arg.dir);
		if(buffApply)
		{
			S_Buff buff = new S_Buff(buffdata.Name_KR, buffdata.BuffType1,
				buffdata.AddType1, buffdata.BuffAmount1);
			if (buff.AddType == E_AddType.Fix)
			{
				for (int i = 0; i < dir_towerlist.Count; ++i)
				{
					dir_towerlist[i].AddDevilSkillBuff_Fix(buff, buffdata.Duration);
				}
			}
			else if (buff.AddType == E_AddType.Percent)
			{
				for (int i = 0; i < dir_towerlist.Count; ++i)
				{
					dir_towerlist[i].AddDevilSkillBuff_Percent(buff, buffdata.Duration);
				}
			}
		}
		yield break;
	}
	protected IEnumerator SK002(DevilSkillArg arg)
	{
		BuffCC_TableExcel buffdata = M_Buff.GetData(arg.skillData.m_StatData.Buff_CC);
		float buffRand = Random.Range(0.000001f, 1f);
		bool buffApply = buffRand <= buffdata.BuffRand1;
		List<Tower> towerlist = new List<Tower>();
		towerlist = M_Tower.GetTowerList();
		if (buffApply)
		{
			S_Buff buff = new S_Buff(buffdata.Name_KR, buffdata.BuffType1,
				buffdata.AddType1, buffdata.BuffAmount1);
			if (buff.AddType == E_AddType.Fix)
			{
				for(int i=0;i<towerlist.Count;++i)
				{
					towerlist[i].AddDevilSkillBuff_Fix(buff, buffdata.Duration);
				}
			}
			else if(buff.AddType==E_AddType.Percent)
			{
				for(int i=0;i<towerlist.Count;++i)
				{
					towerlist[i].AddDevilSkillBuff_Percent(buff, buffdata.Duration);
				}
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
