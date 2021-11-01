using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserInfo
{
    public int SelectedDevilCode;
    public int level;
    public int exp;
    public int max_exp;
    public int gold;

}

[DefaultExecutionOrder(-88)]
public class UserInfoManager : Singleton<UserInfoManager>
{
    public delegate void LevelChangeHandler(int current_level);
    public event LevelChangeHandler OnLevelChanged;
    public delegate void GoldChangeHandler(int current_gold);
    public event GoldChangeHandler OnGoldChangedEvent;
    public delegate void ExpChangeHandler(int max_exp, int curr_epx);
    public event ExpChangeHandler OnExpChangedEvent;

    [SerializeField] UserInfo m_info;
    [SerializeField] Level_TableExcelLoader m_levelLadoer;
    [SerializeField] Stage_TableExcelLoader m_stageLoader;

    public int DevilCode { get { return m_info.SelectedDevilCode; } }
    public int Level { get { return m_info.level; } }
    public int Gold { get { return m_info.gold; } }
    public int EXP { get { return m_info.exp; } }
    public int MaxEXP { get { return m_info.max_exp; } }

    // if has no data , RETURN 0
    public int RequireGoldForPurchaseEXP
    {   // gold amount of purchase exp        
        get
        {
            if (m_info.level >= m_levelLadoer.DataList.Count)
                return 0;  // No Data
            var cur_level_data = m_levelLadoer.DataList[m_info.level - 1];
            return cur_level_data.Buy_Gold;
        }
    }
    public int IncrementOfPurchasingEXP
    {   // when you purchase exp with gold
        // increment of current exp
        get
        {
            var cur_level_data = m_levelLadoer.DataList[m_info.level - 1];
            return cur_level_data.Exp_Buy;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Level == 0)
        {
            m_info.level = 1;
            m_info.exp = 0;
            m_info.max_exp = m_levelLadoer.DataList[Level - 1].LvUP_Exp;
        }
        else
        {
            // TODO : if have user file data...
        }

        if(DevilCode == 0)
        {
            m_info.SelectedDevilCode = 100001;
        }
    }
    private void Start()
    {

    }

    public void UpdateAllInfo()
    {
        OnLevelChanged?.Invoke(m_info.level);
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }

    public void SetDevilCode(int code)
    {
        m_info.SelectedDevilCode = code;
    }

    public void ResetLevel()
    {
        m_info.level = 1;
        OnLevelChanged?.Invoke(m_info.level);
    }
    public void ResetGold()
    {
        m_info.gold = 0;
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }

    public void AddLevel(int val)
    {
        m_info.level += val;
        OnLevelChanged?.Invoke(m_info.level);
    }

    public void AddGold(int val)
    {
        m_info.gold += val;
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }
    public bool UseGold(int val)
    {
        if (val > Gold)
            return false;

        m_info.gold -= val;
        OnGoldChangedEvent?.Invoke(m_info.gold);

        return true;
    }

    public void AddExp(int val)
    {
        // Get current Require EXP       
        m_info.exp = m_info.exp + val;
        if (EXP >= MaxEXP)
        {   // if over max EXP
            // level up
            m_info.exp = EXP - MaxEXP;
            AddLevel(1);
            OnLevelChanged?.Invoke(Level);
        }
        OnExpChangedEvent?.Invoke(MaxEXP, EXP);
    }
}
