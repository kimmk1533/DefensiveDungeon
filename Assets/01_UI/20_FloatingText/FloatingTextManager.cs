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
	protected void Spawn(string text, FloatingTextFilter filter)
	{
		// 스폰
		FloatingText floatingText = M_FloatingTextPool.GetPool("DamageText").Spawn();
		// 초기화
		floatingText.__Initialize();
		// 텍스트 설정
		floatingText.textMeshPro.text = text;
		// 색 설정
		floatingText.textMeshPro.color = filter.color;
		// 아웃라인 색 설정
		floatingText.textMeshPro.outlineColor = filter.outlineColor;
		// 아웃라인 두께 설정
		floatingText.textMeshPro.outlineWidth = filter.outlineWidth;
		// 부모 설정
		floatingText.transform.SetParent(m_Canvas.transform);
		// 위치 설정
		switch (filter.postionType)
		{
			case FloatingTextFilter.E_PostionType.World:
				floatingText.transform.position = filter.position;
				break;
			case FloatingTextFilter.E_PostionType.WorldToScreen:
				floatingText.transform.position = Camera.main.WorldToScreenPoint(filter.position);
				break;
			case FloatingTextFilter.E_PostionType.ViewToScreen:
				floatingText.transform.position = Camera.main.ViewportToScreenPoint(filter.position);
				break;
		}
		// 크기 설정
		floatingText.GetComponent<RectTransform>().sizeDelta = filter.sizeDelta;
		floatingText.transform.localScale = filter.scale;
		// 활성화 설정
		floatingText.gameObject.SetActive(true);
		// 관리 리스트에 추가
		m_FloatingTextList.Add(floatingText);
	}
	IEnumerator Co_SpawnDamageText(string text, FloatingTextFilter filter)
	{
		yield return new WaitForSeconds(filter.time);

		Spawn(text, filter);
	}
	#endregion
	#region 외부 함수
	public void SpawnDamageText(string text, FloatingTextFilter filter)
	{
		if (filter.time > 0f)
		{
			StartCoroutine(Co_SpawnDamageText(text, filter));
			return;
		}

		Spawn(text, filter);
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
