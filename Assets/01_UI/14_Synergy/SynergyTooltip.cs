using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynergyTooltip : MonoBehaviour
{
	[SerializeField] Icon_TableExcelLoader m_icon_loader;

	[SerializeField] TMPro.TextMeshProUGUI m_name_textpro;
	[SerializeField] TMPro.TextMeshProUGUI m_explain_textpro;
	[SerializeField] TMPro.TextMeshProUGUI m_ability_textpro;
	[SerializeField] Image m_icon_image;

	public bool IsActivated { get; private set; }


	private void Awake()
	{
		DeActivate();
	}

	public void Activate()
	{
		if (IsActivated)
			return;

		IsActivated = true;
		this.gameObject.SetActive(true);
	}
	public void DeActivate()
	{
		IsActivated = false;
		this.gameObject.SetActive(false);
	}


	public void SetPoisition(Vector2 mousePos)
	{
		// 앵커 좌측 상단 필수
		this.transform.position = mousePos;
	}

	public void SetInfo(string name, string explain_text, string ability_text, int sprite_code)
	{
		m_name_textpro.text = name;
		m_explain_textpro.text = explain_text;
		m_ability_textpro.text = ability_text;

		Sprite _sprite = m_icon_loader.GetIcon(sprite_code);
		m_icon_image.sprite = _sprite;
	}
}
