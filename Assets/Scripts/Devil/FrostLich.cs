using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostLich : Devil
{
	#region 내부 함수
	public override void CallSkill01()
	{

	}
	public override void CallSkill02()
	{

	}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		InitializeDevil(E_Devil.FrostLich);
	}
	#endregion
}
