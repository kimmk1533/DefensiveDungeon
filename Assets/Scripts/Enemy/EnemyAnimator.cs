using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public Enemy m_Enemy;
    public Animator m_animator;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
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

    #region 유니티 콜백 함수
    void Awake()
    {
        m_Enemy = transform.parent.GetComponent<Enemy>();
        m_animator = GetComponent<Animator>();
    }
    #endregion
}
