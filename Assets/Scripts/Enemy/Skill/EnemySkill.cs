using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    // 스킬 정보 (엑셀)
    public SkillCondition_TableExcel m_ConditionInfo;
    public SkillStat_TableExcel m_StatInfo;

    #region 내부 프로퍼티

    // 스킬 매니져
    protected EnemySkillManager enemyskillmanager => EnemySkillManager.Instance;

    // 버프 매니져
    protected BuffManager M_Buff => BuffManager.Instance;

    // 타겟 위치
    protected Vector3 TargetPos => new Vector3(0f, 0f, 0f);

    // 스킬 이동 속도
    protected float MoveSpeed => m_StatInfo.Speed * Time.deltaTime;

    // 타겟까지의 방향
    protected Vector3 TargetDir => TargetPos - transform.position;

    // 타겟까지의 거리
    public float DistanceTarget => Vector3.Distance(transform.position, TargetPos);

    // 타겟에게 도착 여부
    protected bool ArrivedToTarget;

    private float m_damage;

    #endregion

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
    }

    private void Update()
    {
        if (CheckToDespawn())
        {
            Despawn();
            return;
        }

        RotateSkill();
        MoveSkill();

        if (DistanceTarget <= 0.2f)
        {
            EnemyEffect effect = EnemyEffectManager.Instance.SpawnEffect(m_ConditionInfo.damage_prefab);

            effect.gameObject.transform.position = DevilManager.Instance.Devil.HitPivot.position;
            effect.gameObject.SetActive(true);

            DevilManager.Instance.Devil.GetDamage(m_damage);
            ArrivedToTarget = true;
        }
    }

    #region 내부 함수

    protected bool CheckToDespawn()
    {
        switch ((E_AttackType)m_ConditionInfo.Atk_type)
        {
            case E_AttackType.NormalFire:
                return ArrivedToTarget;
        }

        return  ArrivedToTarget;
    }

    protected void Despawn()
    {
        ArrivedToTarget = false;
        enemyskillmanager.DespawnProjectileSkill(this);
    }

    protected void RotateSkill()
    {
        switch ((E_MoveType)m_ConditionInfo.Move_type)
        {
            case E_MoveType.Straight:
                transform.LookAt(transform.position + TargetDir);
                break;
        }
    }

    protected void MoveSkill()
    {
        switch ((E_MoveType)m_ConditionInfo.Move_type)
        {
            case E_MoveType.Straight:
                StraightMove();
                break;
        }
    }

    protected void StraightMove()
    {
        transform.position += TargetDir.normalized * MoveSpeed;
    }
    
    #endregion

    #region 외부 함수

    //Enemy 스크립트에 있는 데이터 가져오기
    public void InitializeSkill(float damage, SkillCondition_TableExcel conditionData, SkillStat_TableExcel statData, Transform attackpivot)
    {
        gameObject.transform.position = attackpivot.position;
        m_ConditionInfo = conditionData;
        m_StatInfo = statData;

        m_damage = damage;
    }

    #endregion
}
