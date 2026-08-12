using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Основная сетка")]
    [SerializeField] public Tilemap tilemap;
    [SerializeField] private TileBase cellTile;

    [Header("Превью")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase previewTile; // прозрачный тайл

    [Header("Размеры")]
    public int width = 10;
    public int height = 10;

    private List<Vector3Int> currentPreviewPositions = new List<Vector3Int>();

    // ──────────────────────────────────────────────────────────────
    // Инициализация (вызывается из LevelManager)
    // ──────────────────────────────────────────────────────────────

    public void Initialize(LevelData levelData)
    {
        if (levelData != null)
        {
            width = levelData.columns;
            height = levelData.rows;
        }
        ClearPreview();
    }

    // ──────────────────────────────────────────────────────────────
    // Превью
    // ──────────────────────────────────────────────────────────────

    public void ShowPreview(Piece piece)
    {
        if (piece == null || previewTilemap == null || previewTile == null)
            return;

        ClearPreview();

        Vector3Int[] positions = GetPieceGridPositions(piece);
        foreach (Vector3Int pos in positions)
        {
            if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
                continue;
            if (tilemap.HasTile(pos))
                continue;

            previewTilemap.SetTile(pos, previewTile);
            currentPreviewPositions.Add(pos);
        }
    }

    public void ClearPreview()
    {
        foreach (Vector3Int pos in currentPreviewPositions)
            previewTilemap.SetTile(pos, null);
        currentPreviewPositions.Clear();
    }

    public void UpdatePreview(Piece piece)
    {
        ClearPreview();
        ShowPreview(piece);
    }

    // ──────────────────────────────────────────────────────────────
    // Проверка и размещение
    // ──────────────────────────────────────────────────────────────

    public bool CanPlace(Piece piece)
    {
        if (piece == null || piece.pieceData == null)
            return false;

        Vector3Int[] positions = GetPieceGridPositions(piece);
        foreach (Vector3Int pos in positions)
        {
            if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
                return false;
            if (tilemap.HasTile(pos))
                return false;
        }
        return true;
    }

    public bool PlacePiece(Piece piece)
    {
        if (!CanPlace(piece))
            return false;

        Vector3Int[] positions = GetPieceGridPositions(piece);
        foreach (Vector3Int pos in positions)
            tilemap.SetTile(pos, cellTile);

        ClearPreview();
        return true;
    }

    // ──────────────────────────────────────────────────────────────
    // Вспомогательные
    // ──────────────────────────────────────────────────────────────

    private Vector3Int[] GetPieceGridPositions(Piece piece)
    {
        if (piece == null || piece.pieceData == null || !piece.pieceData.HasAnchor)
            return new Vector3Int[0];

        int[,] shape = piece.pieceData.GetShapeMatrix();
        int w = piece.pieceData.size.x;
        int h = piece.pieceData.size.y;
        Vector2Int anchor = piece.pieceData.anchorPosition;

        // Получаем позицию якоря на сетке (центр ячейки)
        Vector3 anchorWorld = piece.GetWorldPosition();
        Vector3Int anchorGridPos = tilemap.WorldToCell(anchorWorld);

        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (shape[x, y] == 1)
                {
                    int offX = x - anchor.x;
                    int offY = y - anchor.y;
                    positions.Add(new Vector3Int(anchorGridPos.x + offX, anchorGridPos.y + offY, 0));
                }
        return positions.ToArray();
    }

    public Vector3Int GetGridPosition(Piece piece)
    {
        if (piece == null || piece.pieceObj == null)
            return Vector3Int.zero;
        return tilemap.WorldToCell(piece.pieceObj.transform.position);
    }
}