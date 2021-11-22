using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
	protected Enemy_TableExcelLoader m_EnemyData;

	// 전체 몬스터
	protected List<Enemy> m_EnemyList;
	// 방향별 나온 몬스터 저장하는 리스트
	protected Dictionary<E_Direction, List<Enemy>> m_DirEnemyList;

	#region 내부 프로퍼티
	#region 매니저
	protected EnemyPool M_EnemyPool => EnemyPool.Instance;
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	#endregion

	protected Enemy_TableExcelLoader EnemyData
	{
		get
		{
			if (null == m_EnemyData)
			{
				m_EnemyData = M_DataTable.GetDataTable<Enemy_TableExcelLoader>();
			}

			return m_EnemyData;
		}
	}
	#endregion

	#region 외부 함수
	public Enemy SpawnEnemy(E_Direction dir, int code)
	{
		string key = GetData(code).Name_EN;
		Enemy enemy = M_EnemyPool.GetPool(key).Spawn();
		enemy.InitializeEnemy(code, dir);

		m_EnemyList.Add(enemy);
		m_DirEnemyList[dir].Add(enemy);
		return enemy;
	}
	public void Despawn(Enemy enemy)
	{
		m_DirEnemyList[enemy.Direction].Remove(enemy);
		m_EnemyList.Remove(enemy);
		enemy.FinializeEnemy();
		M_EnemyPool.GetPool(enemy.Get_EnemyName_EN).DeSpawn(enemy);
	}

	public Enemy_TableExcel GetData(int code)
	{
		Enemy_TableExcel origin = EnemyData.DataList.Where(item => item.Code == code).Single();
		return origin;
	}
	// 전체 몬스터 데이터 뽑아오는 함수
	public List<Enemy> GetEnemyList()
	{
		return m_EnemyList;
	}
	// 각 방위 전체 몬스터 뽑아오는 함수
	public List<Enemy> GetEnemyList(E_Direction direc)
	{
		return m_DirEnemyList[direc];
	}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		m_EnemyData = M_DataTable.GetDataTable<Enemy_TableExcelLoader>();

		m_EnemyList = new List<Enemy>();
		m_DirEnemyList = new Dictionary<E_Direction, List<Enemy>>();

		for (E_Direction i = 0; i < E_Direction.Max; ++i)
		{
			m_DirEnemyList[i] = new List<Enemy>();
		}
	}
	#endregion
}