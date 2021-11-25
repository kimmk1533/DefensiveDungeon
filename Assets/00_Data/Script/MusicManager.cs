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
	}
	void VolumeSetText()
	{
		volumeText.text = "º¼·ý:" + ((int)(volume * 100f)).ToString();
	}
	// Update is called once per frame
	void Update()
	{

	}
}
