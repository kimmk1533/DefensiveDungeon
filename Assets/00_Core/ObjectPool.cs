using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-97)]
public abstract class ObjectPool<Pool, Origin> : Singleton<Pool> where Pool : MonoBehaviour where Origin : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 100;

    public Dictionary<string, Origin> m_Origins = null;
    protected Dictionary<string, MemoryPool<Origin>> m_Pools = null;

    protected ResourcesManager M_Resources => ResourcesManager.Instance;

    protected virtual void Awake()
    {
        __Initialize();
    }
    protected virtual void OnApplicationQuit()
    {
        __Finalize();
    }

    public virtual void __Initialize()
    {
        m_Origins = new Dictionary<string, Origin>();
        m_Pools = new Dictionary<string, MemoryPool<Origin>>();

        //for (int i = 0; i < m_Origins.Count; ++i)
        //{
        //    GameObject parent = new GameObject();
        //    parent.transform.SetParent(this.transform);
        //    parent.name = m_Origins[i].name + "_parent";

        //    m_Pools.Add(m_Origins[i].name, new MemoryPool(m_Origins[i], m_PoolSize, parent.transform));
        //}
    }
    public virtual void __Finalize()
    {
        if (m_Pools == null)
            return;

        foreach (var item in m_Pools)
        {
            item.Value?.Dispose();
        }

        m_Pools.Clear();
        m_Pools = null;
    }

    protected bool AddPool(string key, Origin origin, Transform parent)
    {
        if (m_Origins.ContainsKey(key))
            return false;

        m_Origins.Add(key, origin);

        GameObject Parent = new GameObject();
        Parent.name = origin.name;
        Parent.transform.SetParent(parent);
        origin.transform.SetParent(Parent.transform);

        m_Pools.Add(key, new MemoryPool<Origin>(origin, m_PoolSize, Parent.transform));

        origin.name += "_Origin";

        return true;
    }
    public virtual MemoryPool<Origin> GetPool(string key)
    {
        if (key == null)
            return null;

        if (m_Pools.ContainsKey(key))
            return m_Pools[key];

        return null;
    }

    //public abstract GameObject Spawn();
    //public abstract void Despawn(GameObject obj);
}

[DefaultExecutionOrder(-97)]
public abstract class ObjectPool<Pool> : Singleton<Pool> where Pool : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 100;

    public Dictionary<string, GameObject> m_Origins = null;
    protected Dictionary<string, MemoryPool> m_Pools = null;

//#if UNITY_EDITOR
//    [ReadOnly]
//    public DebugDictionary<string, GameObject> m_DebugOrigin = null;
//#endif

    protected ResourcesManager M_Resources => ResourcesManager.Instance;

    protected virtual void Awake()
    {
        __Initialize();
    }
    protected virtual void OnApplicationQuit()
    {
        __Finalize();
    }

    public virtual void __Initialize()
    {
        m_Origins = new Dictionary<string, GameObject>();
        m_Pools = new Dictionary<string, MemoryPool>();

//#if UNITY_EDITOR
//        m_DebugOrigin = new DebugDictionary<string, GameObject>();
//#endif

        //for (int i = 0; i < m_Origins.Count; ++i)
        //{
        //    GameObject parent = new GameObject();
        //    parent.transform.SetParent(this.transform);
        //    parent.name = m_Origins[i].name + "_parent";

        //    m_Pools.Add(m_Origins[i].name, new MemoryPool(m_Origins[i], m_PoolSize, parent.transform));
        //}
    }
    public virtual void __Finalize()
    {
        if (m_Pools == null)
            return;

        foreach (var item in m_Pools)
        {
            item.Value?.Dispose();
        }

        m_Pools.Clear();
        m_Pools = null;
    }

    protected bool AddPool(string key, GameObject origin, Transform parent)
    {
        if (m_Origins.ContainsKey(key))
            return false;

        m_Origins.Add(key, origin);

        GameObject Parent = new GameObject();
        Parent.name = origin.name;
        Parent.transform.SetParent(parent);
        origin.transform.SetParent(Parent.transform);

        m_Pools.Add(key, new MemoryPool(origin, m_PoolSize, Parent.transform));

        origin.name += "_Origin";

//#if UNITY_EDITOR
//        m_DebugOrigin.Add(key, origin);
//#endif
        return true;
    }
    public virtual MemoryPool GetPool(string key)
    {
        if (key == null)
            return null;

        if (m_Pools.ContainsKey(key))
            return m_Pools[key];

        return null;
    }

    //public abstract GameObject Spawn();
    //public abstract void Despawn(GameObject obj);
}
