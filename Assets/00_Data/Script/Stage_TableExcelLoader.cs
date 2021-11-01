using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stage_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Stage_Num;
	public int StageType;
	public float StageTime;
	public int StageEnemyTable;
	public int Exp;
	public int Gold;
	public int Stage_icon;
	public int Prefab;
}



//////////////////////////

[CreateAssetMenu(fileName = "Stage_TableLoader", menuName = "Scriptable Object/Stage_TableLoader")]
public class  Stage_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Stage_TableExcel> DataList;

	private Stage_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Stage_TableExcel data = new Stage_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Stage_Num = int.Parse(strs[idx++]);
		data.StageType = int.Parse(strs[idx++]);
		data.StageTime = float.Parse(strs[idx++]);
		data.StageEnemyTable = int.Parse(strs[idx++]);
		data.Exp = int.Parse(strs[idx++]);
		data.Gold = int.Parse(strs[idx++]);
		data.Stage_icon = int.Parse(strs[idx++]);
		data.Prefab = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Stage_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Stage_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
