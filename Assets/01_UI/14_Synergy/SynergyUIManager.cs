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
    [SerializeField] SynergySubContentPanel m_extendPanel;

    OptionManager M_Option => OptionManager.Instance;

    public SynergySubContentPanel extendPanel => m_extendPanel;
    public bool IsShowExtendPanel => m_extendPanel.gameObject.activeSelf;
    public bool ChangeExtendActive { get; set; }
    public E_Direction LastLineDir { get; set; }

    private void Start()
    {
        __Initialize();        
    }

	private void Update()
	{
		if (Input.GetKeyDown(M_Option.GetKeyCode(KeyOptionType.Synerge_North)))
		{
            m_lineSlots[0].__OnExtendButtonClicked();
        }
        if (Input.GetKeyDown(M_Option.GetKeyCode(KeyOptionType.Synerge_East)))
        {
            m_lineSlots[1].__OnExtendButtonClicked();
        }
        if (Input.GetKeyDown(M_Option.GetKeyCode(KeyOptionType.Synerge_South)))
        {
            m_lineSlots[2].__OnExtendButtonClicked();
        }
        if (Input.GetKeyDown(M_Option.GetKeyCode(KeyOptionType.Synerge_West)))
        {
            m_lineSlots[3].__OnExtendButtonClicked();
        }
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

        LastLineDir = E_Direction.None;
    }
}
