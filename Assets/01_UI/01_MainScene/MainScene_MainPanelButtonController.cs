using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene_MainPanelButtonController : MonoBehaviour
{
	[SerializeField] CanvasGroup m_Title;
	[SerializeField] CanvasGroup m_Canvas;
	[SerializeField] CanvasGroup m_character_panel;
	[SerializeField] float m_FadeSpeed = 5f;

	private IEnumerator Co_FadeTitleAlpha(float alpha, System.Action action)
	{
		while (true)
		{
			m_Title.alpha = Mathf.Lerp(m_Title.alpha, alpha, m_FadeSpeed * Time.deltaTime);

			if (Mathf.Abs(m_Title.alpha - alpha) <= 0.1f)
				break;

			yield return null;
		}

		m_Title.alpha = alpha;
		action?.Invoke();
	}
	private IEnumerator Co_FadeCanvasAlpha(float alpha, System.Action action)
	{
		while (true)
		{
			m_Canvas.alpha = Mathf.Lerp(m_Canvas.alpha, alpha, m_FadeSpeed * Time.deltaTime);

			if (Mathf.Abs(m_Canvas.alpha - alpha) <= 0.1f)
				break;

			yield return null;
		}

		m_Canvas.alpha = alpha;
		action?.Invoke();
	}
	private IEnumerator Co_FadeCharacterAlpha(float alpha, System.Action action)
	{
		while (true)
		{
			m_character_panel.alpha = Mathf.Lerp(m_character_panel.alpha, alpha, m_FadeSpeed * Time.deltaTime);

			if (Mathf.Abs(m_character_panel.alpha - alpha) <= 0.1f)
				break;

			yield return null;
		}

		m_character_panel.alpha = alpha;
		action?.Invoke();
	}

	public void __OnStartButton(Button button)
	{
		//button.enabled = true;
		//this.gameObject.SetActive(false);
		//m_character_panel.gameObject.SetActive(true);

		m_Canvas.blocksRaycasts = false;
		//StartCoroutine(Co_FadeTitleAlpha(0f, null));
		StartCoroutine(Co_FadeCanvasAlpha(0f, () =>
		{
			CharacterSelectManager.Instance.OnStart();

			StartCoroutine(Co_FadeCharacterAlpha(1f, () => m_character_panel.blocksRaycasts = true));
		}));
	}
	public void __OnExitButton()
	{
		// TODO : Save
		Application.Quit();
	}
}
