using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAnimator : MonoBehaviour
{
	protected Devil m_Devil;

	#region 내부 컴포넌트
	protected Animator m_Animator;
	#endregion
	#region 내부 프로퍼티
	protected Animator animator
	{
		get
		{
			if (m_Animator == null)
				m_Animator = GetComponent<Animator>();

			return m_Animator;
		}
	}
	#endregion

	#region 외부 함수
	public void Initialize(Devil devil)
	{
		m_Devil = devil;

		m_Devil.OnLostDefaultTargetEvent += new Action(() =>
		{
			animator.ResetTrigger("Attack");
		});
	}

	public void SetTrigger(string name)
	{
		animator.SetTrigger(name);
	}
	public void CallAttack()
	{
		m_Devil.CallAttack();
	}
	public void CallSkill01()
	{
		// 마왕 스킬1 투사체 발사
		//m_Devil.CallSkill01();
	}
	public void CallSkill02()
	{
		// 마왕 스킬2 투사체 발사
		//m_Devil.CallSkill02();
	}
	public void CallDie()
	{
		m_Devil.CallDie();
	}
	#endregion
}
