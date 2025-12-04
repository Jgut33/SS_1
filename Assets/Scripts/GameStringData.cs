using System;
using System.Collections.Generic;
using UnityEngine;

// Клас, що представляє одну пару Key-Value для серіалізації
[Serializable]
public class StringEntry
{
    public string key;    // Поле для ключа
    public string value;  // Поле для тексту
}

// Головний клас для десеріалізації JSON
[Serializable]
public class GameStringData
{
    // !!! КРИТИЧНО: JsonUtility ПІДТРИМУЄ List<T>, АЛЕ НЕ Dictionary<K, V> !!!
    // Тому ми використовуємо List для десеріалізації:
    public List<StringEntry> entries = new List<StringEntry>();

    // Словник для швидкого пошуку в коді (заповнюється вручну після десеріалізації)
    [NonSerialized]
    private Dictionary<string, string> stringMap = new Dictionary<string, string>();

    /// Обов'язково викликайте цей метод після JsonUtility.FromJson,
    /// щоб перетворити List у Dictionary для швидкого пошуку.
    public void BuildMap()
    {
        stringMap.Clear();
        foreach (var entry in entries)
        {
            if (!stringMap.ContainsKey(entry.key))
            {
                // Додаємо пару ключ-значення
                stringMap.Add(entry.key, entry.value);
            }
            else
            {
                Debug.LogWarning($"Duplicate string key found: {entry.key}. Skipping duplicate.");
            }
        }
        Debug.Log($"String Map built with {stringMap.Count} entries.");
    }

    /// Отримує текстове значення за ключем, використовуючи внутрішній словник.
    public string GetString(string key)
    {
        if (stringMap.ContainsKey(key))
        {
            return stringMap[key];
        }
        else
        {
            UnityEngine.Debug.LogError($"String key not found: {key}");
            return $"!!MISSING_TEXT:{key}!!";
        }
    }
}