using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class MusicManager : Singleton<MusicManager>
{
	public AudioSource audio;

	[SerializeField, ReadOnly]
	float optionVolume = 1f;
	int MainBGM = 920049;
	int ArcaneTrailerBGM = 920044;
	int CombatPercussionLoopBGM = 920045;
	int TriumphantVictory = 920051;
	int DeathBrassBGM = 920046;
	bool play_delay;
	float time;
	float delayTime = 5f;
	E_BGMType current_BGM = E_BGMType.None;
	Dictionary<E_BGMType, int> excel_code_list;
	Dictionary<E_BGMType, float> excel_volume_list;
	Sound_TableExcel m_sound_excel;
	public enum E_BGMType
	{
		None = -1,

		MAINBGM,
		ARCANEBGM,
		COMBATBGM,
		TRIUM,
		DEATH,

		Max
	}
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected OptionManager M_Option => OptionManager.Instance;
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
		int bgm = MainBGM;
		play_delay = false;
		current_BGM = (E_BGMType)type + 1;
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
				bgm = CombatPercussionLoopBGM;
				break;
			case 3:
				bgm = TriumphantVictory;
				break;
			case 4:
				bgm = DeathBrassBGM;
				break;
		}

		audio.volume = excel_volume_list[current_BGM] * optionVolume;

		audio.clip = SoundData.GetAudio(bgm);
		if (play_delay) return;
		audio.Play();
	}
	public Sound_TableExcel GetData(int code)
	{
		Sound_TableExcel result = m_SoundData.DataList.Where(item => item.Code == code).SingleOrDefault();

		return result;
	}
	public void PlaySound(int soundcode)
	{
		m_sound_excel = GetData(soundcode);
		AudioClip audioClip = m_SoundData.GetAudio(soundcode);
		audio.PlayOneShot(audioClip, m_sound_excel.Volume * optionVolume);
	}

	void Start()
	{
		DontDestroyOnLoad(this.gameObject);

		excel_code_list = new Dictionary<E_BGMType, int>();
		excel_volume_list = new Dictionary<E_BGMType, float>();

		excel_code_list.Add(E_BGMType.MAINBGM, MainBGM);
		excel_code_list.Add(E_BGMType.ARCANEBGM, ArcaneTrailerBGM);
		excel_code_list.Add(E_BGMType.COMBATBGM, CombatPercussionLoopBGM);
		excel_code_list.Add(E_BGMType.TRIUM, TriumphantVictory);
		excel_code_list.Add(E_BGMType.DEATH, DeathBrassBGM);

		for (E_BGMType i = E_BGMType.None + 1; i < E_BGMType.Max; ++i)
		{
			excel_volume_list.Add
					(i, audio.volume = SoundData.DataList.Find((excel) =>
					{
						return excel.Code == excel_code_list[i];
					}).Volume);
		}

		current_BGM = E_BGMType.MAINBGM;

		M_Option.UpdateVolume.AddListener((value) =>
		{
			optionVolume = value;
			audio.volume = optionVolume * excel_volume_list[current_BGM];
		});

		audio.clip = SoundData.GetAudio(MainBGM);
		audio.volume = SoundData.DataList.Find((excel) => { return excel.Code == MainBGM; }).Volume * optionVolume;
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
