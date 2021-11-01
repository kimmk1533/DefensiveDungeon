using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillCondition_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Ally;
	public bool Air_Attack;
	public int Atk_type;
	public int Atk_pick;
	public int Target_type;
	public int Move_type;
	public float Move_Height;
	public int PassiveCode;
	public int Atk_prefab;
	public int projectile_prefab;
	public int damage_prefab;
	public int Skill_icon;
	public string Skill_text;
	public string SkillAvility1_Name;
	public string SkillAvility1_Text;
	public string SkillAvility2_Name;
	public string SkillAvility2_Text;
}



//////////////////////////

[CreateAssetMenu(fileName = "SkillCondition_TableLoader", menuName = "Scriptable Object/SkillCondition_TableLoader")]
public class  SkillCondition_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<SkillCondition_TableExcel> DataList;

	private SkillCondition_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		SkillCondition_TableExcel data = new SkillCondition_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Ally = int.Parse(strs[idx++]);
		data.Air_Attack = bool.Parse(strs[idx++]);
		data.Atk_type = int.Parse(strs[idx++]);
		data.Atk_pick = int.Parse(strs[idx++]);
		data.Target_type = int.Parse(strs[idx++]);
		data.Move_type = int.Parse(strs[idx++]);
		data.Move_Height = float.Parse(strs[idx++]);
		data.PassiveCode = int.Parse(strs[idx++]);
		data.Atk_prefab = int.Parse(strs[idx++]);
		data.projectile_prefab = int.Parse(strs[idx++]);
		data.damage_prefab = int.Parse(strs[idx++]);
		data.Skill_icon = int.Parse(strs[idx++]);
		data.Skill_text = strs[idx++];
		data.SkillAvility1_Name = strs[idx++];
		data.SkillAvility1_Text = strs[idx++];
		data.SkillAvility2_Name = strs[idx++];
		data.SkillAvility2_Text = strs[idx++];

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<SkillCondition_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			SkillCondition_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
