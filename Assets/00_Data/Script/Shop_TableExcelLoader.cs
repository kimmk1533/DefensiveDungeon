using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Shop_TableExcel
{
	public int No;
	public int User_Level;
	public int Tower_Rank1;
	public float Tower_Rand1;
	public int Tower_Rank2;
	public float Tower_Rand2;
	public int Tower_Rank3;
	public float Tower_Rand3;
	public int Tower_Rank4;
	public float Tower_Rand4;
	public int Tower_Rank5;
	public float Tower_Rand5;
	public int Tower_Gold1;
	public int Tower_Gold2;
	public int Tower_Gold3;
	public int Tower_Gold4;
	public int Tower_Gold5;
	public int Reset_Gold;
}



//////////////////////////

[CreateAssetMenu(fileName = "Shop_TableLoader", menuName = "Scriptable Object/Shop_TableLoader")]
public class  Shop_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Shop_TableExcel> DataList;

	private Shop_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Shop_TableExcel data = new Shop_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.User_Level = int.Parse(strs[idx++]);
		data.Tower_Rank1 = int.Parse(strs[idx++]);
		data.Tower_Rand1 = float.Parse(strs[idx++]);
		data.Tower_Rank2 = int.Parse(strs[idx++]);
		data.Tower_Rand2 = float.Parse(strs[idx++]);
		data.Tower_Rank3 = int.Parse(strs[idx++]);
		data.Tower_Rand3 = float.Parse(strs[idx++]);
		data.Tower_Rank4 = int.Parse(strs[idx++]);
		data.Tower_Rand4 = float.Parse(strs[idx++]);
		data.Tower_Rank5 = int.Parse(strs[idx++]);
		data.Tower_Rand5 = float.Parse(strs[idx++]);
		data.Tower_Gold1 = int.Parse(strs[idx++]);
		data.Tower_Gold2 = int.Parse(strs[idx++]);
		data.Tower_Gold3 = int.Parse(strs[idx++]);
		data.Tower_Gold4 = int.Parse(strs[idx++]);
		data.Tower_Gold5 = int.Parse(strs[idx++]);
		data.Reset_Gold = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Shop_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Shop_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
