using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerToolTipManager : Singleton<TowerToolTipManager>
{
    [SerializeField] TowerToolTipUIController m_tooltip;

    bool m_worldFlag;   // summoned tower into world(game)
    Tower m_tower;      // for world

    bool m_invenFlag;        // before summoned tower on inventorySlot
    InventorySlotGUI m_slot; // for ui

    private void Start()
    {
        DeActivateTooltip();
    }

    // TODO : tooltip position setting algorythm
    // TODO : param tower can changed ( Node or tower manager / what you want like towermanager )
    public void ActivateToolTip(Vector3 worldPos, Tower tower, Tower_TableExcel data)
    {
        m_worldFlag = true;

        m_tower = tower;
        m_tooltip.SetUIInfo(data);
        Vector3 mousepos = Camera.main.WorldToScreenPoint(worldPos);
        m_tooltip.SetPosition(mousepos);

        m_tooltip.gameObject.SetActive(true);
    }
    public void ActivateToolTipOnUIClick(Vector2 mousePos, InventorySlotGUI slot, Tower_TableExcel data)
    {
        m_invenFlag = true;

        m_slot = slot;
        m_tooltip.SetUIInfo(data);
        m_tooltip.SetPosition(mousePos);

        m_tooltip.gameObject.SetActive(true);
    }
    public void DeActivateTooltip()
    {
        m_worldFlag = false;
        m_tower = null;
        m_invenFlag = false;
        m_slot = null;
        m_tooltip.gameObject.SetActive(false);
    }

    public void FlushInvenSlot()
    {   // UI
        m_slot.ClearInven();
    }
    public void FlushNode()
    {   // World
        TowerManager.Instance.DespawnTower(m_tower);
    }

    // called by TooltipSalePrice class ( and TooltipSalePrice function called by Button Event )
    public void __OnSaleButtonClicked(int price)
    {
        if (m_invenFlag)
            FlushInvenSlot();
        if (m_worldFlag)
            FlushNode();
        UserInfoManager.Instance.AddGold(price);

        DeActivateTooltip();
    }
}
