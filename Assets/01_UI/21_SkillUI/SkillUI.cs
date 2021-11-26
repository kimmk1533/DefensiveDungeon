using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillUI : MonoBehaviour
{
	protected Image m_SkillImage;
	protected Button m_SkillButton;


	public Button.ButtonClickedEvent onClick => m_SkillButton.onClick;

	public void SetOnOff(bool onoff)
	{
		m_SkillButton.gameObject.SetActive(onoff);
		//m_SkillButton.enabled = onoff;
	}

	void Start()
	{
		m_SkillButton = this.gameObject.GetComponent<Button>();
		m_SkillImage = m_SkillButton?.image;
	}
}
