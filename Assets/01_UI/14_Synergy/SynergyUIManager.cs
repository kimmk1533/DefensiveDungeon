using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class SynergyUIManager : Singleton<SynergyUIManager>
{
    [SerializeField] GridLayoutGroup m_root_panel;

    [SerializeField] SynergyLineSlot m_origin;
    [SerializeField] List<SynergyLineSlot> m_lineSlots;

    private void Start()
    {
        __Initialize();        
    }

    void __Initialize()
    {
        // synergy manager 에서 이벤트로 연결할것

        int cellcount = m_root_panel.constraintCount;

        m_origin.gameObject.SetActive(false);

        for (int i = 0; i < cellcount; i++)
        {
            var newSlot = GameObject.Instantiate<SynergyLineSlot>(m_origin);

            newSlot.__Indexing(i);

            newSlot.gameObject.SetActive(true);
            newSlot.transform.SetParent(m_root_panel.transform);
            m_lineSlots.Add(newSlot);
        }
    }

    public void DeActivateAllExtendSynergyPanel()
    {
        foreach (var item in m_lineSlots)
        {
            item.DeActivateExtendPanel();
        }
    }
}
