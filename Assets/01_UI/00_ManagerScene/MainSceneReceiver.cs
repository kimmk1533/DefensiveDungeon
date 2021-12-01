using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneReceiver : MonoBehaviour
{
	[SerializeField] Slider backVolume;
	[SerializeField] Text volumeText;

	[SerializeField] Button startButton;
	[SerializeField] Button backButton;

	[SerializeField] CanvasGroup mainbutton;
	[SerializeField] CanvasGroup title;

	[SerializeField] float fadeSpeed = 1f;

	public void __Awake()
	{
		OptionManager.Instance.InitAudioOption(backVolume, volumeText);
		startButton.onClick.AddListener(() => MusicManager.Instance.SetBackGroundMusic(1));
		backButton.onClick.AddListener(() => MusicManager.Instance.SetBackGroundMusic(0));
	}
	public void __End()
	{
		title.alpha = 1f;
		StartCoroutine(Co_FadeMainButton());

		CharacterSelectManager.Instance.gameObject.SetActive(true);
	}

	IEnumerator Co_FadeMainButton()
	{
		while (true)
		{
			mainbutton.alpha = Mathf.Lerp(mainbutton.alpha, 1f, fadeSpeed * Time.fixedDeltaTime);

			if (mainbutton.alpha > 0.9f)
				break;

			yield return null;
		}

		mainbutton.alpha = 1f;
	}
}
