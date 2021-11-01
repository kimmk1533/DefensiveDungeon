using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipTowerImage : MonoBehaviour
{
    // for render texture obj
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    [SerializeField] Prefab_TableExcelLoader m_prefabLoader;
    [SerializeField] Vector3 camera_distance;           // from showObj to camera
    [SerializeField] Vector3 camera_rotation;           // camera eular rotation
    [SerializeField] Vector3 m_obj_position;            // showObj position
    [SerializeField] List<CKeyValue> m_showObj_list;    // showObjs
    GameObject m_showObj;                               // current show obj

    // for render texture
    [Space(20)]
    [SerializeField] Camera m_renderCamera;
    [SerializeField] RawImage m_rawImage;
    RenderTexture m_renderTexture;

    private void Awake()
    {
        __InitializeTexture();
    }

    void __InitializeTexture()
    {
        int layer = LayerMask.NameToLayer("Tower");

        // create render texture
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();

        // set camera option for render texture
        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.clearFlags = CameraClearFlags.SolidColor;
        m_renderCamera.backgroundColor = new Color(1f, 1f, 1f, 0f);
        m_renderCamera.cullingMask = 1 << layer;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;

        // get tower data except devil(player character)
        var tower_data_list = m_towerLoader.DataList.GetRange(
            3,  // 3 : devil count
            m_towerLoader.DataList.Count - 3);

        // instantiate all tower
        foreach (var item in tower_data_list)
        {
            // original info
            GameObject origin_prefab = m_prefabLoader.GetPrefab(item.Prefab);
            float scale_rate = m_prefabLoader.DataList.Find(
                (prefabtable_item) =>
                { return item.Prefab == prefabtable_item.Code; })
                .Size;           

            // create
            var new_obj = GameObject.Instantiate(origin_prefab);
            new_obj.transform.position = m_obj_position;
            new_obj.transform.SetParent(this.transform);
            new_obj.SetActive(false);

            // set layer (for camera culling)
            Transform[] allChildren = new_obj.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                child.gameObject.layer = layer;
            }

            // scaling
            new_obj.transform.GetChild(0).localScale = new Vector3(scale_rate, scale_rate, scale_rate);

            // regist to managing list
            m_showObj_list.Add(
                new CKeyValue()
                {
                    Code = item.Code,
                    obj = new_obj
                });
        }
        // instantiate tower end

        // set UI raw image's target texture
        m_rawImage.texture = m_renderTexture;
    }

    public void SetUI(int tower_code)
    {
        // deactivate previous tower
        m_showObj?.SetActive(false);
        // set new obj & activate
        m_showObj = m_showObj_list.Find((item) => { return item.Code == tower_code; }).obj;
        m_showObj.SetActive(true);
    }
}
