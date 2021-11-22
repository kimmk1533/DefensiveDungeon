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

	#region 외부 함수
	public void Initialize(Tower tower)
	{
		m_Tower = tower;
		m_Animator = GetComponent<Animator>();

		m_Tower.OnLostDefaultTargetEvent += new Action(() =>
		{
			m_Tower.CanAttack_Skill = true;
			m_Animator.ResetTrigger("Attack");
		});
		m_Tower.OnLostSkill01TargetEvent += new Action(() =>
		{
			m_Tower.CanAttack_Skill = true;
			m_Animator.ResetTrigger("Skill01");
		});
		m_Tower.OnLostSkill02TargetEvent += new Action(() =>
		{
			m_Tower.CanAttack_Skill = true;
			m_Animator.ResetTrigger("Skill02");
		});
	}

	public void SetFloat(string name, float value)
	{
		m_Animator.SetFloat(name, value);
	}
	public void SetTrigger(string name)
	{
		m_Animator.SetTrigger(name);
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
