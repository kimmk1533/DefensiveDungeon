using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum E_Devil
{
	None,

	HateQueen,
	HellLord,
	FrostLich,

	Max
}
public class DevilManager : Singleton<DevilManager>
{
	protected Devil_TableExcelLoader m_DevilData;
	protected Prefab_TableExcelLoader m_PrefabData;

	protected Devil m_Devil;

	bool m_IsDrawGizmo = false;
	bool m_IsSkillRange = false;
	public GameObject skillRange;
	#region 내부 프로퍼티
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected UserInfoManager M_UserInfo => UserInfoManager.Instance;
	protected DevilSkillUIManager M_DevilSkillUI => DevilSkillUIManager.Instance;
	#endregion
	#region 외부 프로퍼티
	public Devil Devil => m_Devil;
	public bool ActiveSkillRange
	{
		get => m_IsSkillRange;
		set => m_IsSkillRange = value;
	}
	public GameObject SkillRangeObj
	{
		get => skillRange;
		set => skillRange = value;
	}
	//public 
	public event Action OnUseSkillEvent
	{
		add
		{
			m_Devil.OnUseSkillEvent += value;
		}
		remove
		{
			m_Devil.OnUseSkillEvent -= value;
		}
	}
	public event Action OnSkillCountChangedEvent
	{
		add
		{
			m_Devil.OnSkillCountChangedEvent += value;
		}
		remove
		{
			m_Devil.OnSkillCountChangedEvent -= value;
		}
	}

	public bool IsDrawGizmo { get => m_IsDrawGizmo; set => m_IsDrawGizmo = value; }

	public int Skill01_ChargeCount { get => m_Devil.Skill01.m_CurrentCharge; }
	public int Skill01_MaxChargeCount { get => m_Devil.Skill01.m_MaxCharge; }
	public int Skill02_ChargeCount { get => m_Devil.Skill02.m_CurrentCharge; }
	public int Skill02_MaxChargeCount { get => m_Devil.Skill02.m_MaxCharge; }
	public float Skill01_CoolTime { get => m_Devil.Skill01.m_Cooltime; }
	public float Skill01_CoolTimeTimer { get => m_Devil.Skill01.m_CooltimeTimer; }
	public float Skill02_CoolTime { get => m_Devil.Skill02.m_Cooltime; }
	public float Skill02_CoolTimeTimer { get => m_Devil.Skill02.m_CooltimeTimer; }
	public int Skill01_Icon { get => m_Devil.Skill01.m_ConditionData.Skill_icon; }
	public int Skill02_Icon { get => m_Devil.Skill02.m_ConditionData.Skill_icon; }
	#endregion

	#region 외부 함수
	public void SelectDevil(int code)
	{
		Devil_TableExcel data = GetData(code);

		Node node = (new GameObject("Devil Node")).AddComponent<Node>();
		node.transform.SetParent(transform);
		node.transform.position = Vector3.zero;
		node.m_NodeType = E_NodeType.None;
		node.m_Direction = E_Direction.None;
		node.gameObject.layer = LayerMask.NameToLayer("Node");

		// 마왕 툴팁용 콜라이더
		//BoxCollider collider = node.gameObject.AddComponent<BoxCollider>();
		//collider.isTrigger = true;
		//collider.size.Set(6f, 6f, 6f);
		//collider.center.Set(0f, 3f, 0f);

		GameObject devil = GameObject.Instantiate(m_PrefabData.GetPrefab(data.Prefab));
		devil.transform.SetParent(node.transform);
		devil.transform.position = Vector3.zero;
		devil.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		float size = m_PrefabData.DataList.Find((item) => { return item.Code == data.Prefab; }).Size;
		devil.transform.Find("Mesh").localScale = Vector3.one * size;

		switch ((E_Devil)data.No)
		{
			case E_Devil.HateQueen:
				m_Devil = devil.AddComponent<HateQueen>();
				break;
			case E_Devil.HellLord:
				m_Devil = devil.AddComponent<HellLord>();
				break;
			case E_Devil.FrostLich:
				m_Devil = devil.AddComponent<FrostLich>();
				break;
		}

		node.SetDevil(m_Devil);
	}
	public Devil_TableExcel GetData(E_Devil no)
	{
		Devil_TableExcel result = m_DevilData.DataList.Where(item => item.No == (int)no).SingleOrDefault();

		return result;
	}
	public Devil_TableExcel GetData(int code)
	{
		Devil_TableExcel result = m_DevilData.DataList.Where(item => item.Code == code).SingleOrDefault();

		return result;
	}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		m_DevilData = M_DataTable.GetDataTable<Devil_TableExcelLoader>();
		m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
	}
	private void Start()
	{
		SelectDevil(M_UserInfo.DevilCode);
	}
	private void Update()
	{
		if (m_Devil.IsCastingSkill01)
		{
			m_Devil.DevilSkillCasting(Devil.E_SkillNumber.Skill1);
		}
		if (m_Devil.IsCastingSkill02)
		{
			m_Devil.DevilSkillCasting(Devil.E_SkillNumber.Skill2);
		}
		//if (m_IsSkillRange)
		//{
		//	skillRange.SetActive(true);
		//}
		//else skillRange.SetActive(false);
	}
	#endregion
	#region My Callback Func
	public void _Start()
	{
		switch (m_Devil.GetBossType)
		{
			case E_Devil.HellLord:
				{
					// 스킬01
					M_DevilSkillUI.OnSkill01ButtonClickedEvent.AddListener(() =>
					{
						if (ActiveSkillRange)
						{
							ActiveSkillRange = false;
						}

						m_Devil.IsCastingSkill02 = false;

						if (Skill01_ChargeCount > 0)
						{
							m_Devil.IsCastingSkill01 = !m_Devil.IsCastingSkill01;
						}
						else
						{
							m_Devil.IsCastingSkill01 = false;
						}
					});
				}
				break;
			default:
				{
					// 스킬01
					M_DevilSkillUI.OnSkill01ButtonClickedEvent.AddListener(() =>
					{
						if (ActiveSkillRange)
						{
							ActiveSkillRange = false;
						}

						if (Skill01_ChargeCount > 0)
						{
							m_Devil.IsCastingSkill01 = !m_Devil.IsCastingSkill01;
							m_Devil.IsCastingSkill02 = false;
						}
						else
						{
							m_Devil.IsCastingSkill01 = false;
						}
					});
					// 스킬02
					M_DevilSkillUI.OnSkill02ButtonClickedEvent.AddListener(() =>
					{
						if (ActiveSkillRange)
						{
							ActiveSkillRange = false;
						}

						if (Skill02_ChargeCount > 0)
						{
							m_Devil.IsCastingSkill01 = false;
							m_Devil.IsCastingSkill02 = !m_Devil.IsCastingSkill02;
						}
						else
						{
							m_Devil.IsCastingSkill02 = false;
						}
					});
				}
				break;
		}
	}
	#endregion
}
