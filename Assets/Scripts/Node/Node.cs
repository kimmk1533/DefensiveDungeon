using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [ReadOnly]
    public E_NodeType m_NodeType;
    [ReadOnly]
    public E_Direction m_Direction;
    [ReadOnly(true)]
    public Tower m_Tower;
    [ReadOnly(true)]
    public Devil m_Devil;
    [SerializeField, ReadOnly]
    protected GameObject m_Outline;

    #region 내부 프로퍼티
    protected NodeManager M_Node => NodeManager.Instance;
    protected TowerManager M_Tower => TowerManager.Instance;
    protected SynergyManager M_Synergy => SynergyManager.Instance;
    #endregion

    #region 외부 프로퍼티
    public GameObject Outline => m_Outline;
    #endregion

    public void SetTower(Tower tower)
    {
        m_Tower = tower;
        m_Tower.Node = this;

        if (E_NodeType.None != m_NodeType)
        {
            m_Tower.transform.SetParent(transform);
            m_Tower.gameObject.SetActive(true);
            m_Tower.transform.localPosition = new Vector3(0f, 0.501f, 0f);
            m_Tower.transform.localEulerAngles = Vector3.zero;
            m_Tower.m_TowerInfo.Direction = m_Direction;
            m_Tower.m_TowerInfo.LookingDir = m_Tower.transform.forward;

            m_Tower.gameObject.SetActive(true);
            m_Tower.m_TowerInfo.IsOnInventory = false;
            m_Tower.m_TowerInfo.CanAttack = true;

            M_Tower.AddTower(tower, m_Tower.m_TowerInfo.Direction);
            M_Synergy.UpdateSynergy();
        }
    }
    public void SetDevil(Devil devil)
    {
        m_Devil = devil;
    }

    public void ClearNode()
    {
        m_Tower.Node = null;
        m_Tower = null;
    }

    public void Initialize()
    {
        m_Outline = transform.Find("Outline").gameObject;
        //m_Outline.SetActive(false);
    }
}