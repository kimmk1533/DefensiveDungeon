using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
	protected WayPoint m_Previous;
	protected WayPoint m_Next;

	#region 내부 프로퍼티
	protected WayPointManager M_WayPoint => WayPointManager.Instance;
	#endregion
	#region 외부 프로퍼티
	public WayPoint previous => m_Previous;
	public WayPoint next => m_Next;

	public bool isFirst => m_Previous == null;
	public bool isLast => m_Next == null;
	#endregion

	#region 외부 함수
	public void Initialize(WayPoint previous, WayPoint next)
	{
		m_Previous = previous;
		m_Next = next;
	}
	#endregion
}
