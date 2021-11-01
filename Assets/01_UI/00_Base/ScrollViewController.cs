using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 노드의 개수와 scroll view 의 크기에 맞춰서 딱 맞게 노드를 셋팅하는 클래스
[ExecuteInEditMode]
public class ScrollViewController : MonoBehaviour
{
    RectTransform m_thisRectTransform;
    [SerializeField] bool m_panel_size_fitting; // cell 사이즈에 맞춰 fitting

    // 이하 scroll view 셋팅하기
    [Space(10)]
    [SerializeField] ScrollRect m_scrollrect;       // scrollrect 의 size 에 맞춰서 노드 사이즈 조절
    [SerializeField] GridLayoutGroup m_content;     // 노드의 부모

    [SerializeField] RectOffset m_padding;
    [SerializeField] Vector2Int m_cellCount;    // 해당 스크롤 뷰에 할당될 노드 개수
    [SerializeField] Vector2 m_cellsize;    // space 와 padding 으로 자동 조절됨
    [SerializeField] Vector2 m_spacing;
    [SerializeField] TextAnchor m_childAlignment;
    [SerializeField] GridLayoutGroup.Constraint m_constraint;


    public void SetCellCount(int row, int col)
    {
        m_cellCount.x = row;
        m_cellCount.y = col;

        SetContentLayout();
    }

    private void Awake()
    {
        m_thisRectTransform = this.GetComponent<RectTransform>();
        
        SetContentLayout();
    }



    private void Update()
    {
#if UNITY_EDITOR
        SetContentLayout();
#endif
    }



    void SetContentLayout()
    {
        // 모든 노드의 총 공간
        float total_node_width = m_cellCount.x * m_cellsize.x;
        float total_node_height = m_cellCount.y * m_cellsize.y;

        // 상하 좌우 노드 사이의 총 공간 구하기
        // space 는 노드의 개수보다 한개 적음
        float total_space_width = (m_cellCount.x - 1) * m_spacing.x;
        float total_space_height = (m_cellCount.y - 1) * m_spacing.y;

        // panel 크기 = 노드 총 공간 + space 총 공간 + 상하좌우 마진
        float total_width = total_node_width + total_space_width + m_padding.left + m_padding.right;
        float total_height = total_node_height + total_space_height + m_padding.top + m_padding.bottom;

        // 패널 크기 조절
        if(m_panel_size_fitting)
        {
            m_thisRectTransform.sizeDelta = new Vector2(total_width, total_height);
            //Debug.LogFormat($"panel size delta : {total_width}, {total_height}");
        }



        // 현제 프로젝트는 기본 기준은 1920 1080 으로 하고있음
        // 해상도 별로 스케일 되는 값으로 보정이 필요함
        var _scale = m_scrollrect.GetComponent<RectTransform>().lossyScale;
        Scaling(_scale);

        // 하위 컴포넌트에 셋팅
        m_content.padding = m_padding;
        m_content.cellSize = m_cellsize;
        m_content.spacing = m_spacing;
        m_content.childAlignment = m_childAlignment;
        m_content.constraint = m_constraint;
        m_content.constraintCount = m_cellCount.x;
    }

    void Scaling(Vector3 scale)
    {
        foreach (Transform child in m_content.transform)
        {
            child.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        //float scale_x = scale.x;
        //float scale_y = scale.y;

        //m_padding = new RectOffset((int)(m_padding.left * scale_x),
        //                            (int)(m_padding.right * scale_x),
        //                            (int)(m_padding.top * scale_y),
        //                            (int)(m_padding.bottom * scale_y));
        //m_cellsize = new Vector2(m_cellsize.x * scale_x, m_cellsize.y * scale_y);
        //m_spacing = new Vector2(m_spacing.x * scale_x, m_spacing.y * scale_y);
    }
}
