using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankRateUIController : MonoBehaviour
{
    [SerializeField] Color m_color;
    TMPro.TextMeshProUGUI m_textPro;

    public void Initialize()
    {
        m_textPro = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void SetUI(string text)
    {
        m_textPro.text = text;
        m_textPro.color = m_color;
    }
}
