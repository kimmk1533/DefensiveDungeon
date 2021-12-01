using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



// synergy manager 로부터 이벤트를 받아 실제 UI 를 띄움
public class SynergyLineSlot : MonoBehaviour
{
	[System.Serializable]
	private class SynergyData : System.IEquatable<SynergyData>
	{
		public int Code;
		public Synergy_TableExcel cur_data;
		public List<Synergy_TableExcel> data_list;
		public bool IsActivated;
		public bool Equals(SynergyData other)
		{
			return Code == other.Code;
		}

	}

	// 현재 슬롯이 표현할 라인 위치
	E_Direction m_dir;

	[SerializeField] Synergy_TableExcelLoader m_synergy_loader;
	[Space(20)]
	[SerializeField] TMPro.TextMeshProUGUI m_lineTextpro;
	[Space(20)]
	[SerializeField] SynergySlot m_slot_origin;
	[SerializeField] List<SynergySlot> m_slot_list; // 관리할 하위 슬롯들
	[SerializeField] List<SynergySlotInfo> m_slot_info_list; // 슬롯에 들어갈 데이터

	[Space(20)]
	[SerializeField] RectTransform m_panel;             // 메인 슬롯
	private List<SynergySlot> m_main_slot_list;

	[Space(20)]
	[SerializeField] int m_showCount;           // m_panel 보여줄 슬롯 개수 (나머지는 추가 버튼으로 확인)
	[SerializeField] int m_showMaxCount;

	[Space(20)]
	[SerializeField] List<SynergyData> m_synergy_list;

	[Space(30)]
	[SerializeField] Vector2 m_extend_root_pos;

	SynergyUIManager M_SynergyUI => SynergyUIManager.Instance;

	private void Awake()
	{
		m_main_slot_list = new List<SynergySlot>();

		if (m_showCount <= 0)
			m_showCount = 3;
		if (m_showMaxCount <= 0)
			m_showMaxCount = 10;

		__Initialize();
	}
	void Start()
	{
		// link event               
		SynergyManager.Instance.OnUpdateSynergyEndEvent += __OnSynergyUpdated;
	}


	void __Initialize()
	{
		m_slot_origin.gameObject.SetActive(false);

		m_synergy_list = new List<SynergyData>();
		m_slot_info_list = new List<SynergySlotInfo>();

		foreach (var item in m_synergy_loader.DataList)
		{
			var data = m_synergy_list.Find((inner) => { return item.Code == inner.Code; });
			if (data == null)
			{   // new code
				data = new SynergyData()
				{
					Code = item.Code,
					cur_data = item,
					data_list = new List<Synergy_TableExcel>(),
					IsActivated = false
				};
				m_synergy_list.Add(data);
			}
			data.data_list.Add(item);
		}
		int total_synergy_count = m_synergy_list.Count;

		for (int i = 0; i < total_synergy_count; ++i)
		{
			var data = m_synergy_list[i].data_list[0];  // rank 1

			m_slot_info_list.Add(new SynergySlotInfo
			{
				isActivated = false,
				name = data.Name_KR,
				synergy_text = data.Synergy_text,
				synergy_ability = data.Synergy_Avility,
				sprite_code = data.Synergy_icon
			});
		}

		// create slots as much as total count
		for (int i = 0; i < m_showCount; i++)
		{
			SynergySlot newSlot = GameObject.Instantiate<SynergySlot>(m_slot_origin);
			m_slot_list.Add(newSlot);
			newSlot.gameObject.SetActive(true);
			m_main_slot_list.Add(m_slot_list[i]);
			m_slot_list[i].transform.SetParent(m_panel);

			m_slot_list[i].SetInfo(m_slot_info_list[i]);
		}

		// 모든 시너지 정보를 slot 에 셋팅
		// 모두 비활성화 상태
		//for (int i = 0; i < total_synergy_count; i++)
		//{
		//    var data = m_synergy_list[i].data_list[0];  // rank 1

		//    m_slot_list[i].SetInfo(new SynergySlotInfo
		//    {
		//        isActivated = false,
		//        name = data.Name_KR,
		//        synergy_text = data.Synergy_text,
		//        synergy_ability = data.Synergy_Avility,
		//        sprite_code = data.Synergy_icon
		//    });
		//}
		UIUpdate();
	}
	public void __Indexing(int index)
	{
		m_dir = (E_Direction)index;
		m_lineTextpro.text = m_dir.ToString().ToUpper();
	}

	// synergy 정보가 업데이트 된 경우
	public void __OnSynergyUpdated()
	{
		// get activated synergy from manager
		var synergy_list = SynergyManager.Instance.GetSynergyList(m_dir);

		// data flush
		for (int i = 0; i < m_synergy_list.Count; i++)
		{
			var tmp_data = m_synergy_list[i];
			tmp_data.IsActivated = false;
			m_synergy_list[i] = tmp_data;
		}

		// current in synergy list's data update
		foreach (var item in synergy_list)
		{
			int index = m_synergy_list.FindIndex((inner) => { return item.Code == inner.Code; });


			// struct
			var tmp_data = m_synergy_list[index];
			tmp_data.cur_data =
				m_synergy_list[index].data_list.Find((inner_2) => { return inner_2.Rank == item.Rank; });
			tmp_data.IsActivated = true;
			m_synergy_list[index] = tmp_data;
		}

		// sorting
		//시너지 랭크 내림차순 ※ (높은 숫자 먼저)
		//시너지의 인원 수 / 적용 x
		//시너지 코드 오름차순 ※ (낮은 숫자 먼저)
		m_synergy_list = m_synergy_list.
			OrderBy(item => item.Code).
			OrderByDescending((item) => item.cur_data.Rank).
			OrderByDescending(item => item.IsActivated).
			ToList();
		//var query = from data in m_synergy_list
		//            where synergy_list.Find((item) => { return item.Code == data.Code; }).Code != 0 // not default
		//            orderby data.cur_data.Rank descending
		//            orderby data.cur_data.Code
		//            select ;                


		// sort complete

		// ui update
		UIUpdate();
	}

	void UIUpdate()
	{
		for (int i = 0; i < m_synergy_list.Count; ++i)
		{
			var cur_data = m_synergy_list[i];

			m_slot_info_list[i] = new SynergySlotInfo()
			{
				isActivated = cur_data.IsActivated,
				name = cur_data.cur_data.Name_KR,
				synergy_text = cur_data.cur_data.Synergy_text,
				synergy_ability = cur_data.cur_data.Synergy_Avility,
				sprite_code = cur_data.cur_data.Synergy_icon
			};
		}

		// synergy ui update
		for (int i = 0; i < m_showCount; i++)
		{
			m_slot_list[i].SetInfo(m_slot_info_list[i]);
		}
	}

	// 확장 버튼을 클릭 했을 경우
	public void __OnExtendButtonClicked()
	{
		UIUpdate();
		SetExtendPanelInfo();

		if (m_dir != M_SynergyUI.LastLineDir)
		{
			M_SynergyUI.extendPanel.gameObject.SetActive(true);
		}
		else
		{
			M_SynergyUI.extendPanel.gameObject.SetActive(!M_SynergyUI.IsShowExtendPanel);
		}

		M_SynergyUI.LastLineDir = m_dir;
	}

	public void SetExtendPanelInfo()
	{
		M_SynergyUI.extendPanel.SetSlots(m_slot_info_list.GetRange(m_showCount, m_slot_info_list.Count - m_showCount));
	}
}
