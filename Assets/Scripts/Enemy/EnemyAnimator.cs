using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
	#region 내부 컴포넌트
	protected Enemy m_Enemy;
	protected Animator m_Animator;
	#endregion

	#region 외부 함수
	public void Initialize(Enemy enemy)
	{
		m_Enemy = enemy;
		m_Animator = GetComponent<Animator>();
	}

	public void SetBool(string name, bool value)
	{
		m_Animator.SetBool(name, value);
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
		m_Enemy.CallAttack();
	}
	public void CallSkill()
	{
		m_Enemy.CallSkill();
	}
	public void CallDie()
	{
		m_Enemy.CallDie();
	}
	#endregion
}
