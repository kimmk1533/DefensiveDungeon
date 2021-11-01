using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAnimator : MonoBehaviour
{
    protected Devil m_Devil;

    #region 내부 컴포넌트
    protected Animator m_animator;
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    public Animator m_Animator
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

    public void SetDevil(Devil devil)
    {
        m_Devil = devil;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        // m_Devil = transform.parent.GetComponent<Devil>();
    }
    #endregion
}
