using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoldUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_textPro;

    private void Start()
    {
        UserInfoManager.Instance.OnGoldChangedEvent += OnGoldChanged;
        UserInfoManager.Instance.UpdateAllInfo();
    }
    public void OnGoldChanged(int current_gold)
    {
        m_textPro.text = current_gold.ToString();
    }
}
