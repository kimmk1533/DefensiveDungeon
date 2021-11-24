using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationEffectPool : ObjectPool<CombinationEffectPool, CombinationEffect>
{
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

	void ChangeLayer(GameObject obj, string layer)
	{
		obj.layer = LayerMask.NameToLayer(layer);
		for (int i = 0; i < obj.transform.childCount; ++i)
		{
			ChangeLayer(obj.transform.GetChild(0).gameObject, layer);
		}
	}

	public override void __Initialize()
	{
		base.__Initialize();


		GameObject originObj = M_PrefabData.GetPrefab(CombinationEffectManager.PrefabCode1);
		if (originObj != null)
		{
			GameObject originClone = GameObject.Instantiate(originObj);
			string key = originClone.name = originObj.name;

			CombinationEffect origin = originClone.AddComponent<CombinationEffect>();
			origin.m_PrefabCode = CombinationEffectManager.PrefabCode1;
			origin.m_Type = E_ComEffect.Move;

			origin.gameObject.SetActive(false);
			ChangeLayer(origin.gameObject, "CombinationEffect");

			if (!AddPool(key, origin, transform))
			{
				GameObject.Destroy(originClone);
			}
		}

		originObj = M_PrefabData.GetPrefab(CombinationEffectManager.PrefabCode2);
		if (originObj != null)
		{
			GameObject originClone = GameObject.Instantiate(originObj);
			string key = originClone.name = originObj.name;

			CombinationEffect origin = originClone.AddComponent<CombinationEffect>();
			origin.m_PrefabCode = CombinationEffectManager.PrefabCode2;
			origin.m_Type = E_ComEffect.Arrival;

			origin.gameObject.SetActive(false);
			ChangeLayer(origin.gameObject, "CombinationEffect");
			if (!AddPool(key, origin, transform))
			{
				GameObject.Destroy(originClone);
			}
		}

	}

}


