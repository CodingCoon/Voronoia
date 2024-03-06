using System;
using System.Collections.Generic;

public static class DictionaryExtension
{
    public static void AddIfAbsent<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new List<TValue>());
        }
        dictionary[key].Add(value);
    }

    public static TValue GetIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        else
        {
            TValue value = valueCreator.Invoke();
            dictionary.Add(key, value);
            return value;
        }
    }


    public static void AddValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (value != null)
        {
            dictionary[key] = value;
        }
    }

    // Adds only to the dictionary if the value is not null
    public static void AddValue<TValue>(this List<TValue> list, TValue value)
    {
        if (value != null)
        {
            list.Add(value);
        }
    }

    public static void AddValue<TValue>(this List<TValue> list, IEnumerable<TValue> values)
    {
        foreach (TValue value in values)
        {
            list.AddValue(value);
        }
    }

    public static TValue GetRandom<TValue>(this List<TValue> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    public static void RemoveValue<TValue>(this List<TValue> list, TValue value)
    {
        if (value != null)
        {
            if (list.Contains(value))
            {
                list.Remove(value);
            }
            else
            {
                throw new System.Exception("no value to remove");
            }
        }
    }

    public static void RemoveValue<TValue>(this List<TValue> list, IEnumerable<TValue> values)
    {
        foreach (TValue value in values)
        {
            if (value != null)
            {
                if (list.Contains(value))
                {
                    list.Remove(value);
                }
                else
                {
                    throw new System.Exception("no value to remove");
                }
            }
        }
    }
}
