using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillUI : MonoBehaviour
{
	protected Button m_SkillButton;

	[SerializeField]
	protected Image m_SkillIcon_BG;
	[SerializeField]
	protected Image m_SkillIcon_FG;
	[SerializeField]
	protected TMPro.TextMeshProUGUI m_SkillCountText;
	[SerializeField]
	protected TMPro.TextMeshProUGUI m_SkillKeyText;

	public string skillCountText
	{
		get => m_SkillCountText.text;
		set
		{
			m_SkillCountText.text = value;

			if (value == "0")
				m_SkillCountText.gameObject.SetActive(false);
			else
				m_SkillCountText.gameObject.SetActive(true);
		}
	}
	public string skillKeyText { get => m_SkillKeyText.text; set => m_SkillKeyText.text = value; }
	public float skillFillAmount { get => m_SkillIcon_FG.fillAmount; set => m_SkillIcon_FG.fillAmount = value; }

	protected Button SkillButton
	{
		get
		{
			if (null == m_SkillButton)
				m_SkillButton = GetComponent<Button>();

			return m_SkillButton;
		}
	}

	public Button.ButtonClickedEvent onClick => SkillButton.onClick;

	public void SetOnOff(bool onoff)
	{
		m_SkillButton.gameObject.SetActive(onoff);
		//m_SkillButton.enabled = onoff;
	}
	public void SetSkillImage(Sprite sprite)
	{
		m_SkillIcon_BG.sprite = sprite;
		m_SkillIcon_FG.sprite = sprite;
	}

	void Start()
	{
		m_SkillButton = this.gameObject.GetComponent<Button>();
	}
}
