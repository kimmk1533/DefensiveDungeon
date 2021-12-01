using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionManager : Singleton<OptionManager>
{
	Dictionary<KeyOptionType, KeyCode> _KeyCode;

	[SerializeField] Slider backVolume;
	[SerializeField] Text volumeText;

	public Slider.SliderEvent UpdateVolume
	{
		get => backVolume.onValueChanged;
	}
	public KeyCode GetKeyCode(KeyOptionType key)
	{
		return _KeyCode[key];
	}
	public void ClearKeyCode(KeyOptionType key)
	{
		_KeyCode.Remove(key);
	}
	public void SetKeyCode(KeyOptionType key,KeyCode code)
	{
		_KeyCode[key] = code;
	}

	public void InitAudioOption(Slider slider, Text text)
	{
		backVolume = slider;
		volumeText = text;

		backVolume.value = 1f;
		volumeText.text = "º¼·ý:" + ((int)(backVolume.value * 100f)).ToString();
		backVolume.onValueChanged.RemoveAllListeners();
		backVolume.onValueChanged.AddListener((value) =>
		{
			volumeText.text = "º¼·ý:" + (value * 100f).ToString("F2");
		});
	}

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		_KeyCode = new Dictionary<KeyOptionType, KeyCode>();

		SetKeyCode(KeyOptionType.Qkey, KeyCode.Q);
		SetKeyCode(KeyOptionType.Ekey, KeyCode.E);
		SetKeyCode(KeyOptionType.Skill1, KeyCode.Z);
		SetKeyCode(KeyOptionType.Skill2, KeyCode.X);
		SetKeyCode(KeyOptionType.LevelUp, KeyCode.F);
		SetKeyCode(KeyOptionType.ReShop, KeyCode.D);
		SetKeyCode(KeyOptionType.Synerge_North, KeyCode.Alpha1);
		SetKeyCode(KeyOptionType.Synerge_East, KeyCode.Alpha2);
		SetKeyCode(KeyOptionType.Synerge_South, KeyCode.Alpha3);
		SetKeyCode(KeyOptionType.Synerge_West, KeyCode.Alpha4);
		SetKeyCode(KeyOptionType.ActiveShop, KeyCode.Space);
	}
}
