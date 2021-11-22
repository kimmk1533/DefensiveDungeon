using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField]
	protected Enemy_TableExcel m_EnemyInfo_Excel;
	[SerializeField]
	protected S_EnemyData m_EnemyInfo;

	//현재 바라보고 있는 waypoint
	[SerializeField]
	protected WayPoint m_WayPoint;

	#region 내부 컴포넌트
	// 애니메이터
	protected EnemyAnimator m_Animator;
	// 범위
	protected SphereCollider m_RangeCollider;
	// 체력바
	protected EnemyHPBar m_HPBar;
	#endregion
	#region 내부 프로퍼티
	#region 매니저
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected WayPointManager M_WayPoint => WayPointManager.Instance;
	protected EnemySkillManager M_EnemySkill => EnemySkillManager.Instance;
	protected EnemyHPBarManager M_EnemyHPBar => EnemyHPBarManager.Instance;

	protected FloatingTextManager M_DamageText => FloatingTextManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public string Get_EnemyName_EN => m_EnemyInfo_Excel.Name_EN;
	public float Get_EnemyHP => m_EnemyInfo.HP;
	public float Get_EnemyDef => m_EnemyInfo.Def;

	public E_Direction Direction { get => m_EnemyInfo.Direction; set => m_EnemyInfo.Direction = value; }
	public Transform HitPivot => m_EnemyInfo.HitPivot;
	public bool IsDead => m_EnemyInfo.IsDead;
	#endregion

	#region 내부 함수
	private void StartSkill()
	{
		m_Animator.SetTrigger("Skill");
	}
	//private void ChangeMode()
	//{
	//	//그리핀 하늘 코드로 데이터 셋팅
	//	InitializeEnemy(200009);
	//}
	//다음 waypoint 정보
	private void GetNextWayPoint()
	{
		transform.position = m_WayPoint.transform.position;

		if (m_WayPoint.isLast)
			return;

		m_WayPoint = m_WayPoint.next;
		transform.LookAt(m_WayPoint.transform);
	}
	#endregion
	#region 외부 함수
	public void InitializeEnemy(int code, E_Direction dir)
	{
		#region 엑셀 데이터
		m_EnemyInfo_Excel = M_Enemy.GetData(code);
		#endregion

		#region 내부 데이터
		m_EnemyInfo.Atk = m_EnemyInfo_Excel.Atk;
		m_EnemyInfo.HP = m_EnemyInfo_Excel.HP;
		m_EnemyInfo.Def = m_EnemyInfo_Excel.Def;
		m_EnemyInfo.Move_spd = m_EnemyInfo_Excel.Move_spd;

		// 방향 설정
		m_EnemyInfo.Direction = dir;
		// 스폰 포인트 설정
		m_WayPoint = M_WayPoint.GetFirstWayPoint(dir);
		// 위치 설정
		transform.position = m_WayPoint.transform.position;
		// 다음 웨이 포인트 설정
		m_WayPoint = m_WayPoint.next;
		// 방향 설정
		transform.LookAt(m_WayPoint.transform);
		// 사망 여부
		m_EnemyInfo.IsDead = false;

		// 공격 피벗
		if (null == m_EnemyInfo.AttackPivot)
		{
			m_EnemyInfo.AttackPivot = transform.GetChild("AttackPivot");
		}
		// 피격 피벗
		if (null == m_EnemyInfo.HitPivot)
		{
			m_EnemyInfo.HitPivot = transform.GetChild("HitPivot");
		}

		#region 기본 스킬
		// 기본 스킬 엑셀 데이터
		m_EnemyInfo.Condition_Default_Origin = M_EnemySkill.GetConditionData(m_EnemyInfo_Excel.Atk_Code);
		m_EnemyInfo.Stat_Default_Origin = M_EnemySkill.GetStatData(m_EnemyInfo.Condition_Default.PassiveCode);
		// 기본 스킬 데이터
		m_EnemyInfo.Condition_Default = m_EnemyInfo.Condition_Default_Origin;
		m_EnemyInfo.Stat_Default = m_EnemyInfo.Stat_Default_Origin;
		// 기본 스킬 공격 속도
		m_EnemyInfo.AttackSpeed_Default = m_EnemyInfo.Stat_Default_Origin.CoolTime;
		// 기본 스킬 타이머
		m_EnemyInfo.AttackTimer_Default = 0f;
		#endregion
		#endregion

		#region 내부 컴포넌트
		if (null == m_Animator)
		{
			m_Animator = transform.Find("Mesh").GetComponent<EnemyAnimator>();
			m_Animator.Initialize(this);
		}

		// 공격 사거리
		if (null == m_RangeCollider)
		{
			m_RangeCollider = transform.Find("EnemySkillRange").GetComponent<SphereCollider>();
			m_RangeCollider.gameObject.layer = LayerMask.NameToLayer("EnemySkillRange");
			m_RangeCollider.isTrigger = true;
		}

		// 체력바
		if (null == m_HPBar)
		{
			m_HPBar = M_EnemyHPBar.SpawnHPBar();
			m_HPBar.fillAmount = 1f;
			m_HPBar.m_EnemyTransform = transform;
			m_HPBar.transform.position = M_EnemyHPBar.m_HPBarCanvas.worldCamera.WorldToScreenPoint(transform.position) + M_EnemyHPBar.Distance;
		}
		#endregion
	}
	public void FinializeEnemy()
	{
		M_EnemyHPBar.DespawnHPBar(m_HPBar);
		m_HPBar = null;
	}

	// 대미지
	public void On_DaMage(float damage)
	{
		// 예외 처리
		if (m_EnemyInfo.IsDead)
			return;

		// 방어력 계산
		damage -= m_EnemyInfo.Def;

		// 최소 대미지 적용
		if (damage <= 0f)
			damage = 1f;

		// 대미지 적용
		m_EnemyInfo.HP -= damage;

		// 대미지 텍스트
		Vector3 text_position = transform.position + Vector3.forward * 2.5f;
		M_DamageText.SpawnDamageText(damage.ToString(), text_position);

		// 체력바 UI
		if (!m_HPBar.gameObject.activeSelf)
		{
			m_HPBar.gameObject.SetActive(true);
		}
		m_HPBar.fillAmount = m_EnemyInfo.HP / m_EnemyInfo_Excel.HP;

		// 사망 확인
		if (m_EnemyInfo.HP <= 0)
		{
			SetAnimation_Death();
			m_EnemyInfo.IsDead = true;
		}
	}

	// 사망
	public void SetAnimation_Death()
	{
		m_Animator.SetBool("Die", true);
		m_HPBar.gameObject.SetActive(false);
	}
	// 공격
	#endregion
	#region 유니티 콜백 함수
	private void Update()
	{
		if (m_EnemyInfo.IsDead)
			return;

		//마왕만 타겟으로 잡기
		//벽이나 중간에 장애물이 있다면 바꿔야함
		if (m_WayPoint.isLast)
		{
			float Distance = Vector3.Distance(transform.position, new Vector3(0f, 0f, 0f));

			//거리 안에 있다면
			if (Distance <= m_EnemyInfo.Stat_Default.Range)
			{
				// 회전할 방향
				Vector3 lookingDir = m_WayPoint.transform.position - transform.position;

				// y 회전 방지
				lookingDir.y = 0f;

				// 회전
				transform.rotation = Quaternion.LookRotation(lookingDir);

				if (m_EnemyInfo.AttackTimer_Default >= m_EnemyInfo.Stat_Default.CoolTime)
				{
					m_Animator.SetTrigger("Attack");
					m_EnemyInfo.AttackTimer_Default = 0f;
				}
				else
				{
					m_EnemyInfo.AttackTimer_Default += Time.deltaTime;
				}
			}
		}

		Vector3 dir = m_WayPoint.transform.position - transform.position;
		transform.Translate(dir.normalized * 2f * Time.deltaTime, Space.World);
		m_HPBar.transform.position = M_EnemyHPBar.m_HPBarCanvas.worldCamera.WorldToScreenPoint(transform.position) + M_EnemyHPBar.Distance;

		if (Vector3.Distance(transform.position, m_WayPoint.transform.position) <= 0.2f)
		{
			GetNextWayPoint();
		}

		#region 그리핀(하늘)로 체인지

		//if (m_EnemyInfo.Name_EN == "Grffin02")
		//{
		//	//HP가 반아래가 되었을때
		//	if (m_EnemyInfo.HP <= m_EnemyInfo_Excel.HP * 0.5f)
		//	{
		//		ChangeMode();
		//	}
		//}

		#endregion
	}
	#endregion

	#region Call함수
	public void CallAttack()
	{
		m_EnemyInfo.Atk *= m_EnemyInfo.Stat_Default.Dmg_Percent;
		M_EnemySkill.SpawnProjectileSkill(m_EnemyInfo.Condition_Default_Origin.projectile_prefab, m_EnemyInfo.Atk, m_EnemyInfo.Condition_Default, m_EnemyInfo.Stat_Default, m_EnemyInfo.AttackPivot);
	}
	public void CallSkill()
	{

	}
	public void CallDie()
	{
		M_Enemy.Despawn(this);
		m_Animator.SetBool("Die", false);
	}
	#endregion

	[Serializable]
	public struct S_EnemyData
	{
		public float Atk;
		public float HP;
		public float Def;
		public float Move_spd;

		// 적 방향
		public E_Direction Direction;
		// 공격 피벗
		public Transform AttackPivot;
		// 피격 피벗
		public Transform HitPivot;
		// 사망 여부
		public bool IsDead;

		#region 기본 스킬
		// 기본 스킬 엑셀 데이터
		public SkillCondition_TableExcel Condition_Default_Origin;
		public SkillStat_TableExcel Stat_Default_Origin;
		// 기본 스킬 데이터
		public SkillCondition_TableExcel Condition_Default;
		public SkillStat_TableExcel Stat_Default;
		// 기본 스킬 공격 속도
		public float AttackSpeed_Default;
		// 기본 스킬 타이머
		public float AttackTimer_Default;
		#endregion
	}
}