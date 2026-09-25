using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class CameraScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Grid _grid;  
    [SerializeField] private GridManager _gridManager;  
    [SerializeField] private UIManager _UIManager;  
    [SerializeField] private Tilemap _backgroundTilemap;

    [Header("Settings")]
    [SerializeField] private float padding = 1f; // ќтступ от краЄв карты
    [SerializeField] private float tileSize = 1f;

    public void Initialize(GridManager gridManager, UIManager UIManager)
    {
        _gridManager = gridManager;
        _UIManager = UIManager;
        //AdaptCameraToLevel(currentLevel);
    }

    public void AdaptCameraToLevel(FigureData figureData, bool isMenuLevel = false)
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
                return;
        }

        int rows = figureData.rows;
        int columns = figureData.columns;

        // ¬ычисл€ем размеры карты с отступами
        float mapWidth = (columns + padding * 2) * tileSize;
        float mapHeight = (rows + padding * 2) * tileSize;

        // —оотношение сторон экрана
        float screenAspect = (float)Screen.width / Screen.height;

        // –азмер камеры по вертикали и горизонтали
        float sizeByHeight = mapHeight / 2f;
        float sizeByWidth = mapWidth / (2f * screenAspect);

        _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        _camera.transform.position = new Vector3(0, 0, _camera.transform.position.z);

        if (_grid != null) // стабилизаци€ сетки // TODO MAIN TASK допилить так как ломаетс€ после использовани€ на MainMenuLevel
        {
            float horizontal = Math.Abs(_gridManager.maxX + 1) - Math.Abs(_gridManager.minX);
            float vertical = Math.Abs(_gridManager.maxY + 1) - Math.Abs(_gridManager.minY);
            _grid.transform.position = new Vector3(0, 0, _grid.transform.position.z) - new Vector3(horizontal / 2, vertical / 2, 0); // todo найти норм формулу

            if (!isMenuLevel) // загрузка места под фигуры
            {
                float frameHeight = _camera.orthographicSize - mapHeight / 2 - 1;
                float frameWidht = columns;
                Vector3 framePosition = new Vector3(0, 0, _grid.transform.position.z) - new Vector3(0, rows / 2 + 1f + frameHeight / 2, 0);

                _UIManager.SpawnFigurePlacementFrame(frameHeight, frameWidht, framePosition);
            }
        }

        Debug.Log($"[CameraScaler]  амера настроена под уровень: {columns}x{rows}, size: {_camera.orthographicSize} {_camera}");
    }
}