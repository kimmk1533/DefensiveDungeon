using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicManager : Singleton<MusicManager>
{
	public Slider backVolume;
	public AudioSource audio;
	public Text volumeText;
	float volume = 1f;
	int MainBGM = 920049;
	int ArcaneTrailerBGM = 920044;
	int CombatPercussionLoopBGM = 920045;
	int TriumphantVictory = 920051;
	int DeathBrassBGM = 920046;
	bool play_delay;
	float time;
	float delayTime = 5f;
	enum E_BGMType
	{
		MAINBGM,
		ARCANEBGM,
		COMBATBGM,
		TRIUM,
		DEATH
	}
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected Sound_TableExcelLoader m_SoundData;
	protected Sound_TableExcelLoader SoundData
	{
		get
		{
			if (null == m_SoundData)
			{
				m_SoundData = M_DataTable.GetDataTable<Sound_TableExcelLoader>();
			}

			return m_SoundData;
		}
	}

	public void SetSelectViewMusic(int type)
	{
		int bgm=MainBGM;
		play_delay = false;
		switch (type)
		{
			case 0:
				bgm = MainBGM;
				break;
			case 1:
				bgm = ArcaneTrailerBGM;
				break;
			case 2:
				play_delay = true;
				bgm=CombatPercussionLoopBGM;
				break;
			case 3:
				bgm = TriumphantVictory;
				break;
			case 4:
				bgm = DeathBrassBGM;
				break;
		}
		audio.clip = SoundData.GetAudio(bgm);
		if (play_delay) return;
		audio.Play();
	}
	public void PlaySound(SoundInfo soundinfo)
	{
		soundinfo.clip= m_SoundData.GetAudio(soundinfo.Code);
		audio.Play();
	}
	void VolumeSetText()
	{
		volumeText.text = "º¼·ý:" + ((int)(volume * 100f)).ToString();
	}
	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		VolumeSetText();
		backVolume.value = 1f;
		backVolume.onValueChanged.AddListener((value) =>
		{
			volume = value;
			VolumeSetText();
		});
		audio.clip = SoundData.GetAudio(MainBGM);
		audio.volume = SoundData.DataList.Find((excel) =>
		{
			return excel.Code == MainBGM;
		}).Volume;
		audio.Play();
	}
	void Update()
	{
		if (play_delay)
		{
			if (time >= delayTime)
			{
				audio.Play();
				time = 0f;
				return;
			}
			time += Time.deltaTime;
		}
	}
}
