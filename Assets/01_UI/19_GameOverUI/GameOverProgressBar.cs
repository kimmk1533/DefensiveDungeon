using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverProgressBar : MonoBehaviour
{
    [SerializeField] Slider m_progressBar;
    [SerializeField] TMPro.TextMeshProUGUI m_progressTextpro;

    public void SetUI(int current_exp, int max_exp)
    {
        float rate = (float)current_exp / max_exp;
        m_progressBar.value = rate;
        m_progressTextpro.text = current_exp.ToString()
            + "/" + max_exp.ToString();
    }
}
