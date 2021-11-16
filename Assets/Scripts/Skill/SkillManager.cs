using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
	protected SkillCondition_TableExcelLoader m_SkillConditionData;
	protected SkillStat_TableExcelLoader m_SkillStatData;
	protected Prefab_TableExcelLoader m_PrefabData;

	#region 내부 프로퍼티
	// 데이터 테이블
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	// 스킬 메모리풀
	protected SkillPool M_SkillPool => SkillPool.Instance;
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

	public Skill SpawnProjectileSkill(int prefabCode)
	{
		string key = m_PrefabData.GetPrefab(prefabCode)?.name;

		Skill skill = M_SkillPool.GetPool(key)?.Spawn();
		return skill;
	}
	public void DespawnProjectileSkill(Skill skill)
	{
		int projectPrefabCode = GetConditionData(skill.m_ConditionInfo_Excel.Code).projectile_prefab;
		string key = m_PrefabData.GetPrefab(projectPrefabCode).name;
		M_SkillPool.GetPool(key)?.DeSpawn(skill);
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

// 스킬 이동 타입
public enum E_MoveType
{
	None,

	Straight, // 직선 이동
	Curve // 곡선 이동
}

// 스킬 공격 타입
public enum E_AttackType
{
	None,

	NormalFire, // 일반 공격
	FixedFire, // 지점 공격
	PenetrateFire, // 관통 공격
	BounceFire // 튕기는 공격
}
// 스킬 발사 타입
public enum E_FireType
{
	None,

	Select_point,
	Select_self,
	Select_enemy
}
// 타겟 선정 타입
public enum E_TargetType
{
	None,

	CloseTarget,
	RandTarget,
	FixTarget,
	TileTarget
}