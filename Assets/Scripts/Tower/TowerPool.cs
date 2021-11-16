using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerPool : ObjectPool<TowerPool, Tower>
{
	protected Dictionary<int, float> m_TowerCode_Size_Dic;

	#region 내부 프로퍼티
	#region 매니저
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	#endregion

	protected Tower_TableExcelLoader M_TowerData => M_DataTable.GetDataTable<Tower_TableExcelLoader>();
	protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
	#endregion

	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		m_TowerCode_Size_Dic = new Dictionary<int, float>();

		for (int i = 3; i < M_TowerData.DataList.Count; ++i)
		{
			int PrefabCode = M_TowerData.DataList[i].Prefab;

			GameObject originObj = M_PrefabData.GetPrefab(PrefabCode);

			if (null != originObj)
			{
				GameObject originClone = GameObject.Instantiate(originObj);
				originClone.name = originObj.name;

				Tower origin = originClone.AddComponent<Tower>();

				int code = M_TowerData.DataList[i].Code;
				float size = M_PrefabData.DataList[i].Size;
				m_TowerCode_Size_Dic.Add(code, size);

				origin.gameObject.layer = LayerMask.NameToLayer("Tower");
				origin.gameObject.SetActive(false);

				string key = M_TowerData.DataList[i].Name_EN;
				if (!AddPool(key, origin, transform))
				{
					GameObject.Destroy(originClone);
				}
			}
		}

		//for (E_Tower i = E_Tower.OrkGunner01; i < E_Tower.Max; ++i)
		//{
		//    S_TowerData_Excel data = M_Tower.GetData(i);
		//    Tower tower = M_Resources.GetGameObject<Tower>("Tower", i.ToString());
		//    tower.m_TempCode = data.Code;
		//    AddPool(data.Prefeb.ToString(), tower, transform);
		//}
	}
	public float GetTowerSize(int code)
	{
		return m_TowerCode_Size_Dic[code];
	}
	#endregion
}
