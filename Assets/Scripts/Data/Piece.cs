using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Audio.ProcessorInstance;

public class Piece
{
    public GameObject pieceObj { get; private set; }
    public List<GameObject> cells { get; private set; }
    public PieceDragHandler dragHandler { get; private set; }
    public PieceData pieceData { get; private set; }

    public bool isActive { get; set; }
    public bool isPlaced { get; set; }
    private Vector3Int gridPosition;      // позиция якоря на сетке (в клетках)
    public bool isDragging { get; set; }

    private const string FIGURE_SORTING_LAYER_NAME = "Figure";

    public Piece(PieceData pieceData, GameObject obj = null)
    {
        this.pieceData = pieceData;
        pieceObj = obj ?? new GameObject($"Piece_{pieceData.name}");

        // --- Добавляем составной коллайдер на родителя ---
        Rigidbody2D rb = pieceObj.GetOrAddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        CompositeCollider2D composite = pieceObj.GetOrAddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
        composite.generationType = CompositeCollider2D.GenerationType.Synchronous;

        // --- DragHandler (будет проинициализирован позже) ---
        dragHandler = pieceObj.GetOrAddComponent<PieceDragHandler>();
        cells = new List<GameObject>();

        isActive = false;
        isPlaced = false;
        isDragging = false;
    }

    /// <summary>
    /// Инициализация визуала и якоря
    /// </summary>
    public void Initialize(Sprite sprite, float cellSize, PieceManager pieceManager = null, GridManager gridManager = null)
    {
        BuildVisual(sprite, cellSize);
        dragHandler.Initialize(this, pieceManager, gridManager); // передаём менеджер
    }

    private void BuildVisual(Sprite sprite, float cellSize)
    {
        int[,] shape = pieceData.GetShapeMatrix();
        int width = pieceData.size.x;
        int height = pieceData.size.y;


        Vector3 anchorOffset = Vector3.zero;
        // Вычисляем смещение якоря (центр якорной клетки относительно корня)
        if (pieceData.HasAnchor)
        {
            Vector2Int anchor = pieceData.anchorPosition;
            anchorOffset = new Vector3(
                anchor.x * cellSize,
                anchor.y * cellSize,
                0
            );
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (shape[x, y] == 1)
                {
                    GameObject cell = new GameObject($"Cell_{x}_{y}");
                    cells.Add(cell);
                    cell.transform.SetParent(pieceObj.transform);

                    SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();
                    sr.sprite = sprite;
                    sr.sortingLayerName = FIGURE_SORTING_LAYER_NAME;

                    cell.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0) - anchorOffset;
                    cell.transform.localScale = Vector3.one * cellSize;

                    // Коллайдер клетки (включается в составной)
                    BoxCollider2D box = cell.AddComponent<BoxCollider2D>();
                    box.isTrigger = true;
                    box.compositeOperation = Collider2D.CompositeOperation.Merge;
                    box.size = Vector2.one * cellSize;
                }
            }
        }

        // Обновляем составной коллайдер
        CompositeCollider2D composite = pieceObj.GetComponent<CompositeCollider2D>();
        if (composite != null) composite.GenerateGeometry();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        SetVisible(active);
    }

    public void SetVisible(bool visible)
    {
        pieceObj.SetActive(visible);
    }

    public void SetScale(Vector3 scale)
    {
        pieceObj.transform.localScale = scale;
    }

    /// <summary>
    /// Устанавливает позицию фигуры так, чтобы якорь оказался в anchorWorldPosition.
    /// Если передан tilemap, позиция привязывается к центру ближайшей клетки.
    /// </summary>
    public void SetPosition(Vector3 coordinates, Tilemap tilemap = null)
    {
        if (tilemap != null)
        {
            Vector3Int cellPos = tilemap.WorldToCell(coordinates);
            //Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
            pieceObj.transform.position = cellPos;
            gridPosition = cellPos;
        }
        else
        {
            pieceObj.transform.position = coordinates;
        }
    }

    /// <summary>
    /// Возвращает текущую мировую позицию якоря
    /// </summary>
    public Vector3 GetWorldPosition()
    {
        return pieceObj.transform.position;
    }

    /// <summary>
    /// Проверка и размещение фигуры на Tilemap
    /// </summary>
    public void Place()
    {
        isPlaced = true;
        isActive = false;
        DestroyPiece();
    }

    public void DestroyPiece()
    {
        Object.Destroy(pieceObj);
    }

    // Вспомогательный метод для получения позиции на сетке (например, для превью)
    public Vector3Int GetGridPosition(Tilemap targetTilemap)
    {
        return targetTilemap.WorldToCell(GetWorldPosition());
    }

    // Вспомогательный метод для получения позиции на сетке (например, для превью)
    public List<Vector3Int> GetGridCellsPositions(Tilemap targetTilemap)
    {
        int[,] shape = pieceData.GetShapeMatrix();
        List<Vector3Int> result = new List<Vector3Int>();
        int w = pieceData.size.x;
        int h = pieceData.size.y;
        Vector3 center = GetWorldPosition();

        foreach(GameObject cell in cells)
        {
            result.Add(targetTilemap.WorldToCell(cell.transform.position));
        }

        return result;
    }
}