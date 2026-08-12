using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class CameraScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LevelData currentLevel;

    [Header("Settings")]
    [SerializeField] private float padding = 1f; // Отступ от краёв карты
    [SerializeField] private float tileSize = 1f;

    private void Start()
    {
        if (currentLevel != null)
        {
            AdaptCameraToLevel(currentLevel);
        }
        else
        {
            Debug.LogWarning("[CameraScaler] LevelData не назначен! Использую ручные настройки.");
        }
    }

    // ============================================
    // АДАПТАЦИЯ ПОД LevelData
    // ============================================

    public void AdaptCameraToLevel(LevelData levelData)
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("[CameraScaler] Камера не найдена!");
                return;
            }
        }

        if (!_camera.orthographic)
        {
            Debug.LogWarning("[CameraScaler] Камера не ортографическая! Настройка отключена.");
            return;
        }

        if (levelData == null)
        {
            Debug.LogWarning("[CameraScaler] LevelData не передан! Использую ручные настройки.");
            return;
        }

        // Используем данные из LevelData
        int rows = levelData.rows;
        int columns = levelData.columns;

        // Если данные пустые 
        if (rows <= 0 || columns <= 0)
        {
            Debug.LogWarning("[CameraScaler] В LevelData нет данных о размере сетки! Использую ручные настройки.");
            return;
        }

        // Вычисляем размеры карты с отступами
        float mapWidth = (columns + padding) * tileSize;
        float mapHeight = (rows + padding) * tileSize;

        // Соотношение сторон экрана
        float screenAspect = (float)Screen.width / Screen.height;

        // Размер камеры по вертикали и горизонтали
        float sizeByHeight = mapHeight / 2f;
        float sizeByWidth = mapWidth / (2f * screenAspect);

        // Выбираем максимальный размер, чтобы всё поместилось
        _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

        Debug.Log($"[CameraScaler] Камера настроена под уровень: {columns}x{rows}, size: {_camera.orthographicSize} {_camera}");
    }

    // ============================================
    // ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ВНЕШНЕГО ВЫЗОВА
    // ============================================

    /// <summary>
    /// Обновить камеру при смене уровня
    /// </summary>
    public void UpdateCamera(LevelData newLevel)
    {
        currentLevel = newLevel;
        AdaptCameraToLevel(newLevel);
    }

    /// <summary>
    /// Пересчитать камеру (например, при изменении ориентации экрана)
    /// </summary>
    public void RefreshCamera()
    {
        AdaptCameraToLevel(currentLevel);
    }
}