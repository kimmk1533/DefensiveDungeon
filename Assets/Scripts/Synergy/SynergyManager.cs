using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager : Singleton<SynergyManager>
{
    public delegate void SynergyEventHandler();
    public event SynergyEventHandler UpdateSynergyEndEvent;

    protected Synergy_TableExcelLoader m_SynergyData;

    // 시너지 최대 랭크
    [SerializeField]
    protected int m_MaxRank = 3;
    // 골드 추가 시너지에 사용될 추가 골드
    protected int m_BonusGold;
    public int BonusGold => m_BonusGold;

    // 방향별 시너지 리스트
    protected Dictionary<E_Direction, List<Synergy_TableExcel>> m_Synergys = null;
    // 방향별, 시너지 코드별 타워 갯수
    protected Dictionary<E_Direction, Dictionary<int, int>> m_TowerCount = null;

    #region 내부 프로퍼티
    protected TowerManager M_Tower => TowerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected BuffManager M_Buff => BuffManager.Instance;
    protected NodeManager M_Node => NodeManager.Instance;
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected CombinationManager M_Combination => CombinationManager.Instance;
    #endregion

    private void Awake()
    {
        m_SynergyData = M_DataTable.GetDataTable<Synergy_TableExcelLoader>();

        m_Synergys = new Dictionary<E_Direction, List<Synergy_TableExcel>>();
        m_TowerCount = new Dictionary<E_Direction, Dictionary<int, int>>();
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys.Add(i, new List<Synergy_TableExcel>());
            m_TowerCount.Add(i, new Dictionary<int, int>());
        }
    }

    private void Start()
    {
        M_Node.m_RotateEndEvent += UpdateSynergy;
        M_Combination.OnCombinationCompleteEvent += UpdateSynergy_Combination;
    }

    public Synergy_TableExcel GetData(int code, int rank = 1)
    {
        var datas = m_SynergyData.DataList.Where(item => item.Code == code).ToList();
        Synergy_TableExcel synergy = datas.Where(item => item.Rank == rank).SingleOrDefault();

        return synergy;
    }
    public List<Synergy_TableExcel> GetSynergyList(E_Direction dir)
    {
        return m_Synergys[dir];
    }

    public void UpdateSynergy()
    {
        Debug.Log("시너지 업데이트");

        // 시너지 관리 리스트 초기화
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys[i].Clear();
        }

        // 골드 추가
        m_BonusGold = 0;

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            UpdateSynergy(i);
            m_Synergys[i] = m_Synergys[i]
                .OrderByDescending(item => item.Rank)
                //.OrderByDescending(item => m_TowerCount[i][item.Code])
                .OrderBy(item => item.Code)
                .ToList();
        }

        UpdateSynergyEndEvent?.Invoke();
    }
    public void UpdateSynergy_Combination(bool flag)
    {
        if (flag)
        {
            UpdateSynergy();
        }
    }
    protected void LoadSynergyCode(Tower tower, ref Dictionary<int, List<Tower>> SynergyTowers)
    {
        // 중복 체크용
        bool flag;
        // 시너지 코드
        int code1 = tower.SynergyCode1;
        int code2 = tower.SynergyCode2;

        #region 시너지1
        // 첫 체크면 리스트 추가
        if (!SynergyTowers.ContainsKey(code1))
        {
            SynergyTowers[code1] = new List<Tower>();
        }

        // 중복 타워 체크
        flag = false;
        foreach (var item in SynergyTowers[code1])
        {
            if (item.TowerKind == tower.TowerKind)
            {
                flag = true;
                break;
            }
        }

        // 중복 타워 없으면 시너지 리스트에 추가
        if (!flag)
            SynergyTowers[code1].Add(tower);
        #endregion

        #region 시너지2
        // 첫 체크면 리스트 추가
        if (!SynergyTowers.ContainsKey(code2))
        {
            SynergyTowers[code2] = new List<Tower>();
        }

        // 중복 타워 체크
        flag = false;
        foreach (var item in SynergyTowers[code2])
        {
            if (item.TowerKind == tower.TowerKind)
            {
                flag = true;
                break;
            }
        }

        // 중복 타워 없으면 시너지 리스트에 추가
        if (!flag)
            SynergyTowers[code2].Add(tower);
        #endregion
    }
    protected void UpdateSynergy(E_Direction dir)
    {
        List<Tower> towers = M_Tower.GetTowerList(dir);

        if (towers.Count <= 0)
            return;

        // 시너지 초기화
        for (int i = 0; i < towers.Count; ++i)
        {
            // 버프
            towers[i].m_TowerInfo.BuffList.Clear();

            // 공격 타입 변경
            towers[i].m_TowerInfo.Synergy_Atk_type = E_AttackType.None;

            // 버서커
            towers[i].m_TowerInfo.Berserker = false;
            towers[i].m_TowerInfo.BerserkerStack = 0;
            towers[i].m_TowerInfo.BerserkerMaxStack = 0;
            towers[i].m_TowerInfo.BerserkerBuffList.Clear();
        }

        // 시너지 코드, 시너지 적용될 타워들
        Dictionary<int, List<Tower>> SynergyTowers = new Dictionary<int, List<Tower>>();

        // 시너지 코드 로드
        for (int i = 0; i < towers.Count; ++i)
        {
            LoadSynergyCode(towers[i], ref SynergyTowers);
        }

        foreach (var item in SynergyTowers)
        {
            int Rank = m_MaxRank;
            int TowerCount = item.Value.Count;
            m_TowerCount[dir][item.Key] = TowerCount;
            Synergy_TableExcel data;
            S_SynergyEffect effect;
            BuffCC_TableExcel buffData;

            while (true)
            {
                do
                {
                    data = GetData(item.Key, Rank--);
                } while (data.Code == 0 && Rank > 0);

                if (Rank < 0)
                    break;

                if (data.MemReq <= TowerCount)
                {
                    m_Synergys[dir].Add(data);

                    if (data.EffectType1 != 0)
                    {
                        effect = new S_SynergyEffect(
                            data.EffectType1,
                            data.EffectAmount1,
                            data.EffectCode1,
                            data.EffectChange1,
                            data.EffectReq1,
                            data.EffectRand1
                            );
                        buffData = M_Buff.GetData(effect.EffectCode);

                        // 시너지1 적용
                        switch (effect.EffectType)
                        {
                            case E_SynergyEffectType.Buff:
                                {
                                    // 타워 버프
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = M_Tower.GetTowerList(dir);
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.BuffList.Add(buffData);
                                        }
                                    }
                                    // 몬스터 디버프
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Enemy)
                                    {
                                        // 시너지 적용할 적 리스트
                                        List<Enemy> enemyList = null;

                                        // 같은 라인 적
                                        if (data.TargetMem == 1)
                                        {
                                            enemyList = M_Enemy.GetEnemyList(dir);
                                        }
                                        // 전체 라인 적
                                        if (data.TargetMem == 2)
                                        {
                                            enemyList = M_Enemy.GetEnemyList();
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < enemyList.Count; ++i)
                                        {
                                            enemyList[i].BuffList.Add(buffData);
                                        }
                                    }
                                    // 마왕 버프
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.ChangeAtkType:
                                {
                                    // 타워
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                            towerList[i].m_TowerInfo.BounceCount = effect.EffectReq;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.ReduceCooldown:
                                {
                                    // 마왕
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.Berserker:
                                {
                                    // 타워
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Berserker = true;
                                            towerList[i].m_TowerInfo.BerserkerMaxStack = effect.EffectReq;
                                            towerList[i].m_TowerInfo.BerserkerBuffList.Add(buffData);
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.AddGold:
                                {
                                    m_BonusGold += effect.EffectReq;
                                }
                                break;
                        }
                    }
                    if (data.EffectType2 != 0)
                    {
                        effect = new S_SynergyEffect(
                            data.EffectType2,
                            data.EffectAmount2,
                            data.EffectCode2,
                            data.EffectChange2,
                            data.EffectReq2,
                            data.EffectRand2
                            );
                        buffData = M_Buff.GetData(effect.EffectCode);

                        // 시너지2 적용
                        switch (effect.EffectType)
                        {
                            case E_SynergyEffectType.Buff:
                                {
                                    // 타워 버프
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.BuffList.Add(buffData);
                                        }
                                    }
                                    // 몬스터 디버프
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Enemy)
                                    {
                                        // 시너지 적용할 적 리스트
                                        List<Enemy> enemyList = null;

                                        // 같은 라인 적
                                        if (data.TargetMem == 1)
                                        {
                                            enemyList = M_Enemy.GetEnemyList(dir);
                                        }
                                        // 전체 라인 적
                                        if (data.TargetMem == 2)
                                        {
                                            enemyList = M_Enemy.GetEnemyList();
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < enemyList.Count; ++i)
                                        {
                                            enemyList[i].BuffList.Add(buffData);
                                        }
                                    }
                                    // 마왕 버프
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.ChangeAtkType:
                                {
                                    // 타워
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.ReduceCooldown:
                                {
                                    // 타워
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            //towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.Berserker:
                                {
                                    // 타워
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // 시너지 적용할 타워 리스트
                                        List<Tower> towerList = null;

                                        // 같은 시너지 타워들만
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // 현재 라인 타워 전부
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // 시너지 적용
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Berserker = true;
                                            towerList[i].m_TowerInfo.BerserkerMaxStack = effect.EffectReq;
                                            towerList[i].m_TowerInfo.BerserkerBuffList.Add(buffData);
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.AddGold:
                                {
                                    m_BonusGold += effect.EffectReq;
                                }
                                break;
                        }
                    }
                    break;
                }
            }
        }
    }
}

public enum E_SynergyEffectType
{
    None,

    Buff,
    ChangeAtkType,
    ReduceCooldown,
    Berserker,
    AddGold
}
public enum E_SynergyEffectAmount
{
    None,

    Tower,
    Enemy,
    Devil
}

[System.Serializable]
public struct S_SynergyEffect
{
    public E_SynergyEffectType EffectType;
    public E_SynergyEffectAmount EffectAmount;
    public int EffectCode;
    public E_AttackType EffectChange;
    public int EffectReq;
    public float EffectRand;

    public S_SynergyEffect(int effectType, int effectAmount, int effectCode, int effectChange, int effectReq, float effectRand)
    {
        EffectType = (E_SynergyEffectType)effectType;
        EffectAmount = (E_SynergyEffectAmount)effectAmount;
        EffectCode = effectCode;
        EffectChange = (E_AttackType)effectChange;
        EffectReq = effectReq;
        EffectRand = effectRand;
    }
}