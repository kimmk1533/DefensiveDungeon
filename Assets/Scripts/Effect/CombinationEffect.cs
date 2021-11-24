using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationEffect : MonoBehaviour
{
	public int m_PrefabCode;
	public E_ComEffect m_Type;
	protected ParticleSystem m_Particle;
	protected float m_Timer;
	protected CombinationEffectManager M_ComEffect => CombinationEffectManager.Instance;
	#region 내부 컴포넌트

	#endregion
	#region 외부 함수
	public void InitializeEffect()
	{
		if (null == m_Particle)
		{
			m_Particle = GetComponentInChildren<ParticleSystem>(true);
		}
		m_Timer = 0f;
	}
	public void FinalizeEffect()
	{
		m_Timer = 0f;
	}

	public void Play(bool withChildren)
	{
		m_Particle.Play(withChildren);
	}
	#endregion

	#region 유니티 콜백 함수
	void Update()
	{
		if (m_Type == E_ComEffect.Arrival)
		{
			m_Timer += Time.deltaTime;

			if (m_Timer >= m_Particle.main.duration)
			{
				m_Particle.Stop(true);
				m_Particle.Clear(true);
				m_Particle.time = 0f;
				M_ComEffect.DespawnEffect(this);
				this.gameObject.SetActive(false);
			}
		}
	}
	#endregion
}
public enum E_ComEffect
{
	None = -1,
	Move,
	Arrival
}
