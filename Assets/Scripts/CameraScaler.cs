using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class CameraScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Grid _grid;  
    [SerializeField] private GridManager _gridManager;  
    [SerializeField] private UIManager _UIManager;  
    [SerializeField] private Tilemap _backgroundTilemap;

    [Header("Settings")]
    [SerializeField] private float padding = 1f; // Отступ от краёв карты
    [SerializeField] private float tileSize = 1f;

    public void Initialize(LevelData level, GridManager gridManager, UIManager UIManager)
    {
        currentLevel = level;
        _gridManager = gridManager;
        _UIManager = UIManager;
        AdaptCameraToLevel(currentLevel);
    }

    public void AdaptCameraToLevel(LevelData levelData) //todo fix
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
                return;
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
        float mapWidth = (columns + padding * 2) * tileSize;
        float mapHeight = (rows + padding * 2) * tileSize;

        // Соотношение сторон экрана
        float screenAspect = (float)Screen.width / Screen.height;

        // Размер камеры по вертикали и горизонтали
        float sizeByHeight = mapHeight / 2f;
        float sizeByWidth = mapWidth / (2f * screenAspect);

        _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        _camera.transform.position = new Vector3(0, 0, _camera.transform.position.z);

        if (_grid != null)
        {
            float horizontal = Math.Abs(_gridManager.maxX + 1) - Math.Abs(_gridManager.minX);
            float vertical = Math.Abs(_gridManager.maxY + 1) - Math.Abs(_gridManager.minY);
            _grid.transform.position -= new Vector3(horizontal / 2, vertical / 2, 0);

            float frameHeight = _camera.orthographicSize - mapHeight / 2 - 1;
            float frameWidht = columns;
            Vector3 framePosition = new Vector3(0, 0, _grid.transform.position.z) - new Vector3(0, rows / 2 + 1f + frameHeight / 2, 0);

            _UIManager.SpawnFigurePlacementFrame(frameHeight, frameWidht, framePosition);
        }

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