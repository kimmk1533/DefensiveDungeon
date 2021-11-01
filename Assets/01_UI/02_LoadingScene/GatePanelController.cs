using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatePanelController : MonoBehaviour
{
    [SerializeField] SceneLoader m_sceneLoader;

    [SerializeField] Image m_leftPanel;
    [SerializeField] Image m_rightPanel;
    [SerializeField] Image m_eyesPanel;

    [SerializeField] float m_speed;

    float m_leftWidth;
    float m_rightWidth;
    RectTransform left_rt;
    RectTransform right_rt;

    private void Awake()
    {
        m_eyesPanel.gameObject.SetActive(false);

        left_rt = m_leftPanel.transform as RectTransform;
        right_rt = m_rightPanel.transform as RectTransform;

        m_leftWidth = left_rt.sizeDelta.x;
        m_rightWidth = right_rt.sizeDelta.x;

        left_rt.anchoredPosition = new Vector2(0, 0);
        right_rt.anchoredPosition = new Vector2(0, 0);
    }

    private void Start()
    {
        // start gate open
        StartCoroutine(Co_CloseGates(OnCloseGateEnd));
    }

    // when scene start...
    IEnumerator Co_CloseGates(Action callback)
    {
        while (true)
        {
            if (m_leftWidth - left_rt.anchoredPosition.x < 5f
                && -left_rt.anchoredPosition.x - m_rightWidth < 5f)
            {
                left_rt.anchoredPosition3D = new Vector3(m_leftWidth, 0, 0);
                right_rt.anchoredPosition3D = new Vector3(-m_rightWidth, 0, 0);


                // gate open complete
                // call callback funtion
                callback?.Invoke();

                break;
            }

            left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(m_leftWidth, 0, 0), Time.deltaTime * m_speed);
            right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(-m_rightWidth, 0, 0), Time.deltaTime * m_speed);

            yield return null;
        }
    }

    public void OnCloseGateEnd()
    {
        m_eyesPanel.gameObject.SetActive(true);

        // scene load 
        SceneLoader.Instance.LoadProcess(OnSceneLoadComplete);
    }

    // scene load complete callback
    public void OnSceneLoadComplete()
    {
        SceneLoader.Instance.UnloadScenes();
        CreateGatesPanelForOpenProcess();
        DeActivateCloseGates();

        // gate open process
        StartCoroutine(Co_OpenGates(OnOpenGatesEnd));
    }

    // create gates panel to loaded new scene's main UI canvas
    private void CreateGatesPanelForOpenProcess()
    {
        Image copied_left_gate = GameObject.Instantiate<Image>(m_leftPanel);
        Image copied_right_gate = GameObject.Instantiate<Image>(m_rightPanel);

        Canvas mainCanvasOfLoadedUIScene = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        Transform mainPanelOfLoadedUIScene = null;
        for (int i = 0; i < mainCanvasOfLoadedUIScene.transform.childCount; i++)
        {
            var child = mainCanvasOfLoadedUIScene.transform.GetChild(i);
            if (child.tag == "MainPanel")
            {
                mainPanelOfLoadedUIScene = child;
            }
        }

        copied_left_gate.transform.SetParent(mainCanvasOfLoadedUIScene.transform);
        copied_right_gate.transform.SetParent(mainCanvasOfLoadedUIScene.transform);

        left_rt = copied_left_gate.GetComponent<RectTransform>();
        right_rt = copied_right_gate.GetComponentInParent<RectTransform>();

        // anckor pos : canvas' left / top ~ bottom
        // pivot : this panel's right middle
        left_rt.anchorMin = new Vector2(0, 0);
        left_rt.anchorMax = new Vector2(0, 1);
        left_rt.pivot = new Vector2(1, 0.5f);

        // anckor pos : canvas' right / top ~ bottom
        // pivot : this panel's left middle
        right_rt.anchorMin = new Vector2(1, 0);
        right_rt.anchorMax = new Vector2(1, 1);
        right_rt.pivot = new Vector2(0, 0.5f);

        // set size
        float panel_width = Screen.width / 2.0f;
        left_rt.sizeDelta = new Vector2(panel_width, 0);
        //left_rt.offsetMin
        right_rt.sizeDelta = new Vector2(panel_width, 0);

        // set position
        left_rt.anchoredPosition = new Vector2(panel_width, 0);
        right_rt.anchoredPosition = new Vector2(-panel_width, 0);

        // activate
        left_rt.gameObject.SetActive(true);
        right_rt.gameObject.SetActive(true);

        left_rt.SetAsLastSibling();
        right_rt.SetAsLastSibling();

        mainPanelOfLoadedUIScene.gameObject.SetActive(true);
    }

    private void DeActivateCloseGates()
    {
        m_eyesPanel.gameObject.SetActive(false);
        m_leftPanel.gameObject.SetActive(false);
        m_rightPanel.gameObject.SetActive(false);
    }

    IEnumerator Co_OpenGates(Action callback)
    {
        while (true)
        {
            if (left_rt.anchoredPosition.x < 5f
              && -right_rt.anchoredPosition.x < 5f)
            {
                left_rt.anchoredPosition = Vector2.zero;
                right_rt.anchoredPosition = Vector2.zero;

                callback?.Invoke();
                break;
            }

            left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.deltaTime * m_speed);
            right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.deltaTime * m_speed);
            yield return null;
        }
    }

    private void OnOpenGatesEnd()
    {
        Debug.Log("Gate Process Complete!!");
        SceneLoader.Instance.OnAllProcessCompleted();
    }

}
