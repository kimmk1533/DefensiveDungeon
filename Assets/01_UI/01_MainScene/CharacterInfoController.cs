using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_nameTextpro;

    [SerializeField] List<TMPro.TextMeshProUGUI> m_infoTextproList;

    public void Set(string name, string info)
    {
        m_nameTextpro.text = name;
        //m_infoTextpro.text = info;
    }


    public void __OnButtonClicked(int index)
    {
        foreach (var item in m_infoTextproList)
        {
            item.gameObject.SetActive(false);
        }

        m_infoTextproList[index].gameObject.SetActive(true);
    }
}
