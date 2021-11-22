using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : Singleton<WayPointManager>
{
	protected Dictionary<E_Direction, List<WayPoint>> m_WayPointList;
	[SerializeField]
	protected List<WayPoint> m_North;
	[SerializeField]
	protected List<WayPoint> m_East;
	[SerializeField]
	protected List<WayPoint> m_South;
	[SerializeField]
	protected List<WayPoint> m_West;

	#region 외부 함수
	public void Initialize()
	{
		if (null == m_WayPointList)
			m_WayPointList = new Dictionary<E_Direction, List<WayPoint>>();
		else if (m_WayPointList.Count > 0)
			m_WayPointList.Clear();

		m_WayPointList.Add(E_Direction.North, m_North);
		m_WayPointList.Add(E_Direction.East, m_East);
		m_WayPointList.Add(E_Direction.South, m_South);
		m_WayPointList.Add(E_Direction.West, m_West);

		for (E_Direction dir = E_Direction.None + 1; dir < E_Direction.Max; ++dir)
		{
			List<WayPoint> wayPoints = m_WayPointList[dir];

			for (int i = 0; i < wayPoints.Count; ++i)
			{
				WayPoint previous = (i - 1 < 0) ? null : wayPoints[i - 1];
				WayPoint next = (i + 1 >= wayPoints.Count) ? null : wayPoints[i + 1];

				wayPoints[i].Initialize(previous, next);
			}
		}
	}

	public WayPoint GetFirstWayPoint(E_Direction dir)
	{
		if (m_WayPointList[dir].Count <= 0)
			return null;

		return m_WayPointList[dir][0];
	}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		Initialize();
	}
	#endregion
}
