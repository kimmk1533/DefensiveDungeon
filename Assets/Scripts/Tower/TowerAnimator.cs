using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
    protected Tower m_Tower;

    #region 내부 컴포넌트
    protected Animator m_animator;
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    public Animator m_Animator // => m_animator ??= GetComponent<Animator>();
    { 
        get
        {
            if (m_animator == null)
                m_animator = GetComponent<Animator>();

            return m_animator;
        }
    }
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
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

    #region 유니티 콜백 함수
    void Awake()
    {
        m_Tower = transform.parent.GetComponent<Tower>();
        //m_animator = gameObject.AddComponent<Animator>();
    }
    #endregion
}
