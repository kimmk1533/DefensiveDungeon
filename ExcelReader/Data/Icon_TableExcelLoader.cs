using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Icon_TableExcel
{
	public int Code;
	public string Unity_address;
}



//////////////////////////

[CreateAssetMenu(fileName = "Icon_TableLoader", menuName = "Scriptable Object/Icon_TableLoader")]
public class  Icon_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Icon_TableExcel> DataList;

	private Icon_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Icon_TableExcel data = new Icon_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.Code = int.Parse(strs[idx++]);
		data.Unity_address = strs[idx++];

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Icon_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Icon_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
