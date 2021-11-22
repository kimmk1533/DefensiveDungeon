using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyPool, Enemy>
{
	#region 내부 프로퍼티
	#region 매니저
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	#endregion

	protected Enemy_TableExcelLoader M_EnemyData => M_DataTable.GetDataTable<Enemy_TableExcelLoader>();
	protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
	#endregion

	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		for (int i = 0; i < M_EnemyData.DataList.Count; ++i)
		{
			int PrefabCode = M_EnemyData.DataList[i].Prefab;

			GameObject originObj = M_PrefabData.GetPrefab(PrefabCode);

			if (originObj != null)
			{
				GameObject originClone = GameObject.Instantiate(originObj);
				originClone.name = originObj.name;

				Enemy origin = originClone.AddComponent<Enemy>();

				float size = M_PrefabData.DataList[i].Size;

				origin.transform.Find("Mesh").localScale = Vector3.one * size;
				origin.gameObject.layer = LayerMask.NameToLayer("Enemy");
				origin.gameObject.SetActive(false);

				string key = M_EnemyData.DataList[i].Name_EN;
				if (!AddPool(key, origin, transform))
				{
					GameObject.Destroy(originClone);
				}
			}
		}
	}
	#endregion
}
