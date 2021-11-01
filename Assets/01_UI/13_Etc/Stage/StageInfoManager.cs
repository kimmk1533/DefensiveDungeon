using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct StageChangedEventArgs
{
    public string stageName;    // stage name 
    public int stage_num;       // current stage num
    public int stage_type;      // break , battle etc
    public float stage_time;    // stage total time
}

public class StageInfoManager : Singleton<StageInfoManager>
{
    public delegate void StageChangeHandler(StageChangedEventArgs args);
    public event StageChangeHandler OnStageChangedEvent;
    public delegate void TimerUpdateHandler(float remainTime, float progress);
    public event TimerUpdateHandler OnTimeChangedEvent;

    [SerializeField] Stage_TableExcel m_current_stageInfo;
    [SerializeField] int current_stage;
    [SerializeField] Stage_TableExcelLoader m_excel_loader;

    [SerializeField] bool m_startFlag;
    [SerializeField] float m_timer;

    // ( 0.0f ~ 1.0f )
    public float Progress
    {
        get
        {
            return (1.0f - (m_current_stageInfo.StageTime - m_timer) / m_current_stageInfo.StageTime);
        }
    }

    // when scene loaded complete
    // this function must be called by Scene start evnet reciever    
    public void __StartTimer()
    {
        Debug.Log("Stage Timer Start");
        current_stage = 0;
        m_startFlag = true;
        m_timer = 0.0f;
        GoNextStage();
    }    

    private void Update()
    {
        // if time check flag is true
        if (m_startFlag)
        {   // time check
            m_timer += Time.deltaTime;

            float remain_time = m_current_stageInfo.StageTime - m_timer;
            OnTimeChangedEvent?.Invoke(remain_time, Progress);

            if(Progress >= 1.0f)
            {
                m_timer = 0.0f;
                GoNextStage();
            }
        }
    }

    public void GoNextStage()
    {        
        m_current_stageInfo = m_excel_loader.DataList[current_stage];
        ++current_stage;

        // synergy bonus gold
         int bonus_gold = SynergyManager.Instance.BonusGold;

        // user info update
        UserInfoManager.Instance.AddGold(m_current_stageInfo.Gold + bonus_gold);
        UserInfoManager.Instance.AddExp(m_current_stageInfo.Exp);
        

        // else
        OnStageChangedEvent?.Invoke(
            new StageChangedEventArgs
            {
                stageName = m_current_stageInfo.Name_EN,
                stage_num = m_current_stageInfo.Stage_Num,
                stage_type = m_current_stageInfo.StageType,
                stage_time = m_current_stageInfo.StageTime,
            });
    }
}
