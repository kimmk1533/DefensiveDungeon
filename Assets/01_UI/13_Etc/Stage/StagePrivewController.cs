using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ----------slots----------------
// hide / 1 / 2 / 3 / 4 / 5 / hide
// make hide slot because of moving animation
public class StagePrivewController : MonoBehaviour
{
    StageChangedEventArgs m_current_stageInfo;

    private int m_total_cell_count;
    [SerializeField] int m_cell_count;      // total cell = m_cell_count + 2
    [SerializeField] float m_padding_width = 10f;
    [SerializeField] float m_padding_height = 10f;

    [Space(20)]
    [SerializeField] Image m_root_panel;
    [SerializeField] StageIconSlot m_origin_slot;
    [SerializeField] List<StageIconSlot> m_slots;
    [SerializeField] List<float> m_slot_positionX; // standard position (when animation end , each slot return this pos) 

    [SerializeField] Stage_TableExcelLoader m_stage_loader;    

    private void Awake()
    {
        if (m_cell_count == 0)
            m_cell_count = 7;

        m_total_cell_count = m_cell_count + 2;
    }

    private void Start()
    {
        __Initailize();
    }

    private void Update()
    {
       
    }

    private void __Initailize()
    {
        StageInfoManager.Instance.OnStageChangedEvent += __OnStageChnaged;
        // stage manager 로 부터 정보를 가져오기

        // origin
        m_origin_slot.gameObject.SetActive(false);

        Vector2 cell_size = GetCellSize(m_root_panel, m_total_cell_count, m_padding_width, m_padding_height);

        float x_pos = 0.0f;

        // copy cells
        for (int i = 0; i < m_total_cell_count; i++)
        {
            StageIconSlot newSlot = GameObject.Instantiate<StageIconSlot>(m_origin_slot);

            newSlot.gameObject.SetActive(true);
            newSlot.transform.SetParent(m_root_panel.transform);

            newSlot.SetSize(cell_size);
            newSlot.SetPosition(x_pos);
            m_slot_positionX.Add(x_pos);
            x_pos += (cell_size.x + m_padding_width);
            newSlot.ChangeImage(m_stage_loader.DataList[i].Stage_icon);
            m_slots.Add(newSlot);
        }

        // first and last alpha
        m_slots[0].SetImageAlpha(0f);
        m_slots[m_slots.Count - 1].SetImageAlpha(0f);
    }

    Vector2 GetCellSize(Image root_panel, int total_count, float padding_width, float padding_height)
    {
        RectTransform root_rt = root_panel.GetComponent<RectTransform>();
        // root panel size
        float total_width = root_rt.rect.width;
        float total_height = root_rt.rect.height;

        // total padding size
        float total_padding_width = padding_width * (total_count - 1);
        float total_padding_height = padding_height * 2;

        // one cell size
        float cell_widht = (total_width - total_padding_width) / total_count;
        float cell_height = total_height - total_padding_height;

        return new Vector2(cell_widht, cell_height);
    }

    public void __OnStageChnaged(StageChangedEventArgs args)
    {
        m_current_stageInfo = args;
        MoveSlotsAnimation();
    }

    //
    void MoveSlotsAnimation()
    {
        StartCoroutine(Co_Move(UpdateStageUIInfo));
    }

    public IEnumerator Co_Move(System.Action callback)
    {
        while (true)
        {
            for (int i = 1; i < m_slots.Count; i++)
            {
                Vector2 move_pos = Vector2.Lerp(
                    new Vector2(m_slots[i].GetAnckorX(), 0),
                    new Vector2(m_slot_positionX[i - 1], 0),
                    Time.deltaTime
                    );

                m_slots[i].SetPosition(move_pos.x);
                if(i == 1 || i == m_slots.Count - 1)
                {
                    Color target_color = m_slots[i].GetImageColor();
                    if (i == 1)
                        target_color.a = 0f;
                    else
                        target_color.a = 1f;

                    Color color = Color.Lerp(m_slots[i].GetImageColor(),
                        target_color,
                        Time.deltaTime
                        );
                    m_slots[i].SetImageColor(color);
                }
            }

            // alpha setting
            

            if (m_slots[1].GetAnckorX() < 5f)
                break;

            yield return null;
        }

        callback.Invoke();
    }

    // after move slot animation
    void UpdateStageUIInfo()
    {
        /// ....after animation 
        // return each cell postion
        for (int i = 0; i < m_slots.Count; i++)
        {
            m_slots[i].SetPosition(m_slot_positionX[i]);
        }
        // return each cell alpha
        m_slots[0].SetImageAlpha(0f);
        m_slots[1].SetImageAlpha(1f);
        m_slots[m_slots.Count - 1].SetImageAlpha(0f);




        // set stage info as much as cell count from current stage
        int current_stage = m_current_stageInfo.stage_num;
        for (int i = 1; i < m_cell_count + 2; i++)
        {
            int index = i + current_stage - 1;
            if (index < m_stage_loader.DataList.Count)
            {
                var stage_data = m_stage_loader.DataList[index];
                m_slots[i].ChangeImage(stage_data.Stage_icon);                
            }
            else
            {
                // TODO : end of stage
            }
        }
    }
}
