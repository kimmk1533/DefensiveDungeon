using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillStat_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Max_Charge;
	public float CoolTime;
	public float Dmg;
	public float Dmg_plus;
	public float Range;
	public float Speed;
	public int Target_num;
	public float Size;
	public float Life_Time;
	public int Buff_CC;
	public int LoadCode;
}



//////////////////////////

[CreateAssetMenu(fileName = "SkillStat_TableLoader", menuName = "Scriptable Object/SkillStat_TableLoader")]
public class  SkillStat_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<SkillStat_TableExcel> DataList;

	private SkillStat_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		SkillStat_TableExcel data = new SkillStat_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Max_Charge = int.Parse(strs[idx++]);
		data.CoolTime = float.Parse(strs[idx++]);
		data.Dmg = float.Parse(strs[idx++]);
		data.Dmg_plus = float.Parse(strs[idx++]);
		data.Range = float.Parse(strs[idx++]);
		data.Speed = float.Parse(strs[idx++]);
		data.Target_num = int.Parse(strs[idx++]);
		data.Size = float.Parse(strs[idx++]);
		data.Life_Time = float.Parse(strs[idx++]);
		data.Buff_CC = int.Parse(strs[idx++]);
		data.LoadCode = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<SkillStat_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			SkillStat_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
