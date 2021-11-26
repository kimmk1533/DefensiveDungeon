using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
	public int m_PrefabCode;
	public E_EffectType m_Type;
	protected ParticleSystem m_Particle;
	protected float m_Timer;
	protected float m_Duration;

	#region 내부 컴포넌트
	protected EffectManager M_Effect => EffectManager.Instance;
	#endregion

	#region 외부 함수
	public void InitializeEffect()
	{
		if (null == m_Particle)
		{
			m_Particle = GetComponentInChildren<ParticleSystem>(true);
		}
		m_Duration = m_Particle.main.duration;
		m_Timer = 0f;
	}
	public void InitializeEffect(float duration)
	{
		if (null == m_Particle)
		{
			m_Particle = GetComponentInChildren<ParticleSystem>(true);
		}
		ParticleSystem.MainModule main = m_Particle.main;
		main.loop = true;
		m_Duration = duration;
		m_Timer = 0f;
	}
	public void FinalizeEffect()
	{
		m_Timer = 0f;
	}
	#endregion
	#region 유니티 콜백 함수
	void Update()
	{
		m_Timer += Time.deltaTime;

		if (m_Timer >= m_Particle.main.duration)
		{
			m_Particle.Stop(true);
			m_Particle.Clear(true);
			m_Particle.time = 0f;
			M_Effect.DespawnEffect(this);
		}
	}
	#endregion
}

public enum E_EffectType
{
	None = -1,

	Attack,
	Hit,
}