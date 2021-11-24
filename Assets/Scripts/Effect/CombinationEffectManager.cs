using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationEffectManager : Singleton<CombinationEffectManager>
{
	protected Prefab_TableExcelLoader m_PrefabData;

	public const int PrefabCode1 = 900235;
	public const int PrefabCode2 = 900217;

	#region 내부 프로퍼티
	// 데이터 테이블
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected CombinationEffectPool M_EffectPool => CombinationEffectPool.Instance;
	#endregion

	#region 외부 함수
	public CombinationEffect SpawnEffect(int prefabCode, Vector3 _startpos)
	{
		string key = m_PrefabData.GetPrefab(prefabCode)?.name;
		CombinationEffect effect = M_EffectPool.GetPool(key)?.Spawn();
		effect.transform.position = _startpos;
		effect?.InitializeEffect();
		return effect;
	}
	public void DespawnEffect(CombinationEffect effect)
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
