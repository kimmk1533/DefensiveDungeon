using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopStatusUIController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_level_text;

    // 확률 표기할 root panel 
    [SerializeField] Image m_percentage_root_panel;
    List<RankRateUIController> m_rankRateUIs = null;
    private void Awake()
    {
        m_rankRateUIs = new List<RankRateUIController>();
        // 확률 표기할 child panel 가져오기
        // 좌측부터 오도록 정렬
        m_rankRateUIs.AddRange(m_percentage_root_panel.GetComponentsInChildren<RankRateUIController>());
        m_rankRateUIs.Sort(
            (item1, item2) =>
        { return item1.transform.position.x.CompareTo(item2.transform.position.x); }
        );
    }

    // index 는 0부터
    public void SetRates(float[] rates)
    {
        try
        {
            for (int i = 0; i < rates.Length; i++)
            {
                m_rankRateUIs[i].SetUI("● " + (rates[i] * 100).ToString() + "%");
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("SHOP Percentage Panel : 데이터 테이블 개수와 표기할 텍스트 개수가 불일치함");
            throw;
        }        
    }

    public void SetLevel(int level)
    {
        m_level_text.text = "Level " + level.ToString();
    }
}
