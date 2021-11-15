using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    #region 내부 컴포넌트
    protected TextMeshProUGUI m_Text;
    protected Animator m_Animator;
    #endregion
    #region 내부 프로퍼티
    #region 매니저
    protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public string text { get => m_Text.text; set => m_Text.text = value; }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        if (null == m_Text)
        {
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (null == m_Animator)
        {
            m_Animator = GetComponent<Animator>();
        }
    }
    #endregion
    #region 이벤트 함수
    public void Despawn()
    {
        M_FloatingText.DespawnDamageText(this);
    }
    #endregion
}
