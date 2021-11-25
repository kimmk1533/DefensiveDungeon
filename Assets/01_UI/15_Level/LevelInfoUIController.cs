using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct LevelUIInfo
{
    public int curr_exp;
    public int max_exp;
    public int curr_level;
    public int requireGoldForPurchase;
}

public class LevelInfoUIController : MonoBehaviour , IPointerClickHandler
{

    [SerializeField] TMPro.TextMeshProUGUI m_level_textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_exp_textpro;

    [SerializeField] Image m_gold_image;
    [SerializeField] TMPro.TextMeshProUGUI m_gold_textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_purchace_textpro;
    [SerializeField] Slider m_exp_image;

    [SerializeField] LevelUIInfo m_info;

    bool m_max_level;

    private void Start()
    {
        m_info.curr_exp = 0; // temp
        m_info.max_exp = UserInfoManager.Instance.MaxEXP;
        m_info.curr_level = UserInfoManager.Instance.Level;
        m_info.requireGoldForPurchase = UserInfoManager.Instance.RequireGoldForPurchaseEXP;

        m_exp_image.value = 0f;

        m_max_level = false;

        UserInfoManager.Instance.OnLevelChanged += __OnLevelChanged;
        UserInfoManager.Instance.OnExpChangedEvent += __OnExpChanged;
        UserInfoManager.Instance.OnMaxLevelEvent_OnlyOnce += () =>
        {
            m_max_level = true;
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_max_level)
            return;

        __OnEXPPurchaseProcess();
    }

    public void __OnLevelChanged(int cur_level)
    {
        m_info.curr_level = cur_level;
        m_info.max_exp = UserInfoManager.Instance.MaxEXP;
        m_info.requireGoldForPurchase = UserInfoManager.Instance.RequireGoldForPurchaseEXP;

        __OnInfoChanged();
    }
    public void __OnExpChanged(int max_exp, int cur_exp)
    {
        m_info.max_exp = max_exp;
        m_info.curr_exp = cur_exp;

        __OnInfoChanged();
    }

    public void __OnEXPPurchaseProcess()
    {
        int require_gold = UserInfoManager.Instance.RequireGoldForPurchaseEXP;
        if (require_gold == 0) // NO_DATA
            return;

        if (false == UserInfoManager.Instance.UseGold(require_gold))
        {
            // not enough gold
            Debug.Log("not enough gold to purchase exp");
        }
        else
        {
            int increase_exp = UserInfoManager.Instance.IncrementOfPurchasingEXP;
            UserInfoManager.Instance.AddExp(increase_exp);
            Debug.Log($"Purchase EXP complete {increase_exp}");
        }

        __OnInfoChanged();
    }

    public void __OnInfoChanged()
    {
        if (m_max_level)
        {
            m_exp_image.value = 1f;
            m_exp_textpro.text = "EXP Max";
            m_gold_textpro.text = null;
            m_purchace_textpro.text = null;
            return;
        }

        m_level_textpro.text = "Level " + m_info.curr_level;
        m_exp_textpro.text = "EXP " + m_info.curr_exp.ToString() + "/" + m_info.max_exp.ToString();
        m_gold_textpro.text = m_info.requireGoldForPurchase.ToString();

        float rate = (float)m_info.curr_exp / m_info.max_exp;
        m_exp_image.value = rate;
    }
    
}
