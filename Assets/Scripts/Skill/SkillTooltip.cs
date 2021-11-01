using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTooltip : Singleton<SkillTooltip>
{
    public TextMeshProUGUI Skill_name;
    public TextMeshProUGUI Skill_text;
    public TextMeshProUGUI Skill_avility_name1;
    public TextMeshProUGUI Skill_avility_name2;
    public TextMeshProUGUI Skill_avility_text1;
    public TextMeshProUGUI Skill_avility_text2;

    private RectTransform Skill_TT_pos;
    private Transform Avilityname;
    private Transform Avilitytext;
    float screen_maxy;
    float screen_maxx;
    private void Awake()
    {
        Skill_TT_pos = this.gameObject.GetComponent<RectTransform>();
        Avilityname = this.transform.Find("Skill_avility_name2");
        Avilitytext = this.transform.Find("Skill_avility_text2");
        Avilityname.gameObject.SetActive(false);
        Avilitytext.gameObject.SetActive(false);
        screen_maxy = Screen.height;
        screen_maxx = Screen.width;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }
    //public void Set_Skill_TT(Tower_Data tower)
    //{
        
    //    Skill_name.text = tower.skillname;
    //    Skill_text.text = tower.skilltext;
    //    //�����Ƽ 2 ���°�� ��Ȱ��ȭ.
    //    //if(tower.isskillavility)
    //    //{
    //    //    Avilityname.gameObject.SetActive(false);
    //    //    Avilitytext.gameObject.SetActive(false);
    //    //}
    //    //else
    //    //{
    //    //    Avilityname.gameObject.SetActive(true);
    //    //    Avilitytext.gameObject.SetActive(true);
    //    //}
    //    //Skill_avility_name1=tower.skillavility1;
    //    //Skill_avility_text1=tower.skillavility_text1;

    //}
    public void Set_Skill_TT_pos(Vector2 pos)
    {
        float tempposx = pos.x;
        float tempposy = pos.y;

        if (pos.x + Skill_TT_pos.rect.width > screen_maxx)
        {
            tempposx -= Skill_TT_pos.rect.width;
        }
        if (pos.y - Skill_TT_pos.rect.height < 0)
        {
            tempposy += Skill_TT_pos.rect.height;
        }
        Vector2 pos2 = new Vector2(tempposx, tempposy);

        Skill_TT_pos.position = pos2;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
