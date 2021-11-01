using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverDataLine : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_nameTextpro;
    [SerializeField] TMPro.TextMeshProUGUI m_dataTextpro;

    public void SetUI(string name, int exp)
    {
        m_nameTextpro.text = name;
        m_dataTextpro.text = ": " + " " + exp.ToString() + "EXP";
    }
}
