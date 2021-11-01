using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipStar : MonoBehaviour
{   // auto position (grid layout)
    [SerializeField] List<Image> m_stars_list;
     
    public void SetUI(int star_count)
    {
        for (int i = 0; i < star_count; i++)
        {
            m_stars_list[i].gameObject.SetActive(true);
        }
        for (int i = star_count; i < m_stars_list.Count; i++)
        {
            m_stars_list[i].gameObject.SetActive(false);
        }
    }
}
