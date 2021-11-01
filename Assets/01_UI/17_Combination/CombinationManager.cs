using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : Singleton<CombinationManager>
{
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    private int MaxStar = 3;    // star maximum

    public delegate void CombinationEndHnadler(bool DespawnWorldObjFlag);
    public event CombinationEndHnadler OnCombinationCompleteEvent;

    bool m_DespawnWorldObjFlag;

    private bool IsMaximum(Tower tower)
    {
        if (tower.ExcelData.Star >= MaxStar)
            return true;
        return false;
    }


    // param : all towers in list need same code 
    private void CombinationProcess(List<Tower> tower_list)
    {
        Debug.Log("Combination process");

        // desapwn only 3 towers         
        int next_tower_code = tower_list[0].ExcelData.Next_Stat;        

        // despawn
        for (int i = 0; i < 3; i++)
        {
            if(false ==tower_list[i].m_TowerInfo.IsOnInventory)
            {   // Node
                TowerManager.Instance.DespawnTower(tower_list[i]);
                m_DespawnWorldObjFlag = true;
            }
            else
            {   // Inven
                InventoryManager.Instance.RemoveTower(tower_list[i]);
            }
        }

        var newTowerData = m_towerLoader.DataList.Find((item)
            => { return item.Code == next_tower_code; });

        // spawn
        InventoryManager.Instance.AddNewTower(newTowerData);

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

        Debug.Log($"Combinatnion Complete Event : {m_DespawnWorldObjFlag}");
        OnCombinationCompleteEvent?.Invoke(m_DespawnWorldObjFlag);
        m_DespawnWorldObjFlag = false;
    }
}
