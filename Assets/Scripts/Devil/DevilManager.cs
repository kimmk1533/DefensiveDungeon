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

	#region 내부 프로퍼티
	protected DataTableManager M_DataTable => DataTableManager.Instance;
	protected UserInfoManager M_UserInfo => UserInfoManager.Instance;
	#endregion

	#region 외부 프로퍼티
	public Devil Devil => m_Devil;
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

		BoxCollider collider = node.gameObject.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		collider.size.Set(6f, 6f, 6f);
		collider.center.Set(0f, 3f, 0f);

		GameObject devil = GameObject.Instantiate(m_PrefabData.GetPrefab(data.Prefab));
		devil.transform.SetParent(node.transform);
		devil.transform.position = Vector3.zero;
		devil.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		devil.transform.Find("Mesh").localScale = Vector3.one * m_PrefabData.DataList[data.No - 1].Size;

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
	#endregion
}
