using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipRightDataLine : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_stat_name_textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_stat_data_textpro;

    public void SetUI(string name ,string data)
    {
        m_stat_name_textpro.text = name;
        m_stat_data_textpro.text = data;
    }
}
