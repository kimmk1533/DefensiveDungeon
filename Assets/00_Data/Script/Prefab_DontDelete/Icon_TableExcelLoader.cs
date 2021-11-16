using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Icon_TableExcel
{
	public int Code;
	public string Unity_address;
}

[System.Serializable]
public class IconInfo
{
	public int Code;
	public Sprite obj;
}

//////////////////////////

[CreateAssetMenu(fileName = "Icon_TableLoader", menuName = "Scriptable Object/Icon_TableLoader")]
public class Icon_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Prefab_TableExcel> DataList;
	[SerializeField] List<IconInfo> IconList;

	private Prefab_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Prefab_TableExcel data = new Prefab_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.Code = int.Parse(strs[idx++]);
		data.Unity_address = strs[idx++];

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Prefab_TableExcel>();
		IconList = new List<IconInfo>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;

			Prefab_TableExcel data = Read(item);
			DataList.Add(data);

			Sprite sprite = Resources.Load<Sprite>(data.Unity_address);
			IconList.Add(new IconInfo()
			{ Code = data.Code, obj = sprite });
		}
	}

	public Sprite GetIcon(int iconCode)
	{
		//Debug.Log(spriteCode);
		var info = IconList.Find((item) => { return item.Code == iconCode; });
		return info.obj;
	}
}
