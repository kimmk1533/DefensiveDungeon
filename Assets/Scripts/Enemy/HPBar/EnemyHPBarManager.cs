using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarManager : Singleton<EnemyHPBarManager>
{
    public Canvas m_HPBarCanvas;
    [SerializeField]
    protected Vector3 m_Distance = Vector3.up * 50f;
    protected const string key = "EnemyHPBar";

    #region 내부 컴포넌트
    protected EnemyHPBarPool M_HPBarPool => EnemyHPBarPool.Instance;
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    public Vector3 Distance => m_Distance;
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public EnemyHPBar SpawnHPBar()
    {
        EnemyHPBar hpBar = M_HPBarPool.GetPool(key).Spawn();
        hpBar.Initialize();
        return hpBar;
    }
    public void DespawnHPBar(EnemyHPBar hpBar)
    {
        M_HPBarPool.GetPool(key).DeSpawn(hpBar);
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        
    }

    void Update()
    {
        
    }
    #endregion
}
