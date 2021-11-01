using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectManager : Singleton<EnemyEffectManager>
{
    protected Prefab_TableExcelLoader m_PrefabData;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    // 데이터 테이블
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected EnemyEffectPool M_EffectPool => EnemyEffectPool.Instance;
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public EnemyEffect SpawnEffect(int prefabCode)
    {
        string key = m_PrefabData.GetPrefab(prefabCode)?.name;

        EnemyEffect effect = M_EffectPool.GetPool(key)?.Spawn();
        effect?.InitializeEffect();
        return effect;
    }
    public void DespawnEffect(EnemyEffect effect)
    {
        effect.FinalizeEffect();
        string key = m_PrefabData.GetPrefab(effect.m_PrefabCode).name;
        M_EffectPool.GetPool(key)?.DeSpawn(effect);
    }
    #endregion

    #region 유니티 콜백 함수
    private void Awake()
    {
        m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
    }
    #endregion
}
