using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : MonoBehaviour
{
    [SerializeField] SceneLoader SceneLoader;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            List<string> loadScenes = new List<string>();
            List<string> unloadscenes = new List<string>();

            loadScenes.Add("SceneLoadTestScene");
            loadScenes.Add("SceneLoadTestScene1");
            loadScenes.Add("SceneLoadTestScene2");

            //unloadscenes.Add("LoaderScene");

            SceneLoader.SetLoadingSceneInfomation(loadScenes, unloadscenes);
        }
    }
}
