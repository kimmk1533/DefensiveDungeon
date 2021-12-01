using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneLoader : Singleton<MainSceneLoader>
{
	[SerializeField] Image m_background;
	[SerializeField] Animator m_title;
	[SerializeField] Image m_leftPanel;
	[SerializeField] Image m_rightPanel;
	[SerializeField] Image m_eyesPanel;

	[SerializeField] float m_delay;
	[SerializeField] float m_speed;
	[SerializeField] float m_fadespeed;

	public float delay => m_delay;

	RectTransform left_rt;
	RectTransform right_rt;

	public event Action OnMainStartSceneLoadedEvent;

	private void Awake()
	{
		left_rt = m_leftPanel.transform as RectTransform;
		right_rt = m_rightPanel.transform as RectTransform;

		OnMainStartSceneLoadedEvent += () =>
		{
			var objs = GameObject.FindGameObjectsWithTag("MainSceneReceiver");
			foreach (var item in objs)
			{
				var reciever = item.GetComponent<MainSceneReceiver>();
				reciever.__Awake();
			}
		};

		SceneManager.LoadScene("MainStartScene", LoadSceneMode.Additive);
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainStartScene"));
		StartCoroutine(Co_FadeBackground());
		StartCoroutine(Co_OpenGates());
	}

	IEnumerator Co_FadeBackground()
	{
		yield return new WaitForSeconds(m_delay);

		Color color = m_background.color;

		while (true)
		{
			color.a = Mathf.Lerp(color.a, 0f, m_fadespeed * Time.fixedDeltaTime);
			m_background.color = color;

			if (color.a < 0.1f)
				break;

			yield return null;
		}
	}
	IEnumerator Co_OpenGates()
	{
		yield return new WaitForSeconds(m_delay);

		m_eyesPanel.gameObject.SetActive(false);

		while (true)
		{
			if (left_rt.anchoredPosition.x < 5f
			  && -right_rt.anchoredPosition.x < 5f)
			{
				left_rt.anchoredPosition = Vector2.zero;
				right_rt.anchoredPosition = Vector2.zero;
				break;
			}

			left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.fixedDeltaTime * m_speed);
			right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.fixedDeltaTime * m_speed);

			yield return null;
		}

		m_title.SetTrigger("Start");
		OnMainStartSceneLoadedEvent?.Invoke();
	}

	public void End()
	{
		var objs = GameObject.FindGameObjectsWithTag("MainSceneReceiver");
		foreach (var item in objs)
		{
			var reciever = item.GetComponent<MainSceneReceiver>();
			reciever.__End();
		}

		SceneManager.UnloadSceneAsync("ManagerScene");
	}
}
