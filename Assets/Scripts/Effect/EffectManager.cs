using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
	protected Prefab_TableExcelLoader m_PrefabData;

	#region 내부 프로퍼티
	// 데이터 테이블
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected EffectPool M_EffectPool => EffectPool.Instance;
	protected SkillManager M_Skill => SkillManager.Instance;
	#endregion

	#region 외부 함수
	public Effect SpawnEffect(int prefabCode)
	{
		string key = m_PrefabData.GetPrefab(prefabCode)?.name;

		Effect effect = M_EffectPool.GetPool(key)?.Spawn();
		effect?.InitializeEffect();
		return effect;
	}
	public void DespawnEffect(Effect effect)
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
