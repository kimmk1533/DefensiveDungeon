using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : Singleton<PauseManager>
{
	[SerializeField] RectTransform m_MainPanel;
	[SerializeField] RectTransform m_PausePanel;
	[SerializeField] RectTransform m_OptionPanel;
	[SerializeField, ReadOnly] float m_LastTimeScale = 1f;

	public void TimeScale(float scale)
	{
		m_LastTimeScale = Time.timeScale = scale;
	}

	public void Pause()
	{
		m_LastTimeScale = Time.timeScale;
		Time.timeScale = 0f;

		m_MainPanel.gameObject.SetActive(false);
		m_PausePanel.gameObject.SetActive(true);
	}

	public void Resume()
	{
		Time.timeScale = m_LastTimeScale;

		m_MainPanel.gameObject.SetActive(true);
		m_PausePanel.gameObject.SetActive(false);
	}
	public void Option()
	{
		m_OptionPanel.gameObject.SetActive(true);
	}
	public void MainMenu()
	{

	}
	public void Exit()
	{
		Application.Quit();
	}
}
