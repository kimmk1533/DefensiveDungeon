using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    protected BuffCC_TableExcelLoader m_BuffCCData;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public BuffCC_TableExcel GetData(int code)
    {
        BuffCC_TableExcel result = m_BuffCCData.DataList.Where(item => item.Code == code).SingleOrDefault();

        return result;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        m_BuffCCData = M_DataTable.GetDataTable<BuffCC_TableExcelLoader>();
    }
    #endregion
}

public enum E_BuffType
{
    None,

    Atk,
    Range,
    Def,
    Atk_spd,
    Move_spd,
    Crit_rate,
    Crit_Dmg,

    Stun,
    Dot_Dmg,
    Insta_Kill,
    CritDmg_less,
    CritDmg_more,

    Heal,
    Summon,
    Shield
}

public enum E_AddType
{
    None,

    Fix,
    Percent
}

[System.Serializable]
public struct S_Buff
{
    public E_BuffType BuffType;
    public E_AddType AddType;
    public float BuffAmount;
    public float BuffRand;
    public int Summon;

    public S_Buff(int buffType, int addType, float buffAmount, float buffRand, int summon)
    {
        BuffType = (E_BuffType)buffType;
        AddType = (E_AddType)addType;
        BuffAmount = buffAmount;
        BuffRand = buffRand;
        Summon = summon;
    }
}