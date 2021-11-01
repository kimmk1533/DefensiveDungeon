using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Level_TableExcel
{
	public int No;
	public int User_Level;
	public int LvUP_Exp;
	public int Exp_Buy;
	public int Buy_Gold;
}



//////////////////////////

[CreateAssetMenu(fileName = "Level_TableLoader", menuName = "Scriptable Object/Level_TableLoader")]
public class  Level_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Level_TableExcel> DataList;

	private Level_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Level_TableExcel data = new Level_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.User_Level = int.Parse(strs[idx++]);
		data.LvUP_Exp = int.Parse(strs[idx++]);
		data.Exp_Buy = int.Parse(strs[idx++]);
		data.Buy_Gold = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Level_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Level_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
