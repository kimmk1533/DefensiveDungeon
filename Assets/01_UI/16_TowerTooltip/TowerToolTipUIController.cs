using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerToolTipUIController : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool OnThisPanel = false;

    [Space(20)]
    [SerializeField] TMPro.TextMeshProUGUI m_name_textpro;
    [Space(10)]
    [SerializeField] ToolTipTowerImage m_image;
    [Space(10)]
    [SerializeField] ToolTipLeftSynergy m_synergyline1;
    [SerializeField] ToolTipLeftSynergy m_synergyline2;
    [Space(10)]
    [SerializeField] ToolTipRightDataLine m_stat_data1;
    [SerializeField] ToolTipRightDataLine m_stat_data2;
    [SerializeField] ToolTipRightDataLine m_stat_data3;
    [SerializeField] ToolTipRightDataLine m_stat_data4;
    [Space(10)]
    [SerializeField] ToolTipRightSkill m_skill;
    [Space(10)]
    [SerializeField] ToolTipStar m_star;
    [Space(10)]
    [SerializeField] ToolTipSalePrice m_saleprice;

    [Space(20)]
    [SerializeField] Synergy_TableExcelLoader m_synergyLoader;  // for synergy info
    [SerializeField] SkillCondition_TableExcelLoader m_skill_loader; // for skill info overall
    [SerializeField] SkillStat_TableExcelLoader m_skillstat_loader; // for atk speed

    public void SetUIInfo(Tower_TableExcel data)
    {
        // name
        m_name_textpro.text = data.Name_KR;
        // raw image (render texture)
        m_image.SetUI(data.Code);

        // synergy
        int synergy1Code = data.Type1;
        int synergy2Code = data.Type2;

        var synergy1_data = m_synergyLoader.DataList.
            Find((item) => { return item.Code == synergy1Code; }); 
        var synergy2_data = m_synergyLoader.DataList.
            Find((item) => { return item.Code == synergy2Code; });
        m_synergyline1.SetUI(synergy1_data.Synergy_icon, synergy1_data.Name_KR);
        m_synergyline2.SetUI(synergy2_data.Synergy_icon, synergy2_data.Name_KR);


        // tower data (atk critical etc)
        // -atk
        m_stat_data1.SetUI("공격력", data.Atk.ToString());
        // -atk speed
        float atk_speed = m_skillstat_loader.DataList.Find((item) => { return item.Code == data.Atk_Code; }).CoolTime;
        float calc_atk_speed = (atk_speed == 0) ? 100 : ((int)(1f / atk_speed * 100));
        m_stat_data2.SetUI(
            "공격속도",
            calc_atk_speed.ToString() + "%");
        Debug.Log($"atk speed : {atk_speed}");
        Debug.Log($"caculated atk speed : {(int)(1f / atk_speed * 100)}");
        // -critical rate
        m_stat_data3.SetUI("치명타 확률", (data.Crit_rate * 100).ToString() + "%");
        // -critical damage
        m_stat_data4.SetUI("치명타 피해", (data.Crit_Dmg * 100).ToString() + "%");

        // skill
        var skill_data = m_skill_loader.DataList.Find((item) =>
        {
            return item.Code == data.Skill1Code;
        });

        m_skill.SetUI(skill_data);

        m_star.SetUI(data.Star);

        // 타워 구매 비용 * 3^(Star(별) -1)/1.5}
        float sale_price = 0.0f;
        if (data.Star == 1)
            sale_price = data.Price;
        else
            sale_price = data.Price * 3 * (data.Star - 1) / 1.5f;
        m_saleprice.SetUI((int)sale_price);
    }

    enum ETooltipDirection
    {
        Default,
        UpperLeft,
        UpperRight,
        BottomLeft,
        BottomRight,
    }

    private ETooltipDirection GetDirection(Vector2 mousePos)
    {
        ETooltipDirection dir = ETooltipDirection.Default;

        int screen_width = Screen.width;
        int screen_height = Screen.height;

        var m_rt = this.GetComponent<RectTransform>();
        float tooltip_widht = m_rt.rect.width;
        float tooltip_height = m_rt.rect.height;

       
        bool left = false;
        if(mousePos.x < tooltip_widht)
        {   // right            
            left = false;
        }
        else if(mousePos.x > screen_width - tooltip_widht)
        {   // left           
            left = true;
        }
        if( mousePos.y < tooltip_widht)
        {   // upper
            if (left)
                dir = ETooltipDirection.UpperLeft;
            else
                dir = ETooltipDirection.UpperRight;
        }
        else if(mousePos.y > screen_height)
        {   // bottom
            if (left)
                dir = ETooltipDirection.BottomLeft;
            else
                dir = ETooltipDirection.BottomRight;
        }

        return dir;
    }

    public void SetPosition(Vector2 mousePos)
    {
        var m_rt = this.GetComponent<RectTransform>();
        float tooltip_widht = m_rt.rect.width;
        float tooltip_height = m_rt.rect.height;

        var direction = GetDirection(mousePos);
        switch (direction)
        {
            case ETooltipDirection.UpperLeft:
                this.transform.position = mousePos
                    + new Vector2(-tooltip_widht * 0.5f, tooltip_height * 0.5f); ;
                break;
            case ETooltipDirection.UpperRight:
                this.transform.position = mousePos
                    + new Vector2(tooltip_widht * 0.5f, tooltip_height * 0.5f); ;
                break;
            case ETooltipDirection.Default:
            case ETooltipDirection.BottomLeft:
                this.transform.position = mousePos
                    + new Vector2(-tooltip_widht * 0.5f, -tooltip_height * 0.5f); ;
                break;
            case ETooltipDirection.BottomRight:
                this.transform.position = mousePos
                    + new Vector2(tooltip_widht * 0.5f, -tooltip_height * 0.5f); ;
                break;                
        }        
    }

    //////////////////////////////////     

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
            TowerToolTipManager.Instance.DeActivateTooltip();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnThisPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnThisPanel = false;
    }
}
