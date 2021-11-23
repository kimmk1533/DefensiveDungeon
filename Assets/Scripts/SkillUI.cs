using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillUI : MonoBehaviour
{
	protected DevilManager M_Devil => DevilManager.Instance;
	Button m_skillBtn;
	bool activebtn=false;
	// Start is called before the first frame update
	void Start()
	{
		m_skillBtn = this.gameObject.GetComponent<Button>();
		if (m_skillBtn.name == "skill1")
		{
			m_skillBtn.onClick.AddListener(Skill1);
		}

		if (m_skillBtn.name == "skill2")
		{
			if (M_Devil.Devil.GetBossType == E_Devil.HellLord)
			{
				m_skillBtn.gameObject.SetActive(false);
				m_skillBtn.enabled = false;
			}
			m_skillBtn.onClick.AddListener(Skill2);
		}


	}
	void Skill1()
	{  
		activebtn = !M_Devil.UseSkill;
		M_Devil.UseSkill = activebtn;
		M_Devil.skillnumber = (int)Devil.E_SkillNumber.Skill1;
	}
	void Skill2()
	{
		activebtn = !M_Devil.UseSkill;
		M_Devil.UseSkill = activebtn;
		M_Devil.skillnumber = (int)Devil.E_SkillNumber.Skill2;
	}
}
