using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : ObjectPool<EffectPool, Effect>
{
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected SkillCondition_TableExcelLoader M_SkillConditionData => M_DataTable.GetDataTable<SkillCondition_TableExcelLoader>();
    protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

    public override void __Initialize()
    {
        base.__Initialize();

        for (int i = 0; i < M_SkillConditionData.DataList.Count; ++i)
        {
            int PrefabCode = M_SkillConditionData.DataList[i].Atk_prefab;

            if (PrefabCode == 0)
                continue;

            GameObject originObj = M_PrefabData.GetPrefab(PrefabCode);
            if (originObj != null)
            {
                GameObject originClone = GameObject.Instantiate(originObj);
                string key = originClone.name = originObj.name;

                Effect origin = originClone.AddComponent<Effect>();
                origin.m_PrefabCode = PrefabCode;
                origin.m_Type = E_EffectType.Attack;

                origin.gameObject.SetActive(false);
                if (!AddPool(key, origin, transform))
                {
                    GameObject.Destroy(originClone);
                }
            }

            PrefabCode = M_SkillConditionData.DataList[i].damage_prefab;

            if (PrefabCode == 0)
                continue;

            originObj = M_PrefabData.GetPrefab(PrefabCode);
            if (originObj != null)
            {
                GameObject originClone = GameObject.Instantiate(originObj);
                string key = originClone.name = originObj.name;

                Effect origin = originClone.AddComponent<Effect>();
                origin.m_PrefabCode = PrefabCode;
                origin.m_Type = E_EffectType.Hit;

                origin.gameObject.SetActive(false);
                if (!AddPool(key, origin, transform))
                {
                    GameObject.Destroy(originClone);
                }
            }
        }
    }
}
