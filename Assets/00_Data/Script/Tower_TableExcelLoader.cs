using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tower_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Rank;
	public int Star;
	public int Tower_Kinds;
	public float Atk;
	public float HP;
	public float Def;
	public float Crit_rate;
	public float Crit_Dmg;
	public int Atk_Code;
	public int Skill1Code;
	public int Skill2Code;
	public int Type1;
	public int Type2;
	public int Next_Stat;
	public float Price;
	public int Prefab;
}



//////////////////////////

[CreateAssetMenu(fileName = "Tower_TableLoader", menuName = "Scriptable Object/Tower_TableLoader")]
public class  Tower_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Tower_TableExcel> DataList;

	private Tower_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Tower_TableExcel data = new Tower_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Rank = int.Parse(strs[idx++]);
		data.Star = int.Parse(strs[idx++]);
		data.Tower_Kinds = int.Parse(strs[idx++]);
		data.Atk = float.Parse(strs[idx++]);
		data.HP = float.Parse(strs[idx++]);
		data.Def = float.Parse(strs[idx++]);
		data.Crit_rate = float.Parse(strs[idx++]);
		data.Crit_Dmg = float.Parse(strs[idx++]);
		data.Atk_Code = int.Parse(strs[idx++]);
		data.Skill1Code = int.Parse(strs[idx++]);
		data.Skill2Code = int.Parse(strs[idx++]);
		data.Type1 = int.Parse(strs[idx++]);
		data.Type2 = int.Parse(strs[idx++]);
		data.Next_Stat = int.Parse(strs[idx++]);
		data.Price = float.Parse(strs[idx++]);
		data.Prefab = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Tower_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Tower_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
