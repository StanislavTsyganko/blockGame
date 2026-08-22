using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Основная сетка")]
    [SerializeField] public Tilemap backgroundTilemapLayer;
    [SerializeField] private TileBase backgroundCellTile;

    [Header("Целевая фигура")]
    [SerializeField] private Tilemap targetTilemapLayer;

    [Header("Превью")]
    [SerializeField] private Tilemap previewTilemapLayer;
    [SerializeField] private TileBase previewCellTile;

    [Header("Animation")]
    public float tileAppearDelay = 0.03f;
    public bool animateTiles = true;

    public bool gridLoaded = false;
    public int maxX = 0, maxY = 0, minX = 0, minY = 0;

    //private List<Cell> cells = new List<Cell>();.

    private List<Vector3Int> currentPreviewPositions = new List<Vector3Int>();
    private LevelData currentLevelData;

    public UnityEvent<Piece> OnPiecePlaced;
    public UnityEvent OnMapLoaded;

    public void Initialize(LevelData levelData)
    {
        ClearTilemaps();
        currentLevelData = levelData;
        if (levelData != null)
        {
            foreach (TileData tile in levelData.BackgroundTilesLayer)
            {
                if (tile.position.x > maxX) maxX = tile.position.x;
                if (tile.position.y > maxY) maxY = tile.position.y;
                if (tile.position.x < minX) minX = tile.position.x;
                if (tile.position.y < minY) minY = tile.position.y;
            }
        }
        ClearPreview();
        //if (OnPiecePlaced == null)
            //OnPiecePlaced = new UnityEvent<Piece>();
    }

    public void SpawnGrid()
    {
        if(!currentLevelData)
            return;
        if (animateTiles)
            StartCoroutine(AnimateLoadLevel(currentLevelData));
        else
            InstantLoadLevel(currentLevelData);
        OnMapLoaded.Invoke();
    }

    public void ShowPreview(Piece piece)
    {
        if (piece == null || previewTilemapLayer == null || previewCellTile == null)
            return;

        ClearPreview();

        Vector3Int[] positions = GetPieceGridPositions(piece);
        foreach (Vector3Int pos in positions)
        {
            if (!backgroundTilemapLayer.HasTile(pos))
                continue;

            previewTilemapLayer.SetTile(pos, previewCellTile);
            currentPreviewPositions.Add(pos);
        }
    }


    private void ClearTilemaps()
    {
        backgroundTilemapLayer?.ClearAllTiles();
        targetTilemapLayer?.ClearAllTiles();
        previewTilemapLayer?.ClearAllTiles();
    }


    public void ClearPreview()
    {
        foreach (Vector3Int pos in currentPreviewPositions)
            previewTilemapLayer.SetTile(pos, null);
        currentPreviewPositions.Clear();
    }

    public void UpdatePreview(Piece piece)
    {
        ClearPreview();
        ShowPreview(piece);
    }

    private void InstantLoadLevel(LevelData data)
    {
        LoadLayer(backgroundTilemapLayer, data.BackgroundTilesLayer, data.BackgroundTilesPaletteLayer, "background");
        LoadLayer(targetTilemapLayer, data.TargetTilesLayer, data.TargetTilesPaletteLayer);
        gridLoaded = true;
    }

    private void LoadLayer(Tilemap tilemap, List<TileData> tiles, TileBase[] palette, string cellType = null)
    {
        if (tilemap == null || tiles == null || palette == null) return;
        foreach (var tile in tiles)
            if (tile.tileID >= 0 && tile.tileID < palette.Length)
            {
                tilemap.SetTile(tile.position, palette[tile.tileID]);
                //TileBase tileB = tilemap.GetTile(tile.position);
                //Cell cell = tileB.AddComponent<Cell>();
                //if (cellType != null)
                    //cell.Initialize(tile.position, cellType);
            }
    }

    private IEnumerator AnimateLoadLevel(LevelData data)
    {
        var allTiles = new List<AnimatedTileData>();
        CollectTilesForAnimation(data, backgroundTilemapLayer, data.BackgroundTilesLayer, data.BackgroundTilesPaletteLayer, allTiles);
        CollectTilesForAnimation(data, targetTilemapLayer, data.TargetTilesLayer, data.TargetTilesPaletteLayer, allTiles);

        allTiles.Sort((a, b) =>
        {
            if (a.tile.position.y != b.tile.position.y)
                return a.tile.position.y.CompareTo(b.tile.position.y);
            return a.tile.position.x.CompareTo(b.tile.position.x);
        });

        foreach (var dataTile in allTiles)
        {
            if (dataTile.tilemap == null) continue;
            dataTile.tilemap.SetTile(dataTile.tile.position, dataTile.palette[dataTile.tile.tileID]);
            dataTile.tilemap.SetTileFlags(dataTile.tile.position, TileFlags.None);
            dataTile.tilemap.SetColor(dataTile.tile.position, Color.yellow);
            yield return new WaitForSeconds(tileAppearDelay);
            dataTile.tilemap.SetColor(dataTile.tile.position, Color.white);
        }

        gridLoaded = true;
        Debug.Log($"[LevelManager] Анимация завершена. Тайлов: {allTiles.Count}");
    }

    private void CollectTilesForAnimation(LevelData data, Tilemap tilemap, List<TileData> tiles, TileBase[] palette, List<AnimatedTileData> list)
    {
        if (tilemap == null || tiles == null || palette == null) return;
        foreach (var tile in tiles)
            list.Add(new AnimatedTileData { tile = tile, tilemap = tilemap, palette = palette });
    }

    private class AnimatedTileData
    {
        public TileData tile;
        public Tilemap tilemap;
        public TileBase[] palette;
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
            if (backgroundTilemapLayer.GetTile(pos) && backgroundTilemapLayer.GetTile(pos) == backgroundCellTile)
            {
                //Cell cell = backgroundTilemapLayer.GetTile(pos).GetComponent<Cell>();
                //if (cell != null && !cell.destroyed)
                    return true;
            }
        }
        return false;
    }

    public bool PlacePiece(Piece piece)
    {
        if (!CanPlace(piece))
            return false;

        try 
        {
            Vector3Int[] positions = GetPieceGridPositions(piece);
            foreach (Vector3Int pos in positions)
            {
                //Cell cell = backgroundTilemapLayer.GetTile(pos).GetComponent<Cell>();
                //if (cell != null)
                //if (!cell.destroyed)
                //{
                //backgroundTilemapLayer.SetTile(pos, previewCellTile);
                //cell.destroyed = true;
                //}
                //else
                //if (backgroundTilemapLayer.GetTile(pos) == backgroundCellTile)
                if (backgroundTilemapLayer.HasTile(pos))
                    backgroundTilemapLayer.SetTile(pos, previewCellTile);
            }

            ClearPreview();
            OnPiecePlaced.Invoke(piece);
        }
        catch
        { 
            return false; 
        }
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
        Vector3Int anchorGridPos = backgroundTilemapLayer.WorldToCell(anchorWorld);

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
        return backgroundTilemapLayer.WorldToCell(piece.pieceObj.transform.position);
    }

    public Vector3Int[] GetNotDestroyedBackgorund()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach (TileData tileData in currentLevelData.BackgroundTilesLayer)
        {
            // todo Сделать Tile-compound class
            TileBase tileBackground = backgroundTilemapLayer.GetTile(tileData.position);
            TileBase tileTarget = targetTilemapLayer.GetTile(tileData.position);
            if (tileBackground != null && tileBackground == backgroundCellTile && !tileTarget)
                positions.Add(tileData.position);
        }

        return positions.ToArray();
    }
}