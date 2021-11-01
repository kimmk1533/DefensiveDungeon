using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_EnemyManager : Singleton<Stage_EnemyManager>
{
    [SerializeField] StageChangedEventArgs Now_StageData;

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    private Stage_TableExcelLoader stage_excel_loader => M_DataTable.GetDataTable<Stage_TableExcelLoader>();

    [SerializeField] SceneStartEventReciever reciever;

    private int monsterCode;
    private bool stagechange = false;

    void Start()
    {
        reciever.m_scene_start_event.AddListener(Onreciever);
    }

    public void Onreciever()
    {
        StageInfoManager.Instance.OnTimeChangedEvent += OnCountChanged;
        StageInfoManager.Instance.OnStageChangedEvent += OnStageChanged;
    }

    public void OnStageChanged(StageChangedEventArgs args)
    {
        Now_StageData.stage_num = args.stage_num;
        Now_StageData.stageName = args.stageName;
        Now_StageData.stage_time = args.stage_time;
        Now_StageData.stage_type = args.stage_type;

        stagechange = false;
    }

    public void OnCountChanged(float remainTime, float progress)
    {
        if (!stagechange)
        {
            if (Now_StageData.stage_num % 2 == 0)
            {
                M_End_Time();
                stagechange = true;
            }
        }
    }

    public void M_End_Time()
    {
        if (Now_StageData.stage_num != 0)
        {
            monsterCode = stage_excel_loader.DataList[Now_StageData.stage_num - 1].StageEnemyTable;
            SpawnManager.Instance.Start_Stage(monsterCode);
        }
    }
}
