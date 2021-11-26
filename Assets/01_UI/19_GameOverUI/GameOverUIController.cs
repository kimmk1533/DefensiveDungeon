using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameEndData
{
	public bool IsWin;
}

public delegate void GameOverHandler(GameEndData data);

public class GameOverUIController : MonoBehaviour
{
	[SerializeField] TMPro.TextMeshProUGUI m_win_lose_textpro;
	[SerializeField] TMPro.TextMeshProUGUI m_textTextpro;
	[SerializeField] GameOverDataLine m_line1;
	[SerializeField] GameOverDataLine m_line2;
	[SerializeField] GameOverDataLine m_line3;
	[SerializeField] GameOverProgressBar m_progressBar;
	protected MusicManager M_MusicAudio => MusicManager.Instance;
	private void Start()
	{
		StageInfoManager.Instance.OnGameEndEvent += SetUI;
		DevilManager.Instance.Devil.OnGameEndEvent += SetUI;
	}

	public void SetUI(GameEndData data)
	{
		GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero; //Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

		if (data.IsWin)
		{
			M_MusicAudio.SetSelectViewMusic(3);
			m_win_lose_textpro.text = "승리";
			m_textTextpro.text = "긴 전투 끝에 용사들을 막아내고 마왕군이 승리했습니다.";
			m_line1.SetUI("승리 보너스", 300);
			m_line2.SetUI("웨이브 보너스", 400);
			m_line3.SetUI("총 경험치", 300 + 400);
			m_progressBar.SetUI(700, 1000);
		}
		else
		{
			M_MusicAudio.SetSelectViewMusic(4);
			m_win_lose_textpro.text = "패배";
			m_textTextpro.text = "마왕군은 결국 용사들을 막아내지 못했습니다.";
			m_line1.SetUI("승리 보너스", 0);
			m_line2.SetUI("웨이브 보너스", 200);
			m_line3.SetUI("총 경험치", 0 + 200);
			m_progressBar.SetUI(200, 1000);
		}

		Time.timeScale = 0f;
	}

	public void __OnToMainButtonClicked()
	{
		// TODO...
		// TODO : call all finalize first

		Debug.Log("---------------------------GAME END---------------------------");
	}
}
