using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField]
    protected List<ResourcesType> m_TypeInfos;
    //                   타입               경로                이름
    protected Dictionary<string, Dictionary<string, Dictionary<string, GameObject>>> m_GameObjects;
    protected Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>> m_Sprites;
    protected Dictionary<string, Dictionary<string, Dictionary<string, ScriptableObject>>> m_ScriptableObjects;

#if UNITY_EDITOR
    [ReadOnly]
    public DebugDictionary<string, GameObject> Debug_GameObjects;
    [ReadOnly]
    public DebugDictionary<string, Sprite> Debug_Sprites;
    [ReadOnly]
    public DebugDictionary<string, ScriptableObject> Debug_ScriptableObjects;
#endif

    private void Awake()
    {
        m_GameObjects = new Dictionary<string, Dictionary<string, Dictionary<string, GameObject>>>();
        m_Sprites = new Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>();
        m_ScriptableObjects = new Dictionary<string, Dictionary<string, Dictionary<string, ScriptableObject>>>();

#if UNITY_EDITOR
        Debug_GameObjects = new DebugDictionary<string, GameObject>();
        Debug_Sprites = new DebugDictionary<string, Sprite>();
        Debug_ScriptableObjects = new DebugDictionary<string, ScriptableObject>();
#endif

        for (int i = 0; i < m_TypeInfos.Count; ++i)
        {
            LoadAll(m_TypeInfos[i]);
        }
    }

    private void LoadAll(ResourcesType type)
    {
        string path = type.path;

        if (!path.EndsWith("/"))
        {
            path += "/";
        }

        if (type.resourceType.HasFlag(E_ResourcesType.GameObject))
        {
            GameObject[] gameObjects = Resources.LoadAll<GameObject>(path);
            Dictionary<string, Dictionary<string, GameObject>> d_path = new Dictionary<string, Dictionary<string, GameObject>>();
            Dictionary<string, GameObject> d_name = new Dictionary<string, GameObject>();

            foreach (var item in gameObjects)
            {
                d_name.Add(item.name, item);

#if UNITY_EDITOR
                Debug_GameObjects.Add(item.name, item);
#endif
            }

            d_path.Add(type.path, d_name);

            m_GameObjects.Add(type.type, d_path);
        }
        if (type.resourceType.HasFlag(E_ResourcesType.Sprite))
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            string key = sprites[0].name.Split('_')[0];
            Dictionary<string, Dictionary<string, Sprite[]>> d_path = new Dictionary<string, Dictionary<string, Sprite[]>>();
            Dictionary<string, Sprite[]> d_name = new Dictionary<string, Sprite[]>();
            List<Sprite> tempList = new List<Sprite>();

            foreach (var item in sprites)
            {
                if (key != item.name.Split('_')[0])
                {
                    d_name.Add(key, tempList.ToArray());
                    tempList.Clear();
                    key = item.name.Split('_')[0];
                }

                tempList.Add(item);

#if UNITY_EDITOR
                Debug_Sprites.Add(item.name, item);
#endif
            }

            d_name.Add(key, tempList.ToArray());

            d_path.Add(type.path, d_name);

            m_Sprites.Add(type.type, d_path);
        }
        if (type.resourceType.HasFlag(E_ResourcesType.ScriptableObject))
        {
            ScriptableObject[] scriptableObjects = Resources.LoadAll<ScriptableObject>(path);
            Dictionary<string, Dictionary<string, ScriptableObject>> d_path = new Dictionary<string, Dictionary<string, ScriptableObject>>();
            Dictionary<string, ScriptableObject> d_name = new Dictionary<string, ScriptableObject>();

            foreach (var item in scriptableObjects)
            {
                d_name.Add(item.name, item);

#if UNITY_EDITOR
                Debug_ScriptableObjects.Add(item.name, item);
#endif
            }

            d_path.Add(type.path, d_name);

            m_ScriptableObjects.Add(type.type, d_path);
        }
    }

    public GameObject GetGameObject(string type, string name)
    {
        foreach (var item in m_GameObjects[type])
        {
            if (item.Value.ContainsKey(name))
                return item.Value[name];
        }

        return null;
    }
    public T GetGameObject<T>(string type, string name) where T : MonoBehaviour
    {
        foreach (var item in m_GameObjects[type])
        {
            if (item.Value.ContainsKey(name))
                return item.Value[name].GetComponent<T>();
        }

        return null;
    }
    public Sprite[] GetSprites(string type, string name)
    {
        foreach (var item in m_Sprites[type])
        {
            if (item.Value.ContainsKey(name))
                return item.Value[name];
        }

        return null;
    }
    public T GetScriptableObject<T>(string type, string name) where T : ScriptableObject
    {
        foreach (var item in m_ScriptableObjects[type])
        {
            if (item.Value.ContainsKey(name))
                return item.Value[name] as T;
        }

        return null;
    }

    [System.Flags]
    public enum E_ResourcesType
    {
        None,
        GameObject = 1 << 0,
        Sprite = 1 << 1,
        ScriptableObject = 1 << 2,
    }

    [System.Serializable]
    public struct ResourcesType
    {
        public string type;
        public string path;
        public E_ResourcesType resourceType;
    }
}
