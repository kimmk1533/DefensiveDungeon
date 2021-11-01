using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Size
{
    public int width;
    public int height;

    public Size(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}


public enum E_UISpaceType
{
    Equal,      // target 과 동일한 비율임
    LetterBox,  // target 보다 실제 canvas 가 상하로 더 김 (위아래로 letter box)
    PillarBox,  // target 보다 실제 canvas 가 좌우로 더 김 (좌우로 pillar box)
}

public class AspectManager : Singleton<AspectManager>
{
    Canvas m_main_canvas = null;
    RectTransform m_main_canvas_rt = null;

    const float m_tolerance = 0.0001f;   // 허용 범위
    [SerializeField] E_UISpaceType m_UISpaceType;

    //// resoultion   // 대충 preference 에서 가져온다는 뜻
    //[SerializeField] int m_target_width;    // target resoultion width
    //[SerializeField] int m_target_height;   // target resoultion height

    [SerializeField] float m_target_ratio;      // target resolution 비율
    [SerializeField] float m_current_ratio;     // 현재 기기의 실제 비율(canvas 동일)

    [SerializeField] Size m_main_panel_size;            // unity size 기준
    [SerializeField] Vector2 m_main_panel_sizeDelta;    // 실제 resolution 관련 비율
    [SerializeField] Size m_canvas_size;                // unity size 기준
    [SerializeField] Vector2 m_canvas_sizeDelta;        // 실제 resolution 관련 비율    

    [SerializeField] float m_scale;

    public E_UISpaceType GetSpaceType()
    {
        return m_UISpaceType;
    }
    public float GetCanvasRatio()
    {
        return m_current_ratio;
    }
    public float GetMainPanelRatio()
    {
        return m_target_ratio;
    }
    public Size GetCanvasSize()
    {
        return m_canvas_size;
    }
    public Size GetMainPanelSize()
    {
        return m_main_panel_size;
    }
    public Vector2 GetCanvasSizeDelta()
    {
        return m_canvas_sizeDelta;
    }
    public Vector2 GetMainPanelSizeDelta()
    {
        return m_main_panel_sizeDelta;
    }

    private void Awake()
    {
        m_main_canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        m_main_canvas_rt = m_main_canvas.GetComponent<RectTransform>();        
    }

    private void Start()
    {        
        SettingManager.Instance.OnSettingChnagedEvent += __Init;

        //__Init();
    }

    private void Update()
    {
#if UNITY_EDITOR
        __Init(SettingManager.Instance.GetCurrentSetting());
#endif
    }

    public void __Init(SettingArgs e)
    {
        Vector2 resolution = ScreenDataUtility.GetResoultion(e.screen_data);        
        m_target_ratio = resolution.x / resolution.y;
        m_current_ratio = Camera.main.aspect;   // 카메라 비율 == 캔버스 비율
        m_canvas_size = new Size((int)m_main_canvas.pixelRect.width,
                                        (int)m_main_canvas.pixelRect.height);
        m_canvas_sizeDelta = new Vector2(m_main_canvas_rt.rect.width, m_main_canvas_rt.rect.height);

        //m_canvas_sizeDelta =

        // 허용 범위 내이면 동일한 것으로 판단함
        float _diff = m_current_ratio - m_target_ratio;
        if (_diff < m_tolerance && _diff > -m_tolerance)
        {
            m_main_panel_size = new Size(m_canvas_size.width, m_canvas_size.height);
            m_main_panel_sizeDelta = new Vector2(m_canvas_sizeDelta.x, m_canvas_sizeDelta.y);

            m_UISpaceType = E_UISpaceType.Equal;
            OnChangedAspectCallBack?.Invoke();
            return;
        }


        if (m_current_ratio > m_target_ratio)
        {   // 현재 view 가 target resolution 보다 좌우로 더 긴 경우
            m_UISpaceType = E_UISpaceType.PillarBox;

            float _width = m_canvas_size.width * (m_target_ratio / m_current_ratio);
            float _height = m_canvas_size.height;

            m_main_panel_size = new Size((int)_width, (int)_height);            
            float _delta_width = m_canvas_sizeDelta.x * (m_target_ratio / m_current_ratio);
            float _delta_height = m_canvas_sizeDelta.y;

            m_main_panel_sizeDelta = new Vector2(_delta_width, _delta_height);
            
            OnChangedAspectCallBack?.Invoke();
        }
        else
        {   // 상하로 더 긴 경우
            m_UISpaceType = E_UISpaceType.LetterBox;

            float _width = m_canvas_size.width;
            float _height = m_canvas_size.height * (m_current_ratio / m_target_ratio);

            m_main_panel_size = new Size((int)_width, (int)_height);
          
            float _delta_width = m_canvas_sizeDelta.x;
            float _delta_height = m_canvas_sizeDelta.y * (m_current_ratio / m_target_ratio);
            m_main_panel_sizeDelta = new Vector2(_delta_width, _delta_height);
           
            OnChangedAspectCallBack?.Invoke();
        }

        //Debug.LogFormat("panel size : {0} , {1}", m_main_panel_size.width, m_main_panel_size.height);
        //Debug.LogFormat("Left Top : {0} , {1}", m_main_panel_leftTop_position.x, m_main_panel_leftTop_position.y);
    }


    public delegate void OnChangedAspect();

    public OnChangedAspect OnChangedAspectCallBack;
}
