using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// mouse input 을 위한 gui 와
// 실제 몬스터를 위치시킬 slot 인 object 를 사용
public class InventoryManager : Singleton<InventoryManager>
{
	// gui
	[Space(10)]
	[SerializeField] RawImage m_root_panel;    // 이 Panel 의 좌측을 기준으로
	GridLayoutGroup m_root_gridLayoutGroup;       // 이하 slot count 에 맟춰 size 를 조정한다
	[SerializeField] InventorySlotGUI m_originGUI;

	// 관리 list
	[Space(10)]
	[SerializeField] List<InventorySlotGUI> m_slotGUI_list;      // 클릭
	protected CombinationManager M_COM => CombinationManager.Instance;
	protected CombinationEffectManager M_ComEffect => CombinationEffectManager.Instance;
	private void Awake()
	{
		m_root_gridLayoutGroup = m_root_panel.GetComponent<GridLayoutGroup>();
	}

	private void Start()
	{
		__Initialize();
	}

	public void __Initialize()
	{
		int _cellcount = m_root_gridLayoutGroup.constraintCount;

		// origin 끄기
		m_originGUI.gameObject.SetActive(false);

		for (int i = 0; i < _cellcount; i++)
		{
			// gui instantiate
			InventorySlotGUI newSlotGUI = GameObject.Instantiate<InventorySlotGUI>(m_originGUI);
			newSlotGUI.__Indexing(i);
			m_slotGUI_list.Add(newSlotGUI);
			newSlotGUI.gameObject.SetActive(true);
			newSlotGUI.transform.SetParent(m_root_panel.transform);
		}
	}



	// 가장 비어있는 좌측 슬롯
	InventorySlotGUI GetAvailableSlot()
	{
		foreach (var item in m_slotGUI_list)
		{
			if (false == item.IsOccupied)
				return item;
		}
		return null;
	}

	public bool IsAllOccupied()
	{
		InventorySlotGUI slot = GetAvailableSlot();
		if (null == slot)
			return true;
		return false;
	}

	public void AddNewTower(Tower_TableExcel data)
	{
		InventorySlotGUI slot = GetAvailableSlot();
		if (null == slot)
			return;

		Tower newTower = TowerManager.Instance.SpawnTower_Inventory(data.Code);


		newTower.gameObject.SetActive(false);
		slot.SetTower(newTower, data);
		M_COM.EffectEndPos = slot.GetShowObjPosition();
		CombinationManager.Instance.Combinatnion();
		foreach (var item in m_slotGUI_list)
		{
			item.ForceUIUpdate();
		}
		
	}
	public void RemoveTower(Tower tower)
	{
		foreach (var item in m_slotGUI_list)
		{
			if (item.TowerObj == tower)
			{
				item.ClearInven();
				TowerManager.Instance.DespawnTower(tower);
			}
		}
	}
}
