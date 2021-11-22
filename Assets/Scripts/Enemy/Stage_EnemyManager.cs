using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_EnemyManager : Singleton<Stage_EnemyManager>
{
	[SerializeField]
	protected StageChangedEventArgs m_Now_StageData;
	protected Stage_TableExcelLoader m_StageData;
	protected StageEnemy_TableExcelLoader m_StageEnemyData;

	[SerializeField]
	protected SceneStartEventReciever m_SceneStartReciever;

	private List<List<StageEnemy_TableExcel>> m_StageEnemyInfo_Excel;

	#region 내부 프로퍼티
	#region 매니저
	protected Stage_EnemyDataManager M_StageEnemy => Stage_EnemyDataManager.Instance;
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected StageInfoManager M_StageInfo => StageInfoManager.Instance;
	protected SpawnManager M_Spawn => SpawnManager.Instance;
	#endregion

	protected Stage_TableExcelLoader StageData
	{
		get
		{
			if (null == m_StageData)
				m_StageData = M_DataTable.GetDataTable<Stage_TableExcelLoader>();

			return m_StageData;
		}
	}
	protected StageEnemy_TableExcelLoader StageEnemyData
	{
		get
		{
			if (null == m_StageEnemyData)
				m_StageEnemyData = M_DataTable.GetDataTable<StageEnemy_TableExcelLoader>();

			return m_StageEnemyData;
		}
	}
	#endregion

	#region 이벤트 함수
	public void OnStageChanged(StageChangedEventArgs args)
	{
		m_Now_StageData.stage_num = args.stage_num;
		m_Now_StageData.stageName = args.stageName;
		m_Now_StageData.stage_time = args.stage_time;
		m_Now_StageData.stage_type = args.stage_type;

		if (args.stage_type == 2)
		{
			List<StageEnemy_TableExcel> stageEnemy = m_StageEnemyInfo_Excel[m_Now_StageData.stage_num - 1];

			M_Spawn.Start_BattleStage(in stageEnemy);
		}
	}

	protected void Onreciever()
	{
		M_StageInfo.OnStageChangedEvent += OnStageChanged;
	}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		m_StageData = M_DataTable.GetDataTable<Stage_TableExcelLoader>();
		m_StageEnemyData = M_DataTable.GetDataTable<StageEnemy_TableExcelLoader>();

		m_StageEnemyInfo_Excel = new List<List<StageEnemy_TableExcel>>();
	}
	void Start()
	{
		foreach (var item in m_StageData.DataList)
		{
			m_StageEnemyInfo_Excel.Add(M_StageEnemy.GetListData(item.StageEnemyTable));
		}

		m_SceneStartReciever.m_scene_start_event.AddListener(Onreciever);
	}
	#endregion
}