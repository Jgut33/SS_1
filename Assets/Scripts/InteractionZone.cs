// InteractionZone.cs
using UnityEngine;
using UnityEngine.EventSystems; // Потрібно для обробки Drag and Drop

public class InteractionZone : MonoBehaviour
{
    // --- ЗМІННІ ---

    [Tooltip("Назва об'єкта (наприклад, area_pick_Key, area_zoom_Chest)")]
    public string fullZoneName;

    private string actionPrefix; // area_pick_, area_use_, area_get_, etc.
    private string targetName;   // Key, Chest, Lever, etc.

    // --- МЕТОДИ ІНІЦІАЛІЗАЦІЇ ---

    void Awake()
    {
        // Встановлюємо повну назву зони з об'єкта в Unity
        fullZoneName = gameObject.name.ToLower();
        ParseZoneName();
    }

    /// <summary>
    /// Розбиває назву об'єкта на префікс дії та цільову назву.
    /// Наприклад, "area_pick_Key" -> Prefix: "area_pick_", Target: "Key"
    /// </summary>
    private void ParseZoneName()
    {
        int separatorIndex = fullZoneName.IndexOf('_', fullZoneName.IndexOf('_') + 1); // Знаходить друге '_'

        if (separatorIndex != -1)
        {
            actionPrefix = fullZoneName.Substring(0, separatorIndex + 1);
            targetName = fullZoneName.Substring(separatorIndex + 1);
        }
        else
        {
            Debug.LogError($"InteractionZone Error: Invalid name format for {fullZoneName}. Missing underscore separator.");
            actionPrefix = "error_";
            targetName = "";
        }
    }

    // --- ОБРОБКА КЛІКУ (OnMouseDown, OnMouseUp) ---

    // OnMouseDown використовується для звичайного кліку (area_pick_, area_zoom_, etc.)
    private void OnMouseDown()
    {
        // Якщо ми вже щось перетягуємо, не обробляємо клік на фоновій зоні
        // if (GameManager.Instance.IsDragging) return; 

        switch (actionPrefix)
        {
            case "area_pick_":
                // Підбір предмета в інвентар
                LevelManager.Instance.HandleInventoryPickup(targetName);
                break;

            case "area_zoom_":
            case "area_dialog_":
                // Відкриття зуму / діалогу
                LevelManager.Instance.HandleZoom(targetName);
                break;

            case "area_link_":
                // Перехід на іншу сцену (міні-гра або локація)
                LevelManager.Instance.LoadScene(targetName);
                break;

            case "area_click_":
                // Виконання одноразової/багаторазової дії
                LevelManager.Instance.HandleGenericClick(targetName);
                break;

            case "area_hold_":
                // Початок утримання (Hold)
                LevelManager.Instance.HandleHoldStart(targetName, gameObject);
                break;

            default:
                Debug.Log($"InteractionZone: Unhandled action prefix: {actionPrefix} on {gameObject.name}");
                break;
        }
    }

    // Обробка завершення утримання
    private void OnMouseUp()
    {
        if (actionPrefix == "area_hold_")
        {
            LevelManager.Instance.HandleHoldEnd(targetName);
        }
    }

    // --- ЛОГІКА DRAG-AND-DROP (Перетягування) ---
    // (Потрібно, щоб зона була "area_get_..." або "area_drop_...")

    // OnMouseDrag викликається щокадру, коли об'єкт перетягується
    private void OnMouseDrag()
    {
        if (actionPrefix == "area_get_")
        {
            // Беремо предмет на курсор і переміщуємо його слідом за мишею
            LevelManager.Instance.HandleDragStart(targetName);
        }

        // Якщо ми вже перетягуємо, оновлюємо позицію предмета на курсорі
        if (LevelManager.Instance.IsDragging)
        {
            // Оновлюємо позицію візуального плейсхолдера, який ми перетягуємо
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LevelManager.Instance.UpdateDragPosition(new Vector2(mousePos.x, mousePos.y));
        }
    }

    // OnMouseUpAsButton викликається, коли ми відпустили кнопку миші
    private void OnMouseUpAsButton()
    {
        if (actionPrefix == "area_get_")
        {
            // Якщо ми відпустили "area_get_", просто скидаємо предмет, бо це не місце застосування
            LevelManager.Instance.HandleDragEnd(targetName, null);
        }
    }

    // Обробка застосування (Drop) на іншу зону
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, чи ми "кидаємо" щось на зону застосування (area_drop_)
        if (actionPrefix == "area_drop_")
        {
            // Припустимо, LevelManager знає, який предмет ми зараз перетягуємо
            string currentDraggedItem = LevelManager.Instance.GetCurrentlyDraggedItem();

            // Якщо ми перетягуємо потрібний предмет (наприклад, targetName = "Wheel", currentDraggedItem = "Wheel")
            if (currentDraggedItem == targetName)
            {
                LevelManager.Instance.HandleDropSuccess(targetName, gameObject);
            }
            // ... інакше, LevelManager може обробити невдале кидання
        }

        // *Логіка для area_use_ буде реалізована в LevelManager, коли гравець вибере предмет з інвентарю*
    }
}