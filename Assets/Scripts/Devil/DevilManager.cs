using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum E_Devil
{
    None,

    HateQueen,
    HellLord,
    FrostLich,
}

public class DevilManager : Singleton<DevilManager>
{
    protected Tower_TableExcelLoader m_TowerData;
    protected Prefab_TableExcelLoader m_PrefabData;

    protected Devil m_Devil;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected UserInfoManager M_UserInfo => UserInfoManager.Instance;
    #endregion

    #region 외부 프로퍼티
    public Devil Devil => m_Devil;
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public void SelectDevil(E_Devil no)
    {
        Tower_TableExcel data = GetData(no);

        Node node = (new GameObject("Devil Node")).AddComponent<Node>();
        node.transform.SetParent(transform);
        node.transform.position = Vector3.zero;
        node.m_NodeType = E_NodeType.None;
        node.m_Direction = E_Direction.None;
        node.gameObject.layer = LayerMask.NameToLayer("Node");

        BoxCollider collider = node.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size.Set(6f, 6f, 6f);
        collider.center.Set(0f, 3f, 0f);

        GameObject devil = GameObject.Instantiate(m_PrefabData.GetPrefab(data.Prefab));
        devil.transform.position = Vector3.zero;
        devil.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        devil.transform.localScale = Vector3.one * m_PrefabData.DataList[data.No - 1].Size;
        devil.transform.SetParent(transform);

        switch (no)
        {
            case E_Devil.HateQueen:
                m_Devil = devil.AddComponent<HateQueen>();
                break;
            case E_Devil.HellLord:
                m_Devil = devil.AddComponent<HellLord>();
                break;
            case E_Devil.FrostLich:
                m_Devil = devil.AddComponent<FrostLich>();
                break;
        }
    }
    public void SelectDevil(int code)
    {
        Tower_TableExcel data = GetData(code);

        Node node = (new GameObject("Devil Node")).AddComponent<Node>();
        node.transform.SetParent(transform);
        node.transform.position = Vector3.zero;
        node.m_NodeType = E_NodeType.None;
        node.m_Direction = E_Direction.None;
        node.gameObject.layer = LayerMask.NameToLayer("Node");

        BoxCollider collider = node.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = Vector3.one * 6f;
        collider.center.Set(0f, 3f, 0f);

        GameObject devil = GameObject.Instantiate(m_PrefabData.GetPrefab(data.Prefab));
        devil.transform.SetParent(node.transform);
        devil.transform.position = Vector3.zero;
        devil.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        devil.transform.localScale = Vector3.one * m_PrefabData.DataList[data.No - 1].Size;

        switch ((E_Devil)data.No)
        {
            case E_Devil.HateQueen:
                m_Devil = devil.AddComponent<HateQueen>();
                break;
            case E_Devil.HellLord:
                m_Devil = devil.AddComponent<Devil>();
                break;
            case E_Devil.FrostLich:
                m_Devil = devil.AddComponent<Devil>();
                break;
        }

        node.SetDevil(m_Devil);
    }
    public Tower_TableExcel GetData(E_Devil no)
    {
        Tower_TableExcel result = m_TowerData.DataList.Where(item => item.No == (int)no).SingleOrDefault();

        return result;
    }
    public Tower_TableExcel GetData(int code)
    {
        Tower_TableExcel result = m_TowerData.DataList.Where(item => item.Code == code).SingleOrDefault();

        return result;
    }
    public float GetSize(E_Devil no)
    {
        return m_PrefabData.DataList[(int)no - 1].Size;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        m_TowerData = M_DataTable.GetDataTable<Tower_TableExcelLoader>();
        m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
    }

    private void Start()
    {
        SelectDevil(M_UserInfo.DevilCode);
    }
    #endregion
}
