using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySkillManager : Singleton<EnemySkillManager>
{
	protected SkillCondition_TableExcelLoader m_SkillConditionData;
	protected SkillStat_TableExcelLoader m_SkillStatData;
	protected Prefab_TableExcelLoader m_PrefabData;

	#region 내부 프로퍼티
	// 데이터 테이블
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	// 스킬 메모리풀
	protected EnemySkillPool M_SkillPool => EnemySkillPool.Instance;
	#endregion

	#region 외부 함수
	public SkillCondition_TableExcel GetConditionData(int code)
	{
		SkillCondition_TableExcel skillConditionData = m_SkillConditionData.DataList.Where(item => item.Code == code).SingleOrDefault();

		return skillConditionData;
	}
	public SkillStat_TableExcel GetStatData(int code)
	{
		SkillStat_TableExcel skillStatData = m_SkillStatData.DataList.Where(item => item.Code == code).SingleOrDefault();

		return skillStatData;
	}

	public void SpawnProjectileSkill(int prefabCode, float m_damage, SkillCondition_TableExcel condition, SkillStat_TableExcel stat, Transform attackpivot)
	{
		string key = m_PrefabData.GetPrefab(prefabCode)?.name;

		EnemySkill skill = M_SkillPool.GetPool(key)?.Spawn();

		if (null != skill)
		{
			skill.InitializeSkill(m_damage, condition, stat, attackpivot);

			skill.gameObject.SetActive(true);
		}
	}
	public void DespawnProjectileSkill(EnemySkill skill)
	{
		int projectPrefabCode = GetConditionData(skill.m_ConditionInfo.Code).projectile_prefab;
		string key = m_PrefabData.GetPrefab(projectPrefabCode).name;
		M_SkillPool.GetPool(key)?.DeSpawn(skill);

		skill.gameObject.SetActive(false);
	}
	#endregion
	#region 유니티 콜백 함수
	void Awake()
	{
		m_SkillConditionData = M_DataTable.GetDataTable<SkillCondition_TableExcelLoader>();
		m_SkillStatData = M_DataTable.GetDataTable<SkillStat_TableExcelLoader>();
		m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
	}
	#endregion
}