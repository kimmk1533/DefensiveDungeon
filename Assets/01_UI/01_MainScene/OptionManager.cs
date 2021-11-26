using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionManager : Singleton<OptionManager>
{
	public Slider backVolume;

	public Text volumeText;
	Dictionary<KeyOptionType, KeyCode> _KeyCode;

	public KeyCode GetKeyCode(KeyOptionType key)
	{
		return _KeyCode[key];
	}
	public void SetKeyCode(KeyOptionType key,KeyCode code)
	{
		_KeyCode[key] = code;
	}
	public Slider.SliderEvent UpdateVolume
	{
		get => backVolume.onValueChanged;
	}
	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		_KeyCode = new Dictionary<KeyOptionType, KeyCode>();
		backVolume.value = 1f;
		volumeText.text = "º¼·ý:" + ((int)(backVolume.value * 100f)).ToString();
		backVolume.onValueChanged.AddListener((value) =>
		{
			volumeText.text = "º¼·ý:" + (value * 100f).ToString("F2");
		});
	}
	

}
