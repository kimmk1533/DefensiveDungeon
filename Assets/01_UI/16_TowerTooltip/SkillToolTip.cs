using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : MonoBehaviour
{
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Image m_skillImage;
    [SerializeField] TMPro.TextMeshProUGUI m_nameTextpro;
    [SerializeField] TMPro.TextMeshProUGUI m_skillTextpro;
    [SerializeField] TMPro.TextMeshProUGUI m_skillAbilityName1Textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_skillAbilityName2Textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_skillAbility1Textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_skillAbility2Textpro;

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        this.gameObject.SetActive(false);
    }

    public void SetUI(SkillCondition_TableExcel data)
    {
        m_skillImage.sprite = m_spriteLoader.GetSprite(data.Skill_icon);
        m_nameTextpro.text = data.Name_KR;
        m_skillTextpro.text = data.Skill_text;
        m_skillAbilityName1Textpro.text = data.SkillAvility1_Name;
        m_skillAbilityName2Textpro.text = data.SkillAvility2_Name;
        m_skillAbility1Textpro.text = data.SkillAvility1_Text;
        m_skillAbility2Textpro.text = data.SkillAvility2_Text;
    }
}
