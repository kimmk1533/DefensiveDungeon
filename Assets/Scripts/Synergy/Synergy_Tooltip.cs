using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Synergy_Tooltip : Singleton<Synergy_Tooltip>
{
    [SerializeField] public TextMeshProUGUI Synergy_name;
    [SerializeField] public TextMeshProUGUI Synergy_Explane;
    [SerializeField] public TextMeshProUGUI Synergy_Avility;
    private Image Synergy_Icon;
    private RectTransform Synergy_Tooltip_pos;
    private void Awake()
    {
        Synergy_Tooltip_pos = this.gameObject.GetComponent<RectTransform>();
        //Synergy_Icon = this.gameObject.GetComponent<Image>();
        Transform temp = this.transform.Find("Synergy_name/Synergyicon"); //.Find("Synergyicon");
        Synergy_Icon = temp.gameObject.GetComponent<Image>();

    }
    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void Set_Tooltip_Pos(Vector2 pos)
    {
        Vector2 pos2 = new Vector2(pos.x + 150, pos.y - 200);
        Synergy_Tooltip_pos.position = pos2;
    }
    public void Set_Tooltip(string name, string explane, string avility, Image icon)
    {
        Synergy_name.text = name;
        Synergy_Explane.text = explane;
        Synergy_Avility.text = avility;
        Synergy_Icon.sprite = icon.sprite;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
