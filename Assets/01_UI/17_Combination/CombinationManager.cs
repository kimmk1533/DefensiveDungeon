using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : Singleton<CombinationManager>
{
	[SerializeField] Tower_TableExcelLoader m_towerLoader;
	private int MaxStar = 3;    // star maximum

	public event Action OnCombinationDespawnObjEvent;
	protected CombinationEffectManager M_Comeffect => CombinationEffectManager.Instance;
	bool m_DespawnWorldObjFlag;
	Vector3 endpos;
	
	public Vector3 EffectEndPos
	{
		get => endpos;
		set => endpos = value;
	}

	private bool IsMaximum(Tower tower)
	{
		if (tower.ExcelData.Star >= MaxStar)
			return true;
		return false;
	}


	// param : all towers in list need same code 
	private void CombinationProcess(List<Tower> tower_list)
	{
		CombinationEffect comeffect;
		// desapwn only 3 towers         
		int next_tower_code = tower_list[0].ExcelData.Next_Stat;

		// despawn
		for (int i = 0; i < 3; i++)
		{
			if (false == tower_list[i].IsOnInventory)
			{
				// Node
				TowerManager.Instance.DespawnTower(tower_list[i]);
				m_DespawnWorldObjFlag = true;
			}
			else
			{   // Inven
				//startpos = Camera.main.ScreenToWorldPoint(tower_list[i].transform.position);
				InventoryManager.Instance.RemoveTower(tower_list[i]);
			}
		}

		var newTowerData = m_towerLoader.DataList.Find((item)
			=>
		{ return item.Code == next_tower_code; });
        
		// spawn
		InventoryManager.Instance.AddNewTower(newTowerData);
	
		comeffect = M_Comeffect.SpawnEffect(CombinationEffectManager.PrefabCode2, endpos);
		if (null != comeffect)
		{
			comeffect.gameObject.SetActive(true);
			comeffect.Play(true);
		}
	}

	private bool CombinationRecurr()
	{
		Dictionary<int, List<Tower>> codeToCount_dic = new Dictionary<int, List<Tower>>();
		var tower_list = TowerManager.Instance.GetTowerList();
		foreach (var item in tower_list)
		{
			if (codeToCount_dic.ContainsKey(item.TowerCode))
			{
				codeToCount_dic[item.TowerCode].Add(item);
			}
			else
			{
				codeToCount_dic[item.TowerCode] = new List<Tower>();
				codeToCount_dic[item.TowerCode].Add(item);
			}
		}

		foreach (var item in codeToCount_dic)
		{
			if (item.Value.Count >= 3)
			{
				if (IsMaximum(item.Value[0]))
					continue;

				CombinationProcess(item.Value);
				return CombinationRecurr();
			}
		}

		return false;
	}

	public void Combinatnion()
	{
		CombinationRecurr();

		if (m_DespawnWorldObjFlag)
		{
			OnCombinationDespawnObjEvent?.Invoke();
			m_DespawnWorldObjFlag = false;
		}
	}
}
