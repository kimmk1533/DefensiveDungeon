using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene_CharacterPanelButtonController : MonoBehaviour
{
    [SerializeField] Image m_main_button_panel;

    // 선택 버튼 관련된 것은 character select manager 를 참조

    public void __OnBackButton()
    {        
        m_main_button_panel.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void __OnSelectCompleteButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        button.enabled = false;
        StartCoroutine(LoadLoadingScene());
    }

    IEnumerator LoadLoadingScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LoaderScene", LoadSceneMode.Additive);
        //op.allowSceneActivation = false;    // 완료된 씬이 바로 로드되지 않도록 함

        float beforeProgress = 0.0f;
        while (!op.isDone)
        {
            yield return new WaitForSeconds(0.5f);

            //m_progressBar.value -= beforeProgress;
            //m_progressBar.value += op.progress;
            beforeProgress = op.progress;

            if (op.progress >= 1.0f)
            {
                break;
            }
        }


        // 다음 씬으로 이동 할 것 (로딩 씬 부르기)
        List<string> loadScenes = new List<string>();
        List<string> unloadScenes = new List<string>();

        loadScenes.Add("Map");
        loadScenes.Add("UIScene");
        unloadScenes.Add("MainStartScene");

        var obj = GameObject.FindWithTag("SceneLoader");
        var sceneLoader = obj.GetComponent<SceneLoader>();
        sceneLoader.SetLoadingSceneInfomation(loadScenes, unloadScenes);

    }
}
