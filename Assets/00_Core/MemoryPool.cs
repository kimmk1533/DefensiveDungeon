using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool<T> : System.IDisposable where T : MonoBehaviour
{
    // 풀에 담을 원본
    private T original;
    // 오브젝트들을 담을 실제 풀
    private Queue<T> queue = new Queue<T>();
    // 생성한 오브젝트를 기억하고 있다가 디스폰 시 확인할 리스트
    private List<T> despawnList = new List<T>();
    // 초기 풀 사이즈
    private int poolSize;
    // 하이어라키 창에서 관리하기 쉽도록 parent 지정
    private Transform parent = null;

    public int preLoadedPoolSize = 1000;

    // 부모 지정 안하고 생성하는 경우
    public MemoryPool(T _original, int _poolsize)
    {
        original = _original;
        poolSize = _poolsize;

        for (int i = 0; i < poolSize; i++)
        {
            T newItem = GameObject.Instantiate<T>(original);
            newItem.gameObject.SetActive(false);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }
    }
    // 부모 지정하여 생성하는 경우
    public MemoryPool(T _original, int _poolSize, Transform parent)
    {
        original = _original;
        poolSize = _poolSize;
        this.parent = parent;

        for (int i = 0; i < Mathf.Min(poolSize, preLoadedPoolSize); i++)
        {
            T newItem = GameObject.Instantiate<T>(original);  //new
            string[] strs = newItem.name.Split('(');
            newItem.name = strs[0];
            newItem.transform.SetParent(parent);
            newItem.gameObject.SetActive(false);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }


        //if (poolSize > preLoadedPoolSize)
        //{
        //    StartCoroutine(MakePool());
        //}
    }

    IEnumerator MakePool()
    {
        for (int i = preLoadedPoolSize; i < poolSize; ++i)
        {
            T newItem = GameObject.Instantiate<T>(original);
            string[] strs = newItem.name.Split('(');
            newItem.name = strs[0];
            newItem.gameObject.SetActive(false);
            newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
        }

        yield return new WaitForSeconds(0.1f);
    }

    // foreach 문을 위한 반복자
    public IEnumerator GetEnumerator()
    {
        foreach (T item in queue)
            yield return item;
    }

    // 오브젝트 풀이 빌 경우 선택적으로 call
    // 절반만큼 증가
    void ExpandPoolSize()
    {
        int newSize = poolSize + (int)(poolSize * 0.5f);
        for (int i = poolSize; i < newSize; i++)
        {
            T newItem = GameObject.Instantiate<T>(original);
            newItem.gameObject.SetActive(false);
            if (parent != null)
                newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }
        poolSize = newSize;
    }

    // 모든 오브젝트 사용시 추가로 생성할 경우 
    // expand 를 true 로 설정
    public T Spawn(bool expand = true)
    {
        if (expand && queue.Count <= 0)
        {
            ExpandPoolSize();
        }

        if (queue.Count > 0)
        {
            T item = queue.Dequeue();
            return item;
        }

        Debug.LogWarning("Pool Size Over");
        return null;
    }

    // 회수 작업
    public void DeSpawn(T obj)
    {
        if (obj == null ||
            !despawnList.Contains(obj))
            return;

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        queue.Enqueue(obj);
    }

    // 메모리 해제
    public void Dispose()
    {
        foreach (T item in queue)
        {
            GameObject.DestroyImmediate(item);
        }
        queue.Clear();
        queue = null;
        despawnList.Clear();
        despawnList = null;
    }
}

public class MemoryPool : System.IDisposable
{
    // 풀에 담을 원본
    private GameObject original;
    // 오브젝트들을 담을 실제 풀
    private Queue<GameObject> queue = new Queue<GameObject>();
    // 생성한 오브젝트를 기억하고 있다가 디스폰 시 확인할 리스트
    private List<GameObject> despawnList = new List<GameObject>();
    // 초기 풀 사이즈
    private int poolSize;
    // 하이어라키 창에서 관리하기 쉽도록 parent 지정
    private Transform parent = null;

    public int preLoadedPoolSize = 1000;

    // 부모 지정 안하고 생성하는 경우
    public MemoryPool(GameObject _original, int _poolsize)
    {
        original = _original;
        poolSize = _poolsize;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newItem = GameObject.Instantiate<GameObject>(original);
            newItem.gameObject.SetActive(false);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }
    }
    // 부모 지정하여 생성하는 경우
    public MemoryPool(GameObject _original, int _poolSize, Transform parent)
    {
        original = _original;
        poolSize = _poolSize;
        this.parent = parent;

        for (int i = 0; i < Mathf.Min(poolSize, preLoadedPoolSize); i++)
        {
            GameObject newItem = GameObject.Instantiate<GameObject>(original);  //new
            string[] strs = newItem.name.Split('(');
            newItem.name = strs[0];
            newItem.transform.SetParent(parent);
            newItem.gameObject.SetActive(false);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }


        //if (poolSize > preLoadedPoolSize)
        //{
        //    StartCoroutine(MakePool());
        //}
    }

    IEnumerator MakePool()
    {
        for (int i = preLoadedPoolSize; i < poolSize; ++i)
        {
            GameObject newItem = GameObject.Instantiate<GameObject>(original);
            string[] strs = newItem.name.Split('(');
            newItem.name = strs[0];
            newItem.gameObject.SetActive(false);
            newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
        }

        yield return new WaitForSeconds(0.1f);
    }

    // foreach 문을 위한 반복자
    public IEnumerator GetEnumerator()
    {
        foreach (GameObject item in queue)
            yield return item;
    }

    // 오브젝트 풀이 빌 경우 선택적으로 call
    // 절반만큼 증가
    void ExpandPoolSize()
    {
        int newSize = poolSize + (int)(poolSize * 0.5f);
        for (int i = poolSize; i < newSize; i++)
        {
            GameObject newItem = GameObject.Instantiate<GameObject>(original);
            newItem.gameObject.SetActive(false);
            if (parent != null)
                newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }
        poolSize = newSize;
    }

    // 모든 오브젝트 사용시 추가로 생성할 경우 
    // expand 를 true 로 설정
    public GameObject Spawn(bool expand = true)
    {
        if (expand && queue.Count <= 0)
        {
            ExpandPoolSize();
        }

        if (queue.Count > 0)
        {
            GameObject item = queue.Dequeue();
            return item;
        }

        Debug.LogWarning("Pool Size Over");
        return null;
    }

    // 회수 작업
    public void DeSpawn(GameObject obj)
    {
        if (obj == null ||
            !despawnList.Contains(obj))
            return;

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        queue.Enqueue(obj);
    }

    // 메모리 해제
    public void Dispose()
    {
        foreach (GameObject item in queue)
        {
            GameObject.DestroyImmediate(item);
        }
        queue.Clear();
        queue = null;
        despawnList.Clear();
        despawnList = null;
    }
}