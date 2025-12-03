using System;
using System.Collections.Generic;

// Клас, що відповідає спрощеній структурі нашого JSON-файлу
[Serializable]
public class GameStringData
{
    // Єдиний словник, який безпосередньо відповідає структурі JSON
    public Dictionary<string, string> strings;

    /// Отримує текстове значення за ключем.
    public string GetString(string key)
    {
        // Перевіряємо, чи ініціалізовано словник та чи містить він ключ.
        if (strings != null && strings.ContainsKey(key))
        {
            return strings[key];
        }
        else
        {
            // У разі відсутності ключа виводимо помилку та повертаємо заглушку.
            UnityEngine.Debug.LogError($"String key not found: {key}");
            return $"!!MISSING_TEXT:{key}!!";
        }
    }
}