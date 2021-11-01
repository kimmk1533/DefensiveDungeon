using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct InventorySlotGUIInfo
{
    public int index;
    public bool isOccupied;
    public Tower_TableExcel tower_data;
    public Tower tower;
}

[System.Serializable]
public class CKeyValue : System.IEquatable<CKeyValue>
{   // code to tower object
    public int Code;
    public GameObject obj;

    public bool Equals(CKeyValue other)
    {
        if (Code == other.Code)
            return true;
        return false;
    }
}

public class InventorySlotGUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler//IPointerEnterHandler,IPointerExitHandler
{
    public delegate void SummonTowerHandler(Tower tower);
    public event SummonTowerHandler OnTowerSummonEvent;

    public delegate void InfoChangeHandler();
    public event InfoChangeHandler OnInfoChangedEvent;

    [SerializeField] InventorySlotGUIInfo m_info;
    //[SerializeField] TMPro.TextMeshProUGUI m_textPro;

    [Space(10)]
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    [SerializeField] Prefab_TableExcelLoader m_prefabLoader;
    [SerializeField] Vector3 camera_distance;           // from showObj to camera
    [SerializeField] Vector3 camera_rotation;           // camera eular rotation
    [SerializeField] Vector3 m_obj_position;            // showObj position
    [SerializeField] List<CKeyValue> m_showObj_list;    // showObjs
    GameObject m_showObj;   // current show obj

    [Space(10)]
    [SerializeField] RawImage m_rawImage;
    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    RectTransform m_rt;

    //TowerUI_Tooltip tower_tooltip;


    public bool IsOccupied { get { return m_info.isOccupied; } }

    public Tower TowerObj { 
        get {
            if (IsOccupied)
                return m_info.tower;
            else 
                return null;
        } }

    private void Awake()
    {
        //tower_tooltip = TowerUI_Tooltip.Instance;
        OnInfoChangedEvent += OnInfoChanged;

        m_rt = this.GetComponent<RectTransform>();
        SetRenderTexture();
    }

    private void Start()
    {

    }

    public void SetRenderTexture()
    {
        int layer = LayerMask.NameToLayer("Tower");

        // create render texture
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();        

        /// camera setting
        Camera inven_cam_origin = Resources.Load<Camera>("InventoryCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(inven_cam_origin);
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
        // create all tower for this InvenSlotUI
        var tower_data_list = m_towerLoader.DataList.GetRange(3, m_towerLoader.DataList.Count - 3);
        foreach (var item in tower_data_list)
        {
            GameObject origin_obj = m_prefabLoader.GetPrefab(item.Prefab); // get only tower
            GameObject new_obj = GameObject.Instantiate(origin_obj);
            new_obj.transform.SetParent(this.transform);

            // set layer (for camera culling)
            Transform[] allChildren = new_obj.GetComponentsInChildren<Transform>(true);
            foreach (var child  in allChildren)
            {
                child.gameObject.layer = layer;
            }


            // scaling
            float scale_rate = m_prefabLoader.DataList.Find(
                (prefabtable_item) => { return item.Prefab == prefabtable_item.Code; })
                .Size;
            new_obj.transform.GetChild(0).localScale =
               new Vector3(scale_rate, scale_rate, scale_rate);
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
        m_rawImage.texture = m_renderTexture;
    }

    public void MoveRenderPosition(Vector3 delta)
    {
        m_renderCamera.transform.position += delta;
        foreach (var item in m_showObj_list)
        {
            item.obj.transform.position += delta;
        }
    }


    // InvenSlot Swap
    private void SwapInfo(InventorySlotGUI otherSlotGUI)
    {
        // tower swap
        var this_tower_data = m_info.tower_data;
        bool this_occupied = m_info.isOccupied;
        Tower this_tower = m_info.tower;


        m_info.tower_data = otherSlotGUI.m_info.tower_data;
        m_info.isOccupied = otherSlotGUI.m_info.isOccupied;
        m_info.tower = otherSlotGUI.m_info.tower;


        otherSlotGUI.m_info.tower_data = this_tower_data;
        otherSlotGUI.m_info.isOccupied = this_occupied;
        otherSlotGUI.m_info.tower = this_tower;

        otherSlotGUI.OnInfoChangedEvent?.Invoke();
        OnInfoChangedEvent?.Invoke();       // change ui
    }

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    public void SetTower(Tower tower, Tower_TableExcel data)
    {        
        m_info.tower = tower;
        m_info.tower_data = data;
        m_info.isOccupied = true;

        OnInfoChangedEvent?.Invoke();
    }

    public void ClearInven()
    {
        //if (null != m_info.tower)
        //{
        //    TowerManager.Instance.DespawnTower(m_info.tower);
        //}

        m_info.isOccupied = false;
        m_info.tower = null;

        OnInfoChangedEvent?.Invoke();
    }

    public void ForceUIUpdate()
    {
        OnInfoChanged();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////


    private void OnInfoChanged()
    {
        m_showObj?.SetActive(false);

        if (IsOccupied == false)
        {   // is empty slot (tower)
            m_info.tower = null;
            m_showObj = null;
            //m_textPro.text = "Empty";
            return;
        }

        // is occupied (tower)
        // 1. find tower using code
        // 2. set tower infomation to UI
        int code = m_info.tower_data.Code;
        m_showObj = m_showObj_list.Find((item) => { return item.Code == code; }).obj;
        m_showObj.gameObject.SetActive(true);
        //m_textPro.text = m_info.tower_data.Name_EN;
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////

    Vector3 m_drag_startPos;
    bool m_swapFlag;

    // when slot is selected , image must be FRONT
    private void SetThisSLotFirstOrder()
    {
        this.transform.SetAsFirstSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.IsPointerMoving() && m_info.isOccupied)
        {
            //Debug.Log("tower image moving");
            m_rawImage.transform.position = eventData.position;

            Vector3 mouse_pos = eventData.position;
            mouse_pos.z = Camera.main.farClipPlane;
            Debug.DrawRay(Camera.main.transform.position,
                Camera.main.ScreenToWorldPoint(mouse_pos), Color.red);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_drag_startPos = m_rawImage.transform.position;
        if (m_info.isOccupied)
        {
            m_swapFlag = true;
            //Debug.Log("tower image move start");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        RaycastResult target = eventData.pointerCurrentRaycast;
        if (m_swapFlag &&
            target.gameObject != null &&
            target.gameObject.tag.Equals("InvenSlot"))
        {
            // if.. begin drag on Slot UI && end drag on Slot UI
            // swap slot infomation
            SwapInfo(target.gameObject.GetComponent<InventorySlotGUI>());
        }
        else if (m_info.isOccupied)
        {   // if Pointer is out of UI , 
            // Check Pointer is on the Slot (for summon tower process)

            // if mouse is out of UI 
            if (false == EventSystem.current.IsPointerOverGameObject())
            {
                /// for perspective
                Vector3 mouse_pos = eventData.position;
                mouse_pos.z = 1000.0f;

                /// for othographic
                //Vector3 mouse_pos = eventData.position;

                int layermask = 1 << LayerMask.NameToLayer("Node");
                RaycastHit hitinfo;

                /// for perspective
                if (Physics.Raycast(new Ray(Camera.main.transform.position,
                    Camera.main.ScreenToWorldPoint(mouse_pos)),
                    out hitinfo,
                    1000f,
                   layermask))
                ///for othographic
                //if (Physics.Raycast(new Ray(Camera.main.ScreenToWorldPoint(mouse_pos), Camera.main.transform.forward),
                //out hitinfo,
                //Camera.main.farClipPlane,
                //layermask))
                {
                    Node hit_node = hitinfo.collider.gameObject.GetComponent<Node>();
                    if (hit_node.m_Tower == null)
                    {
                        Debug.Log("Summon!");
                        hit_node.SetTower(m_info.tower);
                        ClearInven();
                    }
                }
            }
        }

        // reset moved image position
        //Debug.Log("tower image move end");
        m_swapFlag = false;
        m_rawImage.transform.position = m_drag_startPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {   // for tower tooltip panel
        if (IsOccupied &&
            eventData.button == PointerEventData.InputButton.Right)
        {
            Vector2 mousepos = Input.mousePosition;
            TowerToolTipManager.Instance.ActivateToolTipOnUIClick(mousepos, this, m_info.tower_data);
        }
    }
}
