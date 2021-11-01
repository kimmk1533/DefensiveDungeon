using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public struct ShopSlotInfo
{
    public bool isOccupied;
    public int index;

    public int cost;
    public Tower_TableExcel? excel_data;
}

// Shop slot 에 관한 정보를 담는 클래스
public class ShopSlot : MonoBehaviour , IPointerClickHandler
{
    delegate void OnInfoChangedHandler();
    OnInfoChangedHandler OnInfoChangedCallback;

    [SerializeField] ShopSlotInfo m_info;

    [Space(30)]
    [SerializeField] TextMeshProUGUI m_nameTextPro;
    [SerializeField] TextMeshProUGUI m_synergy1TextPro;
    [SerializeField] TextMeshProUGUI m_synergy2TextPro;
    [SerializeField] TextMeshProUGUI m_goldTextPro;

    [Space(30)]
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Synergy_TableExcelLoader m_synergyLoader;
    [SerializeField] Image m_goldImage;
    [SerializeField] Image m_synergy1Image;
    [SerializeField] Image m_synergy2Image;

    [Space(30)]
    [SerializeField] RawImage m_towerImage;
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    [SerializeField] Prefab_TableExcelLoader m_prefabLoader;
    [SerializeField] Vector3 camera_distance;           // 오브젝트로부터의 거리
    [SerializeField] Vector3 camera_rotation;           // 카메라 회전 값
    [SerializeField] Vector3 m_obj_position;            // 간섭 없는 곳으로 셋팅할것
    [SerializeField] List<CKeyValue> m_showObj_list;
    GameObject m_showObj = null;

    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    Dictionary<int, Color> m_rankToColor_dic;

    public bool IsOccupied { get { return m_info.isOccupied; } }
    public int Price { get { return m_info.cost; } }

    private void Awake()
    {
        m_rankToColor_dic = new Dictionary<int, Color>();
        m_rankToColor_dic.Add(1, Color.white);
        m_rankToColor_dic.Add(2, Color.green);
        m_rankToColor_dic.Add(3, Color.blue);
        m_rankToColor_dic.Add(4, new Color(0.7f,0f,1f));
        m_rankToColor_dic.Add(5, Color.yellow);



        OnInfoChangedCallback += OnInfoChanged;
        SetRenderTexture();
    }

    public void MoveRenderPosition(Vector3 delta)
    {
        m_obj_position += delta;
        m_renderCamera.transform.position += delta;
        foreach (var item in m_showObj_list)
        {
            item.obj.transform.position += delta;
        }
    }
    public void SetRenderTexture()
    {
        int layer = LayerMask.NameToLayer("Tower");

        // create render texture
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();

        /// camera setting
        Camera shop_cam_origin = Resources.Load<Camera>("ShopCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(shop_cam_origin);
        m_renderCamera.transform.SetParent(this.transform);

        m_renderCamera.clearFlags = CameraClearFlags.SolidColor;
        m_renderCamera.backgroundColor = new Color(0, 0, 0, 0);
        m_renderCamera.cullingMask = 1 << layer;
        m_renderCamera.farClipPlane = 50f;

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;
        /// camera setting end

        /// tower objects setting

        // create all tower for this ShopSlotUI
        // except devil (character)
        var tower_data_list = m_towerLoader.DataList.GetRange(3, m_towerLoader.DataList.Count - 3);
        foreach (var item in tower_data_list)
        {
            GameObject origin_obj = m_prefabLoader.GetPrefab(item.Prefab); // get only tower
            GameObject new_obj = GameObject.Instantiate(origin_obj);
            new_obj.transform.SetParent(this.transform);

            // set layer (for camera culling)
            Transform[] allChildren = new_obj.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                child.gameObject.layer = layer;
            }

            // scaling
            float scale_rate = m_prefabLoader.DataList.Find(
                (prefabtable_item) => { return item.Prefab == prefabtable_item.Code; })
                .Size;
            new_obj.transform.GetChild(0).localScale = 
                new Vector3( scale_rate, scale_rate, scale_rate);            
            //new_obj.transform.localScale *= scale_rate;

            // regist to managing list
            CKeyValue val = new CKeyValue
            { Code = item.Code, obj = new_obj };
            m_showObj_list.Add(val);
        }       

        // deactivate all created objects
        foreach (var item in m_showObj_list)
        {         
            item.obj.transform.position = m_obj_position;
            item.obj.gameObject.SetActive(false);
        }
        /// tower setting end

        // set render texture
        m_towerImage.texture = m_renderTexture;
    }

       

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    // index 는 초기화 하지 않음
    public void ClearInfo()
    {
        m_info.isOccupied = false;

        m_info.cost = 0;
        m_info.excel_data = null;

        OnInfoChangedCallback?.Invoke();
    }

    // slot 을 비워야 할 경우는 ClearInfo를 사용할 것    
    public void SetInfo(int cost, Tower_TableExcel excel)
    {
        m_info.isOccupied = true;
        
        m_info.cost = cost;
        m_info.excel_data = excel;     

        OnInfoChangedCallback?.Invoke();
    }
    public ShopSlotInfo GetInfo()
    {
        return m_info;
    }

    void DeActivateAll()
    {
        foreach (var item in m_showObj_list)
        {
            item.obj.SetActive(false);
        }
    }


    // 현재 정보에 따라 ui 변경 process
    void OnInfoChanged()
    {       
        if(IsOccupied)
        {
            ///tower obj
            // deactivate previous obj
            m_showObj?.SetActive(false);
            // get current data
            var data = m_info.excel_data.Value;
            // activate current obj
            m_showObj = m_showObj_list.Find((item) => { return item.Code == data.Code; }).obj;
            m_showObj.SetActive(true);
            /// tower obj end

            /// synergy icon & text          
            // find synergy data
            var synergy1_data = m_synergyLoader.DataList.Find(
                (item) => { return item.Code == data.Type1; });
            var synergy2_data = m_synergyLoader.DataList.Find(
                (item) => { return item.Code == data.Type2; });

            // set sprite
            Sprite synergy1_sprite = m_spriteLoader.GetSprite(synergy1_data.Synergy_icon);
            m_synergy1Image.sprite = synergy1_sprite;
            Sprite synergy2_sprite = m_spriteLoader.GetSprite(synergy2_data.Synergy_icon);
            m_synergy2Image.sprite = synergy2_sprite;             


            // set text
            m_nameTextPro.text = data.Name_KR;
            m_nameTextPro.color = m_rankToColor_dic[data.Rank];
            m_synergy1TextPro.text = synergy1_data.Name_KR;
            m_synergy2TextPro.text = synergy2_data.Name_KR;
            m_goldTextPro.text = ((int)data.Price).ToString();
            /// synergy icon & text end           


            SetActivateAllChildren();
        }
        else
        {
            // deactivate previous obj
            m_showObj?.SetActive(false);

            SetDeActivateChilderen();
        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        Debug.Log($"Shop Slot ({m_info.index})");

        // 이 슬롯이 활성화 된 경우
        if(IsOccupied)
        {
            if(ShopManager.Instance.PurchaseProcess(m_info.index))
            {   // 구매에 성공한 경우
                ClearInfo();
            }
        }
    }

    public void SetActivateAllChildren()
    {
        RectTransform[] children = this.transform.GetComponentsInChildren<RectTransform>(true);
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);
        }        
    }
    public void SetDeActivateChilderen()
    {
        RectTransform[] children = this.transform.GetComponentsInChildren<RectTransform>();
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
        }       
    }
}
