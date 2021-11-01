using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage_EnemyDataManager : Singleton<Stage_EnemyDataManager>
{
    protected StageEnemy_TableExcelLoader m_StageEnemyData;

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    private void Awake()
    {
        m_StageEnemyData = M_DataTable.GetDataTable<StageEnemy_TableExcelLoader>();
    }

    public StageEnemy_TableExcel GetData(int code)
    {
        StageEnemy_TableExcel origin = m_StageEnemyData.DataList.Where(item => item.Code == code).Single();
        return origin;
    }

    public List<StageEnemy_TableExcel> GetListData(int code)
    {
        List<StageEnemy_TableExcel> origin = new List<StageEnemy_TableExcel>();

        for (int i = 0; i < m_StageEnemyData.DataList.Count; i++)
        {
            if (m_StageEnemyData.DataList[i].Code == code)
            {
                origin.Add(m_StageEnemyData.DataList[i]);
            }
        }
        
        return origin;
    }
}
