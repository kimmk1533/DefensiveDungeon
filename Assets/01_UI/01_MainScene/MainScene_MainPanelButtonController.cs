using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene_MainPanelButtonController : MonoBehaviour
{
    [SerializeField] Image m_character_panel;    
    [Space(10)]
    [SerializeField] Image m_option_panel;    

    public void __OnStartButton(Button button)
    {
        m_character_panel.gameObject.SetActive(true);
        button.enabled = true;
        this.gameObject.SetActive(false);
        CharacterSelectManager.Instance.OnStart();
    }
    public void __OnExitButton()
    {
        // TODO : Save
        Application.Quit();
    }
}
