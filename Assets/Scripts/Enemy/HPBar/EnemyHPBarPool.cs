using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarPool : ObjectPool<EnemyHPBarPool, EnemyHPBar>
{
	public EnemyHPBar m_Origin;

	#region 내부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		if (null == m_Origin)
		{
			m_Origin = transform.Find("EnemyHPBar").GetComponent<EnemyHPBar>();
		}

		m_Origin.Initialize();

		AddPool("EnemyHPBar", m_Origin, transform);

		m_Origin.gameObject.SetActive(false);
	}
	#endregion
}
