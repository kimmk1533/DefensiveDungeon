using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Grid layout 전용
[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(Image))]
public class CellSizeFitter : MonoBehaviour
{
    public delegate void OnSettingChangeHandler();
    public event OnSettingChangeHandler OnSettingChanged;
    GridLayoutGroup m_layoutGroup = null;

    [SerializeField] RectOffset m_padding;
    [SerializeField] Vector2Int m_cellcount;
    [SerializeField] Vector2 m_cellsize;    // space 와 padding 으로 자동 조절됨
    [SerializeField] Vector2 m_spacing;

    float width_scale;
    float height_scale;

    public Vector2Int CellCount
    {
        get
        {
            return m_cellcount;
        }
    }

    private void Awake()
    {
        m_layoutGroup = this.GetComponent<GridLayoutGroup>();
        if (m_padding == null)
            m_padding = new RectOffset(1, 1, 1, 1);  
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (m_cellcount.x < 1)
            m_cellcount.x = 1;
        if (m_cellcount.y < 1)
            m_cellcount.y = 1;
        m_layoutGroup = this.GetComponent<GridLayoutGroup>();
        SetCellSize();
    }
#endif

    public Vector2 SetCellCount(int cellcountX, int cellcountY)
    {
        m_cellcount.x = cellcountX;
        m_cellcount.y = cellcountY;

        SetCellSize();

        return m_cellsize;
    }

    private void SetCellSize()
    {
        RectTransform panel_rt = this.GetComponent<RectTransform>();
        Vector2 panel_size = panel_rt.rect.size;
        //Debug.Log(panel_size);

        float total_width_space = m_padding.left + m_padding.right + m_spacing.x * (m_cellcount.x - 1);
        float total_height_space = m_padding.top + m_padding.bottom + m_spacing.y * (m_cellcount.y - 1);

        m_cellsize.x = (panel_size.x - total_width_space) / m_cellcount.x;
        m_cellsize.y = (panel_size.y - total_height_space) / m_cellcount.y;

        UpdateGridLayoutGroup();

        RectTransform[] children_rt = panel_rt.GetComponentsInChildren<RectTransform>();
        foreach (var item in children_rt)
        {
            item.localScale = new Vector3(1, 1 , 1);
        }

        OnSettingChanged?.Invoke();
    }

    private void UpdateGridLayoutGroup()
    {
        m_layoutGroup.cellSize = m_cellsize;
        m_layoutGroup.spacing = m_spacing;
        m_layoutGroup.padding = m_padding;

        Canvas.ForceUpdateCanvases();
    }
}
