using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    protected Enemy_TableExcelLoader m_EnemyData => M_DataTable.GetDataTable<Enemy_TableExcelLoader>();

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    //방향별 나온 몬스터 저장하는 리스트
    public Dictionary<E_Direction, List<Enemy>> Enemy_Direction;

    //전체 몬스터
    public List<Enemy> All_Enemy;

    private void Awake()
    {
        All_Enemy = new List<Enemy>();

        Enemy_Direction = new Dictionary<E_Direction, List<Enemy>>();

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            Enemy_Direction[i] = new List<Enemy>();
        }
    }

    public Enemy_TableExcel GetData(int code)
    {
        Enemy_TableExcel origin = m_EnemyData.DataList.Where(item => item.Code == code).Single();
        return origin;
    }

    //전체 몬스터 데이터 뽑아오는 함수
    public List<Enemy> GetEnemyList()
    {
        return All_Enemy;
    }

    //각 방위 전체 몬스터 뽑아오는 함수
    public List<Enemy> GetEnemyList(E_Direction direc)
    {
        return Enemy_Direction[direc];
    }
}
