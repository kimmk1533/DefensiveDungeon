using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
	protected List<FloatingText> m_FloatingTextList;
	protected Canvas m_Canvas;

	#region 내부 프로퍼티
	protected FloatingTextPool M_FloatingTextPool => FloatingTextPool.Instance;
	#endregion

	#region 내부 함수
	IEnumerator Co_SpawnDamageText(string text, float time)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(time);
		}

		SpawnDamageText(text, Vector2.one * 0.5f);
	}
	IEnumerator Co_SpawnDamageText(string text, float time, Vector2 position)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(time);
		}

		SpawnDamageText(text, position);
	}
	#endregion
	#region 외부 함수
	public void SpawnDamageText(string text)
	{
		SpawnDamageText(text, Vector2.one * 0.5f);
	}
	public void SpawnDamageText(string text, Vector3 position)
	{
		// 스폰
		FloatingText floatingText = M_FloatingTextPool.GetPool("DamageText").Spawn();
		// 초기화
		floatingText.__Initialize();
		// 텍스트 설정
		floatingText.text = text;
		// 부모 설정
		floatingText.transform.SetParent(m_Canvas.transform);
		// 위치 설정
		floatingText.transform.position = Camera.main.WorldToScreenPoint(position);
		// 활성화 설정
		floatingText.gameObject.SetActive(true);
		// 관리 리스트에 추가
		m_FloatingTextList.Add(floatingText);
	}
	public void SpawnDamageText(string text, float time)
	{
		StartCoroutine(Co_SpawnDamageText(text, time));
	}
	public void SpawnDamageText(string text, float time, Vector3 position)
	{
		StartCoroutine(Co_SpawnDamageText(text, time, position));
	}
	public void DespawnDamageText(FloatingText floatingText)
	{
		// 부모 설정
		floatingText.transform.SetParent(M_FloatingTextPool.transform);
		// 관리 리스트에서 제거
		m_FloatingTextList.Remove(floatingText);
		// 디스폰
		M_FloatingTextPool.GetPool("DamageText").DeSpawn(floatingText);
	}
	#endregion
	#region 유니티 콜백 함수
	private void Awake()
	{
		if (null == m_FloatingTextList)
		{
			m_FloatingTextList = new List<FloatingText>();
		}
		if (null == m_Canvas)
		{
			m_Canvas = transform.Find("FloatingTextCanvas").GetComponent<Canvas>();
		}
	}
	#endregion
}
