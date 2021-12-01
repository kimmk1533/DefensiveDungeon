using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SynergyExtensionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SynergyUIManager M_SynergyUI => SynergyUIManager.Instance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        M_SynergyUI.ChangeExtendActive = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        M_SynergyUI.ChangeExtendActive = false;
    }
}
