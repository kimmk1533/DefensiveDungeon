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

	protected IEnumerator SK001(DevilSkillArg arg)
	{
		yield return null;
	}
	protected IEnumerator SK002(DevilSkillArg arg)
	{
		yield return null;
	}
	#endregion

	#region 유니티 콜백 함수
	private void Awake()
	{
		InitializeDevil(E_Devil.HateQueen);
	}
	#endregion
}
