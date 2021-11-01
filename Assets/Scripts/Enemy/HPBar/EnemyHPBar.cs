using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Transform m_EnemyTransform;
    protected Image m_BackGround;
    protected Image m_Fill;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected EnemyHPBarManager M_EnemyHPBar => EnemyHPBarManager.Instance;
    protected Camera m_Camera => M_EnemyHPBar.m_HPBarCanvas.worldCamera;
    #endregion

    #region 외부 프로퍼티
    public float fillAmount { get => m_Fill.fillAmount; set => m_Fill.fillAmount = value; }
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public void Initialize()
    {
        if (null == m_BackGround)
        {
            m_BackGround = transform.Find("BackGround").GetComponent<Image>();
        }
        if (null == m_Fill)
        {
            m_Fill = transform.Find("Fill").GetComponent<Image>();
        }

        fillAmount = 1f;
    }
    #endregion

    #region 유니티 콜백 함수
    #endregion
}
