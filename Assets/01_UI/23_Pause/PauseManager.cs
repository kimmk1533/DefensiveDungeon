using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : Singleton<PauseManager>
{
	[SerializeField] RectTransform m_MainPanel;
	[SerializeField] RectTransform m_PausePanel;
	[SerializeField] RectTransform m_OptionPanel;

	[SerializeField] GameObject m_Managers;

	[SerializeField] Image m_SpeedImage;
	[SerializeField] Sprite m_Speed_1;
	[SerializeField] Sprite m_Speed_2;
	[SerializeField] Sprite m_Speed_3;

	[SerializeField, ReadOnly] float m_LastTimeScale = 1f;

	private List<string> m_unload_scenes;

	[SerializeField] Image m_leftPanel;
	[SerializeField] Image m_rightPanel;

	float m_leftWidth;
	float m_rightWidth;
	RectTransform left_rt;
	RectTransform right_rt;

	[SerializeField] float m_speed;

	private void Awake()
	{
		m_unload_scenes = new List<string>();

		m_unload_scenes.Add("LoaderScene");
		m_unload_scenes.Add("Map");

		left_rt = m_leftPanel.transform as RectTransform;
		right_rt = m_rightPanel.transform as RectTransform;

		m_leftWidth = left_rt.sizeDelta.x;
		m_rightWidth = right_rt.sizeDelta.x;
	}

	public void TimeScale()
	{
		switch (m_LastTimeScale)
		{
			case 1f:
				m_LastTimeScale = Time.timeScale = 1.5f;
				m_SpeedImage.sprite = m_Speed_2;
				break;
			case 1.5f:
				m_LastTimeScale = Time.timeScale = 2f;
				m_SpeedImage.sprite = m_Speed_3;
				break;
			case 2f:
				m_LastTimeScale = Time.timeScale = 1f;
				m_SpeedImage.sprite = m_Speed_1;
				break;
		}
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
		//m_OptionPanel.gameObject.SetActive(true);
	}
	public void MainMenu()
	{
		Time.timeScale = 1f;

		MusicManager.Instance.FadeVolume(0f);

		SceneManager.LoadScene("MainStartScene", LoadSceneMode.Additive);
		GameObject.DestroyImmediate(m_Managers);
		foreach (var item in m_unload_scenes)
		{
			if (SceneManager.GetSceneByName(item).name == item)
				SceneManager.UnloadSceneAsync(item);
		}

		StartCoroutine(Co_CloseGates());
	}
	public void Exit()
	{
		Application.Quit();
	}

	IEnumerator Co_OpenGates()
	{
		MusicManager.Instance.SetBackGroundMusic(0);

		while (true)
		{
			if (left_rt.anchoredPosition.x < 5f
			  && -right_rt.anchoredPosition.x < 5f)
			{
				left_rt.anchoredPosition = Vector2.zero;
				right_rt.anchoredPosition = Vector2.zero;
				break;
			}

			left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(0, 0, 0), m_speed * Time.fixedDeltaTime);
			right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(0, 0, 0), m_speed * Time.fixedDeltaTime);
			yield return null;
		}

		var objs = GameObject.FindGameObjectsWithTag("MainSceneReceiver");
		foreach (var item in objs)
		{
			var reciever = item.GetComponent<MainSceneReceiver>();
			reciever.__End();
		}

		SceneManager.UnloadSceneAsync("UIScene");
	}
	IEnumerator Co_CloseGates()
	{
		while (true)
		{
			if (m_leftWidth - left_rt.anchoredPosition.x < 5f
				&& -left_rt.anchoredPosition.x - m_rightWidth < 5f)
			{
				left_rt.anchoredPosition3D = new Vector3(m_leftWidth, 0, 0);
				right_rt.anchoredPosition3D = new Vector3(-m_rightWidth, 0, 0);
				break;
			}

			left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(m_leftWidth, 0, 0), m_speed * Time.fixedDeltaTime);
			right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(-m_rightWidth, 0, 0), m_speed * Time.fixedDeltaTime);

			yield return null;
		}

		var objs = GameObject.FindGameObjectsWithTag("MainSceneReceiver");
		foreach (var item in objs)
		{
			var reciever = item.GetComponent<MainSceneReceiver>();
			reciever.__Awake();
		}

		StartCoroutine(Co_OpenGates());
	}

}
