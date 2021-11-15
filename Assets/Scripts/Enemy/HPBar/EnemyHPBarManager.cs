using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarManager : Singleton<EnemyHPBarManager>
{
	public Canvas m_HPBarCanvas;
	[SerializeField]
	protected Vector3 m_Distance = new Vector3(0f, 50f, 0f);
	protected const string key = "EnemyHPBar";

	protected List<EnemyHPBar> m_EnemyHPBarList;

	#region 내부 컴포넌트
	protected EnemyHPBarPool M_HPBarPool => EnemyHPBarPool.Instance;
	#endregion
	#region 외부 프로퍼티
	public Vector3 Distance => m_Distance;
	#endregion

	#region 외부 함수
	public EnemyHPBar SpawnHPBar()
	{
		// 스폰
		EnemyHPBar hpBar = M_HPBarPool.GetPool(key).Spawn();
		// 초기화
		hpBar.Initialize();
		// 부모 설정
		hpBar.transform.SetParent(m_HPBarCanvas.transform);
		// 관리 리스트에 추가
		m_EnemyHPBarList.Add(hpBar);
		return hpBar;
	}
	public void DespawnHPBar(EnemyHPBar hpBar)
	{
		// 부모 설정
		hpBar.transform.SetParent(M_HPBarPool.transform);
		// 관리 리스트에서 제거
		m_EnemyHPBarList.Remove(hpBar);
		// 디스폰
		M_HPBarPool.GetPool(key).DeSpawn(hpBar);
	}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		if (null == m_EnemyHPBarList)
		{
			m_EnemyHPBarList = new List<EnemyHPBar>();
		}
	}
	#endregion
}
