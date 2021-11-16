using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
	protected Tower m_Tower;

	#region 내부 컴포넌트
	protected Animator m_Animator;
	#endregion
	#region 내부 프로퍼티
	protected Animator animator // => m_Animator ??= GetComponent<Animator>();
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
	public void Initialize(Tower tower)
	{
		m_Tower = tower;

		m_Tower.OnLostDefaultTargetEvent += new Action(() =>
		{
			animator.ResetTrigger("Attack");
		});
		m_Tower.OnLostSkill01TargetEvent += new Action(() =>
		{
			animator.ResetTrigger("Skill01");
		});
		m_Tower.OnLostSkill02TargetEvent += new Action(() =>
		{
			animator.ResetTrigger("Skill02");
		});
	}

	public void SetTrigger(string name)
	{
		animator.SetTrigger(name);
	}
	public void CallAttack()
	{
		m_Tower.CallAttack();
	}
	public void CallSkill01()
	{
		m_Tower.CallSkill01();
	}
	public void CallSkill02()
	{
		m_Tower.CallSkill02();
	}
	#endregion
}
