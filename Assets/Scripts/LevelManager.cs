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
    public void HandleInventoryPickup(string itemName)
    {
        Debug.Log($"Picked up item: {itemName}");
        // TODO: Додати предмет до InventoryManager
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