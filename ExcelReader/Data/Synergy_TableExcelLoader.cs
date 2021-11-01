using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Synergy_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Rank;
	public int MemReq;
	public int TargetMem;
	public int EffectType1;
	public int EffectAmount1;
	public int EffectCode1;
	public int EffectChange1;
	public int EffectReq1;
	public float EffectRand1;
	public int EffectType2;
	public int EffectAmount2;
	public int EffectCode2;
	public int EffectChange2;
	public int EffectReq2;
	public float EffectRand2;
	public int Synergy_icon;
	public int Prefab;
	public string Synergy_text;
	public string Synergy_Avility;
}



//////////////////////////

[CreateAssetMenu(fileName = "Synergy_TableLoader", menuName = "Scriptable Object/Synergy_TableLoader")]
public class  Synergy_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Synergy_TableExcel> DataList;

	private Synergy_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Synergy_TableExcel data = new Synergy_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Rank = int.Parse(strs[idx++]);
		data.MemReq = int.Parse(strs[idx++]);
		data.TargetMem = int.Parse(strs[idx++]);
		data.EffectType1 = int.Parse(strs[idx++]);
		data.EffectAmount1 = int.Parse(strs[idx++]);
		data.EffectCode1 = int.Parse(strs[idx++]);
		data.EffectChange1 = int.Parse(strs[idx++]);
		data.EffectReq1 = int.Parse(strs[idx++]);
		data.EffectRand1 = float.Parse(strs[idx++]);
		data.EffectType2 = int.Parse(strs[idx++]);
		data.EffectAmount2 = int.Parse(strs[idx++]);
		data.EffectCode2 = int.Parse(strs[idx++]);
		data.EffectChange2 = int.Parse(strs[idx++]);
		data.EffectReq2 = int.Parse(strs[idx++]);
		data.EffectRand2 = float.Parse(strs[idx++]);
		data.Synergy_icon = int.Parse(strs[idx++]);
		data.Prefab = int.Parse(strs[idx++]);
		data.Synergy_text = strs[idx++];
		data.Synergy_Avility = strs[idx++];

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Synergy_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Synergy_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
