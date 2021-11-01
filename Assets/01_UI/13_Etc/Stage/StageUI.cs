using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_stage_textprro;
    [Space(10)]
    [SerializeField] Image m_radialImage;
    [SerializeField] TMPro.TextMeshProUGUI m_time_textpro;

    
    private void Start()
    {
        StageInfoManager.Instance.OnTimeChangedEvent += OnCountChanged;
        StageInfoManager.Instance.OnStageChangedEvent += OnStageChanged;
    }

    public void OnStageChanged(StageChangedEventArgs args)
    {
        m_stage_textprro.text = args.stageName;
    }
    public void OnCountChanged(float remainTime, float progress)
    {
        int val = (int)remainTime;

        m_time_textpro.text = val.ToString();
        m_radialImage.fillAmount = progress;
    }
}
