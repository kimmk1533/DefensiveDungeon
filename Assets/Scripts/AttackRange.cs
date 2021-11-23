using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRange : MonoBehaviour
{
	[SerializeField]
	protected List<Enemy> m_TargetList;

	[SerializeField]
	protected E_Direction m_Direction;
	[SerializeField]
	protected bool m_CanFindTarget;

	#region 내부 컴포넌트
	protected SphereCollider m_RangeCollider;
	#endregion
	#region 내부 프로퍼티
	protected SphereCollider RangeCollider
	{
		get
		{
			if (m_RangeCollider == null)
			{
				m_RangeCollider = GetComponent<SphereCollider>();
			}

			return m_RangeCollider;
		}
	}
	#endregion
	#region 외부 프로퍼티
	public List<Enemy> TargetList => m_TargetList;
	public float Range { get => RangeCollider.radius; set => RangeCollider.radius = value; }
	public E_Direction Direction { get => m_Direction; set => m_Direction = value; }
	public bool CanFindTarget { get => m_CanFindTarget; set => m_CanFindTarget = value; }
	#endregion

	#region 외부 함수
	public void Initialize()
	{
		m_TargetList = new List<Enemy>();

		m_RangeCollider = GetComponent<SphereCollider>();
		//m_RangeCollider ??= gameObject.AddComponent<SphereCollider>();
		if (null == m_RangeCollider)
			m_RangeCollider = gameObject.AddComponent<SphereCollider>();
		m_RangeCollider.isTrigger = true;
	}
	public void Clear()
	{
		m_TargetList?.Clear();
	}
	public bool RemoveTarget(Enemy target)
	{
		return m_TargetList.Remove(target);
	}
	public Enemy GetFirstTarget()
	{
		return m_TargetList[0];
	}
	public Enemy GetNearTarget(bool exceptFirst = false)
	{
		Enemy target;

		var tempList = m_TargetList
			.OrderBy(obj =>
			{
				return (transform.position - obj.transform.position).sqrMagnitude;
			})
			.ToList();

		if (exceptFirst)
		{
			if (tempList.Count > 1)
			{
				target = tempList[1];
			}
			else
			{
				target = null;
			}
		}
		else
		{
			target = tempList.FirstOrDefault();
		}

		return target;
	}
	public Enemy GetRandomTarget()
	{
		int max = m_TargetList.Count;
		if (max <= 0)
		{
			return null;
		}

		int index = Random.Range(0, max);

		return m_TargetList[index];
	}
	#endregion
	#region 유니티 콜백 함수
	private void OnTriggerEnter(Collider other)
	{
		Enemy enemy = other.GetComponent<Enemy>();

		if (null == enemy)
			return;
		if (enemy.IsDead)
			return;

		if (m_CanFindTarget && (m_Direction == E_Direction.None || enemy.Direction == m_Direction))
			m_TargetList.Add(enemy);
	}
	private void OnTriggerExit(Collider other)
	{
		Enemy enemy = other.GetComponent<Enemy>();

		if (null == enemy)
			return;

		m_TargetList.Remove(enemy);
	}
	#endregion
}
