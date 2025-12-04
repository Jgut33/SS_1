using UnityEngine;

// Клас, який містить усі незмінні константи гри.
// Не потрібно прикріплювати до об'єкта.
public static class Const
{
    // Тривалість затемнення для переходу між сценами (в секундах).
    public const float FadeDuration = 0.5f;

    // Назва файлу з текстовими стрінгами у папці Resources (без розширення).
    public const string StringDataFileName_UA = "strings_ua";
    public const string StringDataFileName_EN = "strings_en";

    // Швидкість прокрутки діалогового вікна.
    public const float DialogueScrollSpeed = 0.05f;
}