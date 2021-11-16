using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Devil_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Rank;
	public int Star;
	public float Atk;
	public float HP;
	public float Def;
	public float Crit_rate;
	public float Crit_Dmg;
	public int Atk_Code;
	public int Skill1Code;
	public int Skill2Code;
	public int Prefab;
}



//////////////////////////

[CreateAssetMenu(fileName = "Devil_TableLoader", menuName = "Scriptable Object/Devil_TableLoader")]
public class  Devil_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Devil_TableExcel> DataList;

	private Devil_TableExcel Read(string line)
	{
		line = line.Trim();

		Devil_TableExcel data = new Devil_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Rank = int.Parse(strs[idx++]);
		data.Star = int.Parse(strs[idx++]);
		data.Atk = float.Parse(strs[idx++]);
		data.HP = float.Parse(strs[idx++]);
		data.Def = float.Parse(strs[idx++]);
		data.Crit_rate = float.Parse(strs[idx++]);
		data.Crit_Dmg = float.Parse(strs[idx++]);
		data.Atk_Code = int.Parse(strs[idx++]);
		data.Skill1Code = int.Parse(strs[idx++]);
		data.Skill2Code = int.Parse(strs[idx++]);
		data.Prefab = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Devil_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Devil_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
