using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Root Panel 에 origin slot을 동적으로 생성
// Root Panel 에는 CellSizeFitter 가 존재해야함
public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] Tower_TableExcelLoader m_excel_towerdata_so;
    List<Tower_TableExcel> m_tower_data_list;
    [SerializeField] Shop_TableExcelLoader m_excel_shopdata_so;
    private Shop_TableExcel m_cur_data;         // 가장 최근에 UserInfoManger 로부터 불러온 데이터 값
    private List<float> m_rates = null;         // 위의 data 에서 확률만 뽑아논 리스트
    private List<int> m_costs = null;           // 위의 data 에서 cost만 뽑아논 리스트

    // SetActive On Off 용
    [SerializeField] Image m_shop_panel;
    // root panel 의 하위로 slot 을 생성
    [SerializeField] Image m_root_panel;
    // root panel 의 size fitter
    // fitter 에서 정의한 slot 개수만큼 생성
    private CellSizeFitter m_fitter;

    // 좌측 하단 레벨과 확률을 조절
    [SerializeField] ShopStatusUIController m_status_controller;

    [SerializeField] ShopSlot m_origin;
    [SerializeField] List<ShopSlot> m_slot_list;

    private void Awake()
    {
        __Initialize();
    }

    private void Start()
    {
        // 최초 Status 리셋        
        OnLevelChanged(UserInfoManager.Instance.Level);
        // 최초 상점 리셋
        ShopReset();
        // lock button release
        ShopUnLock();

        m_shop_panel.gameObject.SetActive(false);

        // link callback
        UserInfoManager.Instance.OnLevelChanged += OnLevelChanged;
        StageInfoManager.Instance.OnStageChangedEvent += OnStageChanged;
    }

    public void __Initialize()
    {
        // this memeber alloc
        m_cur_data = new Shop_TableExcel();
        m_rates = new List<float>();
        m_costs = new List<int>();

        // setting
        m_origin.gameObject.SetActive(false);

        m_fitter = m_root_panel.GetComponent<CellSizeFitter>();
        int total_alloc_count = m_fitter.CellCount.x * m_fitter.CellCount.y;

        // new slots Instantiate
        for (int i = 0; i < total_alloc_count; i++)
        {
            ShopSlot newSlot = GameObject.Instantiate<ShopSlot>(m_origin);
            m_slot_list.Add(newSlot);
            newSlot.__Indexing(i);
            newSlot.transform.SetParent(m_root_panel.transform);
            newSlot.gameObject.SetActive(true);
            newSlot.MoveRenderPosition(new Vector3(100 * i, 100 * i, 0));   // 렌더링 위치가 달라지도록
        }

        // sub 3 item form front ( except devil )
        m_tower_data_list = m_excel_towerdata_so.DataList.GetRange(
            3,
            m_excel_towerdata_so.DataList.Count - 3);
    }


    public void OnStageChanged(StageChangedEventArgs args)
    {
        ShopReset();
    }

    /*************************** UI Control ******************************/

    // 레벨 변경시 좌측 하단 패널의 text 변경
    public void OnLevelChanged(int level)
    {
        // user info 를 가져오고 레벨에 따라 확률을 셋팅함
        int cur_user_level = UserInfoManager.Instance.Level;

        var data_list = m_excel_shopdata_so.DataList;

        m_cur_data = data_list.Find((item) => { return cur_user_level == item.User_Level; });


        m_rates.Clear();
        m_rates.Add(m_cur_data.Tower_Rand1);
        m_rates.Add(m_cur_data.Tower_Rand2);
        m_rates.Add(m_cur_data.Tower_Rand3);
        m_rates.Add(m_cur_data.Tower_Rand4);
        m_rates.Add(m_cur_data.Tower_Rand5);

        m_costs.Clear();
        m_costs.Add(m_cur_data.Tower_Gold1);
        m_costs.Add(m_cur_data.Tower_Gold2);
        m_costs.Add(m_cur_data.Tower_Gold3);
        m_costs.Add(m_cur_data.Tower_Gold4);
        m_costs.Add(m_cur_data.Tower_Gold5);

        m_status_controller.SetRates(m_rates.ToArray());
        m_status_controller.SetLevel(cur_user_level);
    }

    /*************************** Mouse callback ******************************/
    // TODO : 타워 매니저를 통해 타워 생성
    // TODO : Inventory 에 추가
    /*************************** button Process ******************************/

    [Space(30)]
    [SerializeField] bool m_isLocked = false;
    [SerializeField] Image m_LockButtonImage;
    [SerializeField] Sprite m_LockSprite;
    [SerializeField] Sprite m_UnLockSprite;

    private void ShopReset()
    {
        // rank 지정 확률
        float rand_val = 0.0f;

        // tower data 개수
        int total_towerType_count = m_tower_data_list.Count;
        // tower type 지정 확률        
        int rand_val_tower = 0;

        // slot 개수만큼 반복
        for (int i = 0; i < m_slot_list.Count; i++)
        {
            rand_val = Random.Range(0.0f, 1.0f);
            rand_val_tower = Random.Range(0, total_towerType_count);    // index 기준(No - 1)

            int rank_forCreate = 1;
            int cost = 0;
            float acc = 0.0f;
            // 낮은 랭크의 확률부터 계산
            for (int j = 0; j < m_rates.Count; j++)
            {
                acc += m_rates[j];
                if (rand_val <= acc)
                {
                    rank_forCreate = j + 1;
                    cost = m_costs[j];
                    break;
                }
            }

            //Debug.Log($"rank : {rank_forCreate}");           

            // shop can create tower ONLY 1 STAR
            var OnlyOneStar_tower_data_list = m_tower_data_list.FindAll((item) => { return item.Star == 1; });

            // rank 에 부합하는 데이터 뽑기
            var tower_data_list = OnlyOneStar_tower_data_list.FindAll((item) => { return item.Rank == rank_forCreate; });

            // tower_data_list 중에 하나
            int selected_index = Random.Range(0, tower_data_list.Count);

            // 위에서 생성된 데이터로 Shop Slot의 정보 업데이트            
            var tower_data = tower_data_list[selected_index];
            m_slot_list[i].SetInfo(cost, tower_data);
        }
    }


    private void ShopLock()
    {
        m_LockButtonImage.sprite = m_LockSprite;
    }
    private void ShopUnLock()
    {
        m_LockButtonImage.sprite = m_UnLockSprite;
    }

    public bool PurchaseProcess(int slotIndex)
    {
        ShopSlot slot = m_slot_list[slotIndex];

        if (false == InventoryManager.Instance.IsAllOccupied())
        {   // 인벤에 빈 공간이 있으면
            if (false == UserInfoManager.Instance.UseGold(slot.Price))
            {   // 보유 골드가 부족하면
                Debug.Log("Shop : have not enough minerals");
                return false;
            }

            var info = slot.GetInfo();

            InventoryManager.Instance.AddNewTower(info.excel_data.Value);

            Debug.Log("Puchase!!");
            return true;
        }
        else
        {

        }

        return false;
    }

    public Tower CombineTowerProcess(int tower_code)
    {   // tower count is always over 2
        var towers = TowerManager.Instance.GetTowers(tower_code);
        for (int i = 0; i < 3; i++)
        {
            if (towers[i].m_TowerInfo.IsOnInventory)
                TowerManager.Instance.DespawnTower(towers[i]);
        }

        // TODO : fix

        //TowerManager.Instance.SpawnTower

        return null;
    }

    /*************************** button callback ******************************/

    public void __OnResetButtonClicked()
    {
        Debug.Log("ShopReset");

        if (false == m_isLocked)
        {
            int reset_cost = m_excel_shopdata_so.DataList.Find(
                (item) => { return item.User_Level == UserInfoManager.Instance.Level; }).Reset_Gold;

            if (UserInfoManager.Instance.UseGold(reset_cost))
                ShopReset();
        }
    }

    // Lock 된 경우 true
    // UnLock 된 경우 false
    public void __OnLockButtonClicked()
    {
        m_isLocked = !m_isLocked;
        if (m_isLocked)
        {
            ShopLock();
            Debug.Log("ShopLock");
        }
        else
        {
            ShopUnLock();
            Debug.Log("ShopUnLock");
        }
    }

    public void __OnShopButtonClicked()
    {
        if (m_shop_panel.gameObject.activeSelf)
            m_shop_panel.gameObject.SetActive(false);
        else
            m_shop_panel.gameObject.SetActive(true);
    }
}
