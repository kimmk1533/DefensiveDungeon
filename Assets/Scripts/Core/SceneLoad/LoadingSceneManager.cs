using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    [SerializeField]
    Image progressBar;
    AsyncOperation op;

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadSceneProcess()
    {
        yield return null;

        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.1f)
            {
                StartCoroutine(WaitTime(5f, timer));
            }

            else if (op.progress < 0.3f)
            {
                StartCoroutine(WaitTime(5f, timer));
            }

            else if (op.progress < 0.5f)
            { 
                StartCoroutine(WaitTime(5f, timer));
            }

            else if (op.progress < 0.9f)
            {
                StartCoroutine(WaitTime(5f, timer));

                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }

            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount >= 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    IEnumerator WaitTime(float time, float timer)
    {
        yield return new WaitForSeconds(time);

        progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
    }

    //void Update()
    //{
    //    if (coroutine == null)
    //    {
    //        coroutine = LoadSceneCoroutine();
    //    }

    //    if (coroutine != null)
    //    {
    //        if (coroutine.MoveNext())
    //        {
    //            if (_subsceneAsyncOperation.progress < 0.9f)
    //            {
    //                float loadingAmount = _subsceneAsyncOperation.progress / 0.9f;

    //                Debug.Log(loadingAmount);

    //                progressBar.fillAmount = loadingAmount;
    //            }
    //            else if (_subsceneAsyncOperation.progress < 1.0f)
    //            {
    //                activationPercentage += activationPerFrame * Time.deltaTime;
    //                activationPercentage = Mathf.Min(activationPercentage, 1f);

    //                progressBar.fillAmount = activationPercentage;
    //            }
    //        }
    //        else
    //        {
    //            coroutine = null;
    //            activationPercentage = 0f;
    //        }
    //    }

        //// Start coroutine to load next scene
        //if (_loadSceneEnumerator == null)
        //{
        //    _loadSceneEnumerator = LoadSceneCoroutine();
        //    _activationPercentage = 0;
        //}

        //if (_loadSceneEnumerator != null)
        //{
        //    // Continue coroutine
        //    if (_loadSceneEnumerator.MoveNext())
        //    {
        //        // Loading phase
        //        if (_subsceneAsyncOperation.progress < 0.9f)
        //        {
        //            _loadingPercentage = _subsceneAsyncOperation.progress / 0.9f;

        //            UpdateSceneLoadingSlider(_loadingPercentage);
        //        }
        //        // Activation phase
        //        else if (_subsceneAsyncOperation.progress < 1.0f)
        //        {
        //            _activationPercentage += activatePhaseIncrementPerFrame * Time.deltaTime;
        //            _activationPercentage = Mathf.Min(_activationPercentage, 1);

        //            UpdateSceneLoadingSlider(_activationPercentage);
        //        }
        //    }
        //    // Scene fully loaded
        //    else
        //    {
        //        _loadSceneEnumerator = null;
        //        _currentSceneIndex++;
        //        UpdateTotalLoadingSlider(++_loadedScenes, _numberScenes);

        //        _yieldedCount = 0;
        //        _activationPercentage = 0;
        //    }
        //}
    //}

    #region Methods

    //private void UpdateSceneLoadingSlider(float progress)
    //{
    //    progressBar.fillAmount = progress;
    //    // sceneLoadingSlider.value = progress;
    //    // sceneProgressText.text = string.Format("{0}{1}", ((int)(progress * 100)), "%");
    //}

    //private void UpdateTotalLoadingSlider(float loadedScenes, float numberScenes)
    //{
    //    var percentageLoaded = loadedScenes / numberScenes;

    //    progressBar.fillAmount = percentageLoaded;
    //    //totalLoadingSlider.value = percentageLoaded;
    //    //totalProgressText.text = string.Format("{0}{1}", (int)(percentageLoaded * 100), "%");
    //}

#endregion

    #region
    //private IEnumerator LoadSceneCoroutine()
    //{
    //    _subsceneAsyncOperation = SceneManager.LoadSceneAsync(nextScene);
    //    _subsceneAsyncOperation.allowSceneActivation = false;

    //    while (_subsceneAsyncOperation.progress < 0.9f)
    //    {
    //        yield return null;
    //    }

    //    _subsceneAsyncOperation.allowSceneActivation = true;

    //    while (_subsceneAsyncOperation.progress < 1.0f)
    //    {
    //        yield return null;
    //    }

    //    yield return null;
    //}

    #endregion
}