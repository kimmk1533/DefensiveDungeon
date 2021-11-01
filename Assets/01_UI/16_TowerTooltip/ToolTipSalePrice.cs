using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSalePrice : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_price_text;
    int m_price;
    public void SetUI(int price)
    {
        m_price = price;
        m_price_text.text = price.ToString();
    }

    public void __OnSaleButtonClicked()
    {
        Debug.Log("Sell Button Clicked");
        TowerToolTipManager.Instance.__OnSaleButtonClicked(m_price);
    }
}
