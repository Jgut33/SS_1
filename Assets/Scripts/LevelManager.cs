using UnityEngine;
using UnityEngine.SceneManagement; // Потрібно для LoadScene

public class LevelManager : MonoBehaviour
{
    // 1. Singleton: Статичний екземпляр класу
    public static LevelManager Instance;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- МЕТОДИ ДЛЯ InteractionZone ---

    // area_pick_
    /// <summary>
    /// Обробляє підбір предмета, приховуючи колайдер та запускаючи анімацію візуалу.
    /// </summary>
    /// <param name="itemName">Назва предмета.</param>
    /// <param name="zoneObject">Батьківський об'єкт зони (для вимкнення колайдера).</param>
    /// <param name="visualObject">Візуальний об'єкт, який потрібно анімувати.</param>
    public void HandleInventoryPickup(string itemName, GameObject zoneObject, GameObject visualObject)
    {
        // 1. ПРИХОВУЄМО ЗОНУ ВЗАЄМОДІЇ (Колайдер)
        // Ми вимикаємо лише компонент Collider2D, щоб ZoneObject міг залишитися,
        // якщо він потрібен для запуску анімації польоту, або вимикаємо весь об'єкт,
        // якщо він більше не потрібен (залежить від вашого flow).

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
    public void HandleZoom(string zoomTargetName)
    {
        Debug.Log($"Opening zoom/dialog: {zoomTargetName}");
        // TODO: Активувати UI Canvas для зуму
    }

    // area_link_
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        // SceneManager.LoadScene(sceneName);
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