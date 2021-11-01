using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인스펙터에 보이기 위한 디버깅용 딕셔너리
/// </summary>
/// <typeparam name="TKey">키</typeparam>
/// <typeparam name="TValue">값</typeparam>
[System.Serializable]
public class DebugDictionary<TKey, TValue>
{
    [SerializeField, NonReorderable]
    List<TKey> m_Keys;
    [SerializeField, NonReorderable]
    List<TValue> m_Values;

    public DebugDictionary()
    {
        m_Keys = new List<TKey>();
        m_Values = new List<TValue>();
    }
    public DebugDictionary(Dictionary<TKey, TValue> dictionary)
    {
        m_Keys = new List<TKey>(dictionary.Keys);
        m_Values = new List<TValue>(dictionary.Values);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!m_Keys.Contains(key))
                return default(TValue);

            int index = m_Keys.IndexOf(key);

            return m_Values[index];
        }
        set
        {
            if (!m_Keys.Contains(key))
                return;

            int index = m_Keys.IndexOf(key);

            m_Values[index] = value;
        }
    }
    public int Count
    {
        get
        {
            return Mathf.Min(m_Keys.Count, m_Values.Count);
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (m_Keys.Contains(key))
        {
            Debug.LogError(
                string.Format(
                    "DebugDictionary Add({0}, {1}) : 딕셔너리에 동일한 키 값이 존재합니다.",
                    key, value)
                );
            return;
        }

        m_Keys.Add(key);
        m_Values.Add(value);
    }
    public void Clear()
    {
        m_Keys.Clear();
        m_Values.Clear();
    }
    public bool ContainsKey(TKey key)
    {
        return m_Keys.Contains(key);
    }
    public bool ContainsValue(TValue value)
    {
        return m_Values.Contains(value);
    }
    public bool Remove(TKey key)
    {
        if (!m_Keys.Contains(key))
            return false;

        int index = m_Keys.IndexOf(key);

        m_Values.RemoveAt(index);

        return true;
    }
}
