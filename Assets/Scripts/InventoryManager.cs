using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton-патерн для легкого доступу
    public static InventoryManager Instance;

    // Словник для зберігання предметів та їхньої кількості (для збірних частин)
    // Key = Назва предмета (наприклад, "Key", "RopePart", "Gear")
    // Value = Кількість частин / наявність (1, якщо предмет цілий)
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Додає предмет до інвентарю. Викликається InteractionZone (area_pick_).
    /// </summary>
    /// <param name="itemName">Назва предмета/частини.</param>
    /// <param name="isPart">Чи це частина збірного предмета.</param>
    public void AddItem(string itemName, bool isPart = false, int requiredParts = 1)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName]++;
        }
        else
        {
            inventory.Add(itemName, 1);
        }

        Debug.Log($"Inventory: Added 1 x {itemName}. Current count: {inventory[itemName]}");

        // TODO: Оновити UI Інвентарю

        // Перевірка, чи предмет зібраний
        if (isPart && inventory[itemName] >= requiredParts)
        {
            Debug.Log($"ASSEMBLED: {itemName} is now complete!");
            // TODO: Замінити частини на цілий предмет в інвентарі, можливо, відобразити анімацію збірки
        }
    }

    /// <summary>
    /// Перевіряє, чи є предмет в інвентарі.
    /// </summary>
    public bool HasItem(string itemName)
    {
        return inventory.ContainsKey(itemName) && inventory[itemName] > 0;
    }

    /// <summary>
    /// Видаляє предмет з інвентарю (після одноразового використання).
    /// </summary>
    public void RemoveItem(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName]--;
            if (inventory[itemName] <= 0)
            {
                inventory.Remove(itemName);
            }
            Debug.Log($"Inventory: Removed 1 x {itemName}.");
            // TODO: Оновити UI Інвентарю
        }
    }
}