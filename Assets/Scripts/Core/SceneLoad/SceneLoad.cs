using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad : MonoBehaviour
{
    public string loadscene;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadingSceneManager.LoadScene(loadscene);
        }
    }

    void e(object sender, EventArgs e)
    {

    }
}
