using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyPool, Enemy>
{
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected Enemy_TableExcelLoader M_EnemyData => M_DataTable.GetDataTable<Enemy_TableExcelLoader>();
    protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

    public override void __Initialize()
    {
        base.__Initialize();

        for (int i = 0; i < M_EnemyData.DataList.Count; ++i)
        {
            int PrefabCode = M_EnemyData.DataList[i].Prefab;
            Debug.Log("Enemy_" + PrefabCode);

            GameObject originObj = M_PrefabData.GetPrefab(PrefabCode);

            if (originObj != null)
            {
                GameObject originClone = GameObject.Instantiate(originObj);
                string key = originClone.name = originObj.name;

                Enemy origin = originClone.AddComponent<Enemy>();
                origin.InitializeEnemy(M_EnemyData.DataList[i].Code);

                origin.gameObject.SetActive(false);

                if (!AddPool(key, origin, transform))
                {
                    GameObject.Destroy(originClone);
                }
            }
        }
    }
}
