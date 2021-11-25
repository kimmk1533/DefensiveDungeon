using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SynergySubContentPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SynergySlot m_slot_origin;
    [SerializeField] GridLayoutGroup m_contents_panel;
    private List<SynergySlot> m_extend_slot_list;
    [SerializeField] bool OnThisPanel = false;  // mouse

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnThisPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnThisPanel = false;
    }

    public void SetSlots(List<SynergySlotInfo> slotList)
    {
        while (m_extend_slot_list.Count < slotList.Count)
        {
            SynergySlot newSlot = GameObject.Instantiate<SynergySlot>(m_slot_origin);
            m_extend_slot_list.Add(newSlot);
            newSlot.transform.SetParent(m_contents_panel.transform);
            newSlot.gameObject.SetActive(true);
        }

        foreach (var item in m_extend_slot_list)
        {
            item.gameObject.SetActive(true);
        }

        for (int i = slotList.Count; i < m_extend_slot_list.Count; ++i)
        {
            m_extend_slot_list[i].gameObject.SetActive(false);
        }

        for (int i =  0; i < m_extend_slot_list.Count; ++i)
        {
            m_extend_slot_list[i].SetInfo(slotList[i]);
        }
    }

    private void Awake()
    {
        m_slot_origin.gameObject.SetActive(false);

        m_extend_slot_list = new List<SynergySlot>();
    }

    private void Update()
    {
        //
        // check click position is out of this panel
        // 
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (OnThisPanel)
                return;

            // out of panel
            SynergyUIManager.Instance.DeActivateAllExtendSynergyPanel();
        }
    }


}
