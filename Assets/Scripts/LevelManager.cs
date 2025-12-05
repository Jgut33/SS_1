using UnityEngine;
using UnityEngine.SceneManagement; // Потрібно для LoadScene
using UnityEngine.UI;
using System.Collections; // Потрібно ДЛЯ РОБОТИ З COROUTINES

public class LevelManager : MonoBehaviour
{
    // 1. Singleton: Статичний екземпляр класу
    public static LevelManager Instance;

    [Header("Zoom UI")]
    [Tooltip("Головний батьківський об'єкт, який містить усі елементи зуму (спрайт, зони взаємодії).")]
    public GameObject zoomContainer;

    // ЗМІННІ ДЛЯ ЗАТЕМНЕННЯ (FADE)
    [Header("Scene Transition Settings")]
    [Tooltip("UI Image, який використовується як затемнення (FaderPanel).")]
    public Image faderPanel;

    [Tooltip("Час (у секундах), за який відбувається затемнення до чорного.")]
    public float fadeDuration = 0.5f;

    [Header("Data Management")]
    public GameStringData gameStrings;

    // Глобальний стан для перетягування (Drag & Drop)
    [HideInInspector] public bool IsDragging = false;
    private string currentlyDraggedItem = "";

    void Awake()
    {
        // Перевірка та ініціалізація Singleton
        if (Instance == null)
        {
            Instance = this;
            // Optionally: DontDestroyOnLoad(gameObject); якщо менеджер має бути на всіх сценах
            LoadStrings("strings_ua");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Одразу після того, як сцена завантажилася, і Singleton LevelManager ініціалізувався:
        if (faderPanel != null) 
        {
            // Плавно виходимо з чорного екрана
            FadeIn();
        }
    }

    /// <summary>
    /// Завантажує текстові дані з JSON-файлу у папці Resources.
    /// </summary>
    /// <param name="fileName">Назва файлу без розширення (наприклад, "strings_ua").</param>
    private void LoadStrings(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            // 1. Десеріалізація JSON:
            // JsonUtility не підтримує пряму десеріалізацію в Dictionary<string, string>, 
            // тому ми використовуємо обхідний шлях: десеріалізацію у клас з єдиним словником.
            gameStrings = JsonUtility.FromJson<GameStringData>(jsonFile.text);

            if (gameStrings != null && gameStrings.strings != null)
            {
                Debug.Log($"Strings loaded successfully from {fileName}.json. Total entries: {gameStrings.strings.Count}");
            }
        }
        else
        {
            Debug.LogError($"String data file not found in Resources: {fileName}.");
        }
    }

    // --- МЕТОДИ ДЛЯ InteractionZone ---

    // area_pick_
    /// Обробляє підбір предмета, приховуючи колайдер та запускаючи анімацію візуалу.
    /// <param name="itemName">Назва предмета.</param>
    /// <param name="zoneObject">Батьківський об'єкт зони (для вимкнення колайдера).</param>
    /// <param name="visualObject">Візуальний об'єкт, який потрібно анімувати.</param>
    public void HandleInventoryPickup(string itemName, GameObject zoneObject, GameObject visualObject)
    {
        // 1. ПРИХОВУЄМО ЗОНУ ВЗАЄМОДІЇ (Колайдер)
        // Ми вимикаємо лише компонент Collider2D, щоб ZoneObject міг залишитися,
        // якщо він потрібен для запуску анімації польоту, або вимикаємо весь об'єкт,
        // якщо він більше не потрібен (залежить від вашого flow).

        string hintText = LevelManager.Instance.gameStrings.GetString("00_END_GAME_BBT");
        Debug.Log($"New Hint: {hintText}");

        // Щоб зберегти об'єкт для майбутніх анімацій, краще вимкнути лише колайдер:
        Collider2D collider = zoneObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            // Альтернатива: якщо колайдер не знайдено, просто деактивуємо весь батьківський об'єкт зони.
            zoneObject.SetActive(false);
        }

        // 2. ДОДАЄМО ПРЕДМЕТ (Логіка)
        InventoryManager.Instance.AddItem(itemName, isPart: false);

        // 3. ЗАПУСКАЄМО АНІМАЦІЮ ВІЗУАЛЬНОГО ОБ'ЄКТА
        if (visualObject != null)
        {
            Debug.Log($"Starting animation for {itemName} to Inventory Panel.");
            // TODO: Викликати тут метод, який перемістить visualObject від його поточної позиції
            //       до цільової позиції на UI Panel (наприклад, за допомогою DOTween або Unity Animation).
            //       Після завершення анімації visualObject слід Destroy() або SetActive(false).
            visualObject.SetActive(false);
        }

        Debug.Log($"Item picked up: {itemName}. Ready for animation.");
    }

    // area_zoom_, area_dialog_
    /// Відкриває режим зуму, активуючи відповідний контейнер.
    /// <param name="zoomName">Назва зуму (наприклад, "Chest" або "Shelf").</param>
    public void HandleZoom(string zoomName)
    {
        if (zoomContainer != null)
        {
            // У реальній грі тут буде логіка: 
            // 1. Зміна спрайта/текстури в zoomContainer на арт, що відповідає zoomName
            // 2. Активація zoomContainer

            zoomContainer.SetActive(true);
            Debug.Log($"Zoom opened: {zoomName}");
        }
        else
        {
            Debug.LogError("Zoom Container is not assigned in LevelManager!");
        }
    }

    /// Закриває режим зуму, повертаючись до основного огляду сцени.
    public void CloseZoom()
    {
        if (zoomContainer != null)
        {
            // Деактивуємо контейнер, повертаючись до основної сцени
            zoomContainer.SetActive(false);
            Debug.Log("Zoom closed. Back to main scene.");
        }
    }

    // area_link_
    // МЕТОД ПЕРЕХОДУ З ЗАТЕМНЕННЯМ
    /// Запускає процес затемнення та завантаження сцени.
    /// Цей метод викликається з InteractionZone.
    public void LoadScene(string sceneName)
    {
        // Перевіряємо, чи існує сцена та чи налаштована панель затемнення
        if (Application.CanStreamedLevelBeLoaded(sceneName) && faderPanel != null)
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
        else
        {
            Debug.LogError($"Cannot load scene '{sceneName}' or FaderPanel is not assigned.");
        }
    }

    /// Корутина для плавного затемнення та завантаження нової сцени.
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 1. АНІМАЦІЯ ЗАТЕМНЕННЯ (Fade Out - затемнення до чорного)
        yield return StartCoroutine(Fade(1f)); // Затемнюємо до непрозорості (alpha = 1)

        // 2. ЗАВАНТАЖЕННЯ СЦЕНИ
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);

        // Додатково: Щоб уникнути чорного екрана назавжди, 
        // нова сцена також повинна запустити Fade(0f) у своєму LevelManager.Awake()
    }

    /// Змінює прозорість FaderPanel від поточної до targetAlpha.
    /// <param name="targetAlpha">Цільова прозорість (0f = прозорий, 1f = чорний).</param>
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = faderPanel.color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);

            // Оновлюємо колір, змінюючи лише альфа-канал
            Color newColor = faderPanel.color;
            newColor.a = newAlpha;
            faderPanel.color = newColor;

            yield return null; // Чекаємо наступного кадру
        }

        // Встановлюємо точне кінцеве значення, щоб уникнути похибок Lerp
        Color finalColor = faderPanel.color;
        finalColor.a = targetAlpha;
        faderPanel.color = finalColor;
    }

    // МЕТОД ДЛЯ НОВОЇ СЦЕНИ (Fade In)
    /// Метод, який нова сцена викликає для плавного "виходу" з чорного екрана.
    /// Рекомендується викликати його у Start() нової сцени.
    public void FadeIn()
    {
        // Переконайтеся, що faderPanel починає з 1f (чорний)
        Color startColor = faderPanel.color;
        startColor.a = 1f;
        faderPanel.color = startColor;

        StartCoroutine(Fade(0f)); // Починаємо затемнення до прозорості (alpha = 0)
    }

    // area_click_
    public void HandleGenericClick(string actionTarget)
    {
        Debug.Log($"Generic click action on: {actionTarget}");
        // TODO: Додати логіку для одноразових/багаторазових кліків
    }

    // area_hold_
    public void HandleHoldStart(string targetName, GameObject zone)
    {
        Debug.Log($"Hold started on: {targetName}");
        // TODO: Почати таймер утримання
    }

    // area_hold_
    public void HandleHoldEnd(string targetName)
    {
        Debug.Log($"Hold ended on: {targetName}");
        // TODO: Зупинити таймер утримання та виконати дію
    }

    // --- МЕТОДИ DRAG & DROP ---

    // area_get_
    public void HandleDragStart(string itemToDrag)
    {
        if (!IsDragging)
        {
            IsDragging = true;
            currentlyDraggedItem = itemToDrag;
            Debug.Log($"Started dragging: {itemToDrag}");
            // TODO: Створити візуальний плейсхолдер предмета
        }
    }

    public void UpdateDragPosition(Vector2 pos)
    {
        // TODO: Оновити позицію візуального плейсхолдера
    }

    public string GetCurrentlyDraggedItem()
    {
        return currentlyDraggedItem;
    }

    // area_drop_
    public void HandleDropSuccess(string dropTargetName, GameObject zone)
    {
        Debug.Log($"Successfully dropped {currentlyDraggedItem} on target: {dropTargetName}");
        currentlyDraggedItem = "";
        IsDragging = false;
        // TODO: Видалити плейсхолдер, виконати дію застосування
    }

    public void HandleDragEnd(string itemDragged, GameObject dropZone)
    {
        if (IsDragging)
        {
            Debug.Log($"Drag ended, item {itemDragged} was not successfully dropped.");
            currentlyDraggedItem = "";
            IsDragging = false;
            // TODO: Видалити плейсхолдер
        }
    }


}