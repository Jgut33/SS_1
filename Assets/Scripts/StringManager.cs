using UnityEngine;

public class StringManager : MonoBehaviour
{
    public static StringManager Instance;

    // Тут буде зберігатися ваш клас із текстами
    public GameStringData gameStrings;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // !!! НЕ ЗНИЩУВАТИ ПРИ ЗМІНІ СЦЕНИ !!!
            DontDestroyOnLoad(gameObject);

            // 1. Завантажити тут, при першому запуску
            LoadStrings(Const.StringDataFileName_UA);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadStrings(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            gameStrings = JsonUtility.FromJson<GameStringData>(jsonFile.text);

            if (gameStrings != null)
            {
                // !!! НОВИЙ ВИКЛИК: Перетворюємо List у Dictionary !!!
                gameStrings.BuildMap();
                Debug.Log($"Strings loaded successfully from {fileName}.txt.");
            }
        }
        else
        {
            Debug.LogError($"String data file not found in Resources: {fileName}.");
        }
    }

    // Зручний метод для доступу до тексту
    public string GetString(string key)
    {
        if (gameStrings == null) return "Error: Strings not loaded!";
        return gameStrings.GetString(key);
    }
}