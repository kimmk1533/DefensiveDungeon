using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class MusicManager : Singleton<MusicManager>
{
	[SerializeField]
	private AudioSource m_Audio;

	[SerializeField]
	private float m_FadeSpeed = 2.5f;

	[SerializeField, ReadOnly]
	float optionVolume = 1f;
	int MainBGM = 920049;
	int ArcaneTrailerBGM = 920044;
	int CombatPercussionLoopBGM = 920045;
	int TriumphantVictory = 920051;
	int DeathBrassBGM = 920046;

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

	private Coroutine m_Co_FadeVolume;
	private Coroutine m_Co_BGM;
	private IEnumerator Co_FadeVolume(float volume)
	{
		while (true)
		{
			m_Audio.volume = Mathf.Lerp(m_Audio.volume, volume, m_FadeSpeed * Time.deltaTime);

			if (Mathf.Abs(m_Audio.volume - volume) < 0.1f)
				break;

			yield return null;
		}

		m_Audio.volume = volume;
		m_Co_FadeVolume = null;
	}
	private IEnumerator Co_SetBGM(int bgm)
	{
		if (null != m_Co_FadeVolume)
			StopCoroutine(m_Co_FadeVolume);

		if (m_Audio.volume != 0f)
			m_Co_FadeVolume = StartCoroutine(Co_FadeVolume(0f));

		while (true)
		{
			if (null == m_Co_FadeVolume)
				break;

			yield return null;
		}

		m_Audio.clip = SoundData.GetAudio(bgm);
		m_Audio.Play();

		m_Co_FadeVolume = StartCoroutine(Co_FadeVolume(excel_volume_list[current_BGM] * optionVolume));
	}
	public void SetBackGroundMusic(int type)
	{
		int bgm = -1;

		switch (type)
		{
			case 0:
				bgm = MainBGM;
				break;
			case 1:
				bgm = ArcaneTrailerBGM;
				break;
			case 2:
				bgm = CombatPercussionLoopBGM;
				break;
			case 3:
				bgm = TriumphantVictory;
				m_Audio.loop = false;

				m_Audio.clip = SoundData.GetAudio(bgm);
				m_Audio.Play();
				return;
			case 4:
				bgm = DeathBrassBGM;
				m_Audio.loop = false;

				m_Audio.clip = SoundData.GetAudio(bgm);
				m_Audio.Play();
				return;
			default:
				if (null != m_Co_FadeVolume)
					StopCoroutine(m_Co_FadeVolume);
				m_Co_FadeVolume = StartCoroutine(Co_FadeVolume(0f));
				return;
		}

		current_BGM = (E_BGMType)type;

		if (null != m_Co_BGM)
			StopCoroutine(m_Co_BGM);
		m_Co_BGM = StartCoroutine(Co_SetBGM(bgm));
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
		m_Audio.PlayOneShot(audioClip, m_sound_excel.Volume * optionVolume);
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
					(i, SoundData.DataList.Find((excel) =>
					{
						return excel.Code == excel_code_list[i];
					}).Volume);
		}

		current_BGM = E_BGMType.MAINBGM;

		M_Option.UpdateVolume.AddListener((value) =>
		{
			optionVolume = value;
			m_Audio.volume = excel_volume_list[current_BGM] * optionVolume;
		});

		m_Audio.clip = SoundData.GetAudio(MainBGM);
		m_Co_FadeVolume = StartCoroutine(Co_FadeVolume(excel_volume_list[current_BGM] * optionVolume));
		m_Audio.Play();
	}
}
