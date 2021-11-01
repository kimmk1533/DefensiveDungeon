using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevilToolTipUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool OnThisPanel = false;

    [Space(20)]
    [SerializeField] TMPro.TextMeshProUGUI m_name_textpro;
    [Space(10)]
    [SerializeField] DevilToolTipImage m_image;
    [Space(10)]
    [SerializeField] TMPro.TextMeshProUGUI m_text_textpro;

    [Space(10)]
    [SerializeField] DevilToolTipDataLine m_stat_data1;
    [SerializeField] DevilToolTipDataLine m_stat_data2;
    [SerializeField] DevilToolTipDataLine m_stat_data3;
    [SerializeField] DevilToolTipDataLine m_stat_data4;
    [SerializeField] DevilToolTipDataLine m_stat_data5;
    [SerializeField] DevilToolTipDataLine m_stat_data6;
    [Space(10)]
    [SerializeField] DevilToolTipSkill m_skill_1;
    [SerializeField] DevilToolTipSkill m_skill_2;


    [Space(20)]
    [SerializeField] SkillCondition_TableExcelLoader m_skill_loader; // for skill info overall
    [SerializeField] SkillStat_TableExcelLoader m_skillstat_loader; // for atk speed

    public void SetUIInfo(Tower_TableExcel data)
    {
        // name
        m_name_textpro.text = data.Name_KR;

        // raw image (render texture)
        m_image.SetUI(data.Code);


        // tower data (atk critical etc)
        // -atk
        m_stat_data1.SetUI("공격력", data.Atk.ToString());
        // -atk speed
        float atk_speed = m_skillstat_loader.DataList.Find((item) => { return item.Code == data.Atk_Code; }).CoolTime;
        float calc_atk_speed = (atk_speed == 0) ? 1 : ((int)(1f / atk_speed * 100));
        m_stat_data2.SetUI(
            "공격속도",
            calc_atk_speed.ToString() + "%");
        Debug.Log($"atk speed : {atk_speed}");
        Debug.Log($"caculated atk speed : {(int)(1f / atk_speed * 100)}");
        // -critical rate
        m_stat_data3.SetUI("치명타 확률", (data.Crit_rate * 100).ToString() + "%");
        // -critical damage
        m_stat_data4.SetUI("치명타 피해", (data.Crit_Dmg * 100).ToString() + "%");
        m_stat_data5.SetUI("체력", (data.HP * 100).ToString());
        m_stat_data6.SetUI("방어력", (data.Def * 100).ToString());

        // skill
        var skill_data1 = m_skill_loader.DataList.Find((item) =>
        {
            return item.Code == data.Skill1Code;
        });
        var skill_data2 = m_skill_loader.DataList.Find((item) =>
        {
            return item.Code == data.Skill2Code;
        });

        m_skill_1.SetUI(skill_data1);
        m_skill_2.SetUI(skill_data2);
    }

    public void SetPosition(Vector2 mousePos)
    {
        var m_rt = this.GetComponent<RectTransform>();
        float tooltip_widht = m_rt.rect.width;
        float tooltip_height = m_rt.rect.height;

        this.transform.position =
            mousePos + new Vector2(tooltip_widht * 0.5f, -tooltip_height * 0.5f);
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
            DevilToolTipManager.Instance.DeActivateTooltip();
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

