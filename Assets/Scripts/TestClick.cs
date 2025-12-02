using UnityEngine;
using UnityEngine.EventSystems; // Обов'язково для IPointerClickHandler

// Реалізуємо інтерфейс IPointerClickHandler
public class TestClick : MonoBehaviour, IPointerClickHandler
{
    // Цей метод викликається EventSystem, коли було клікнуто на колайдер.
    public void OnPointerClick(PointerEventData eventData)
    {
        // Перевіряємо, чи це була ліва кнопка миші/основний дотик
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // !!! ОСНОВНИЙ ЛОГ-ВИКЛИК !!!
            Debug.Log($"SUCCESS: Click Registered on {gameObject.name} at {Time.time}!");
        }
    }

    // Перевірка надійності: цей метод спрацює при дотику/кліку, навіть якщо OnPointerClick має проблеми.
    // Якщо цей лог спрацьовує, а OnPointerClick - ні, проблема в EventSystem, а не в колайдері.
    void OnMouseUp()
    {
        Debug.Log($"FALLBACK: OnMouseUp registered on {gameObject.name}!");
    }
}