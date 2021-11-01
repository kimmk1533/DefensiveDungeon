using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneStartEventReciever : MonoBehaviour
{
    public UnityEvent m_scene_start_event;

    private void Awake()
    {
        this.tag = "SceneStart";
    }

    public void __Start()
    {
        Debug.Log("asdfasdf");
        m_scene_start_event?.Invoke();
    }
}
