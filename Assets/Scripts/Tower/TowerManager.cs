using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
	protected Tower_TableExcelLoader m_TowerData;

	protected List<Tower> m_TowerList;
	protected Dictionary<E_Direction, List<Tower>> m_DirTowerList;

	#region 내부 프로퍼티
	#region 매니저
	protected TowerPool M_TowerPool => TowerPool.Instance;
	protected NodeManager M_Node => NodeManager.Instance;
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	#endregion

	protected Tower_TableExcelLoader TowerData
	{
		get
		{
			if (null == m_TowerData)
			{
				m_TowerData = M_DataTable.GetDataTable<Tower_TableExcelLoader>();
			}

			return m_TowerData;
		}
	}
	#endregion

	#region 외부 함수
	public Tower SpawnTower_Inventory(int code)
	{
		// only spawn in inventory
		// do not call this function other class
		string key = GetData(code).Name_EN;
		Tower spawn = M_TowerPool.GetPool(key).Spawn();
		spawn.InitializeTower(code);
		spawn.IsOnInventory = true;
		m_TowerList.Add(spawn);
		return spawn;
	}
	public void SpawnTower_Node(E_Direction direction, Tower tower)
	{
		m_DirTowerList[direction].Add(tower);
	}
	public void DespawnTower(Tower tower)
	{   // only on NODE
		// cha
		var tower_pool = M_TowerPool.GetPool(tower.Name);
		m_TowerList.Remove(tower);
		m_DirTowerList[tower.Direction].Remove(tower);
		tower.FinializeTower();
		tower_pool.DeSpawn(tower);
	}

	public Tower_TableExcel GetData(int code)
	{
		Tower_TableExcel result = TowerData.DataList
			.Where(item => item.Code == code).SingleOrDefault();

		return result;
	}
	public List<Tower> GetTowerList()
	{
		return m_TowerList;
	}
	public List<Tower> GetTowerList(E_Direction dir)
	{
		return m_DirTowerList[dir];
	}
	public void UpdateTowerList()
	{
		for (E_Direction i = 0; i < E_Direction.Max; i++)
		{
			m_DirTowerList[i].Clear();
			UpdateTowerList(i);
		}
	}
	public void UpdateTowerList(E_Direction dir)
	{
		List<Node> nodeList = M_Node.GetNodeList(dir);

		foreach (var item in nodeList)
		{
			if (item.m_Tower != null)
			{
				m_DirTowerList[dir].Add(item.m_Tower);
			}
		}
	}

	//public int GetSameTowerCount(int tower_code)
	//{
	//	return m_TowerList.FindAll((item) =>
	//	{ return item.TowerCode == tower_code; }).Count;
	//}
	//public Tower[] GetTowers(int tower_code)
	//{
	//	return m_TowerList.FindAll((item) =>
	//	{ return item.TowerCode == tower_code; }).ToArray();
	//}
	//public Tower_TableExcel GetTower(int kind, int star)
	//{
	//	Tower_TableExcel result = TowerData.DataList
	//		.Where(item => item.Tower_Kinds == kind && item.Star == star)
	//		.SingleOrDefault();

	//	return result;
	//}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		m_TowerData = M_DataTable.GetDataTable<Tower_TableExcelLoader>();

		m_TowerList = new List<Tower>();
		m_DirTowerList = new Dictionary<E_Direction, List<Tower>>();

		for (E_Direction i = 0; i < E_Direction.Max; ++i)
		{
			m_DirTowerList.Add(i, new List<Tower>());
		}

		M_Node.OnRotateEndEvent += UpdateTowerList;
	}
	#endregion
}