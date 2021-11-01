using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellLord : Devil
{
	#region 내부 함수
	protected override void DoSkill01(DevilSkillArg arg)
	{
		
	}
	protected override void DoSkill02(DevilSkillArg arg)
	{
		
	}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		InitializeDevil(E_Devil.HellLord);
	}
	#endregion
}
