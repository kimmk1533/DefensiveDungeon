using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilToolTipManager : Singleton<DevilToolTipManager>
{
    [SerializeField] DevilToolTipUIController m_tooltip;

    Devil m_devil;   

    private void Start()
    {
        DeActivateTooltip();
        m_devil = DevilManager.Instance.Devil;
    }

    public void ActivateToolTip(Vector3 worldPos)
    {               
        m_tooltip.SetUIInfo(m_devil.ExcelData);
        Vector3 mousepos = Camera.main.WorldToScreenPoint(worldPos);
        m_tooltip.SetPosition(mousepos);

        m_tooltip.gameObject.SetActive(true);
    }
  
    public void DeActivateTooltip()
    {        
        m_tooltip.gameObject.SetActive(false);
    }  
   
}
