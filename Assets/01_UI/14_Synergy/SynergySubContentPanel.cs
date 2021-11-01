using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SynergySubContentPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool OnThisPanel = false;  // mouse

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnThisPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnThisPanel = false;
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
