using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sound_TableExcel
{
	public int Code;
	public float Volume;
	public string Unity_address;
}

[System.Serializable]
public class SoundInfo
{
	public int Code;
	public float Volume;
	public AudioClip clip;
}

//////////////////////////

[CreateAssetMenu(fileName = "Sound_TableLoader", menuName = "Scriptable Object/Sound_TableLoader")]
public class Sound_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Sound_TableExcel> DataList;
	[SerializeField] List<SoundInfo> SoundList;

	private Sound_TableExcel Read(string line)
	{
		line = line.Trim();

		Sound_TableExcel data = new Sound_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.Code = int.Parse(strs[idx++]);
		data.Volume = float.Parse(strs[idx++]);
		data.Unity_address = strs[idx++];

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Sound_TableExcel>();
		SoundList = new List<SoundInfo>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Sound_TableExcel data = Read(item);
			DataList.Add(data);

			AudioClip audio_obj = Resources.Load<AudioClip>(data.Unity_address);
			SoundList.Add(new SoundInfo()
			{
				Code = data.Code,
				Volume = data.Volume,
				clip = audio_obj
			});
		}
	}

	public AudioClip GetAudio(int clipCode)
	{
		var info = SoundList.Find((item) => { return item.Code == clipCode; });
		return info?.clip;
	}
}
