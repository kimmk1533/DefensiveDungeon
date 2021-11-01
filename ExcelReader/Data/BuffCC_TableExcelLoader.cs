using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuffCC_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int BuffType1;
	public int AddType1;
	public float BuffAmount1;
	public float BuffRand1;
	public int Summon1;
	public int BuffType2;
	public int AddType2;
	public float BuffAmount2;
	public float BuffRand2;
	public int Summon2;
	public int BuffType3;
	public int AddType3;
	public float BuffAmount3;
	public float BuffRand3;
	public int Summon3;
	public float Duration;
	public int Prefab;
}



//////////////////////////

[CreateAssetMenu(fileName = "BuffCC_TableLoader", menuName = "Scriptable Object/BuffCC_TableLoader")]
public class  BuffCC_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<BuffCC_TableExcel> DataList;

	private BuffCC_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		BuffCC_TableExcel data = new BuffCC_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.BuffType1 = int.Parse(strs[idx++]);
		data.AddType1 = int.Parse(strs[idx++]);
		data.BuffAmount1 = float.Parse(strs[idx++]);
		data.BuffRand1 = float.Parse(strs[idx++]);
		data.Summon1 = int.Parse(strs[idx++]);
		data.BuffType2 = int.Parse(strs[idx++]);
		data.AddType2 = int.Parse(strs[idx++]);
		data.BuffAmount2 = float.Parse(strs[idx++]);
		data.BuffRand2 = float.Parse(strs[idx++]);
		data.Summon2 = int.Parse(strs[idx++]);
		data.BuffType3 = int.Parse(strs[idx++]);
		data.AddType3 = int.Parse(strs[idx++]);
		data.BuffAmount3 = float.Parse(strs[idx++]);
		data.BuffRand3 = float.Parse(strs[idx++]);
		data.Summon3 = int.Parse(strs[idx++]);
		data.Duration = float.Parse(strs[idx++]);
		data.Prefab = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<BuffCC_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			BuffCC_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
