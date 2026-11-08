using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public enum TileState
{
    Normal,
    Destroyed,
    Figure,
    None,
}

public enum CellTypes
{
    Background,
    Target,
    Preview,
    Animation,
    None,
}

public struct TileInfo
{
    public TileBase tile;
    public Color color;
    public TileState state;

    public TileInfo(TileBase tile, Color color, TileState state)
    {
        this.tile = tile;
        this.color = color;
        this.state = state;
    }

    public TileInfo(TileBase tile, Color color)
    {
        this.tile = tile;
        this.color = color;
        state = TileState.Normal;
    }
}

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
    [SerializeField] private AnimatedTile explodeTile;
    [SerializeField] private Tilemap animationTilemapLayer;

    private Dictionary<Vector3Int, TileInfo> tilesInfo = new Dictionary<Vector3Int, TileInfo>(); //redo tileState to tile info and unite all info to know what kind of block destroyed
    public Dictionary<TileInfo, int> uniqTiles = new Dictionary<TileInfo, int>();

    public bool gridLoaded = false;
    public int maxX = 0, maxY = 0, minX = 0, minY = 0;

    private List<Vector3Int> currentPreviewPositions = new List<Vector3Int>();
    private FigureData currentFigureData;

    public UnityEvent<Piece> OnPiecePlaced;
    public UnityEvent OnMapLoaded;

    public void Initialize(FigureData figureData)
    {
        ClearTilemaps();
        currentFigureData = figureData;
        maxX = 0; maxY = 0; minX = 0; minY = 0;
        if (figureData != null)
        {
            foreach (TileData tile in figureData.BackgroundTilesLayer)
            {
                if (tile.position.x > maxX) maxX = tile.position.x;
                if (tile.position.y > maxY) maxY = tile.position.y;
                if (tile.position.x < minX) minX = tile.position.x;
                if (tile.position.y < minY) minY = tile.position.y;
            }
            foreach (TileData tile in figureData.TargetTilesLayer)
            {
                if (tile.position.x > maxX) maxX = tile.position.x;
                if (tile.position.y > maxY) maxY = tile.position.y;
                if (tile.position.x < minX) minX = tile.position.x;
                if (tile.position.y < minY) minY = tile.position.y;
            }
        }
        ClearPreview();
    }

    public void SpawnGrid()
    {
        ClearTilemaps();
        if (!currentFigureData)
            return;
        if (animateTiles)
            StartCoroutine(AnimateLoadLevel(currentFigureData));
        else
            InstantLoadLevel(currentFigureData);
    }

    public void ShowPreview(Piece piece)
    {
        if (piece == null || previewTilemapLayer == null || previewCellTile == null)
            return;

        ClearPreview();

        Vector3Int[] positions = GetPieceGridPositions(piece);
        foreach (Vector3Int pos in positions)
        {
            if (!backgroundTilemapLayer.HasTile(pos) || GetTileState(pos) != TileState.Normal)
                continue;

            previewTilemapLayer.SetTile(pos, previewCellTile);
            currentPreviewPositions.Add(pos);
        }
    }


    public void ClearTilemaps()
    {
        backgroundTilemapLayer?.ClearAllTiles();
        targetTilemapLayer?.ClearAllTiles();
        previewTilemapLayer?.ClearAllTiles();
        animationTilemapLayer?.ClearAllTiles();
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

    private void InstantLoadLevel(FigureData data)
    {
        tilesInfo.Clear();
        uniqTiles.Clear();
        LoadLayer(targetTilemapLayer, data.TargetTilesLayer, data.TargetTilesPaletteLayer, CellTypes.Target);
        LoadLayer(backgroundTilemapLayer, data.BackgroundTilesLayer, data.BackgroundTilesPaletteLayer, CellTypes.Background);
        gridLoaded = true;
        OnMapLoaded.Invoke();
    }

    private void LoadLayer(Tilemap tilemap, List<TileData> tiles, TileBase[] palette, CellTypes cellType = CellTypes.None)
    {
        if (tilemap == null || tiles == null || palette == null) return;
        foreach (var tile in tiles)
        {
            if (tile.tileID >= 0 && tile.tileID < palette.Length)
            {
                tilemap.SetTile(tile.position, palette[tile.tileID]);
                tilemap.SetTileFlags(tile.position, TileFlags.None);
                tilemap.SetColor(tile.position, tile.color);
                if (cellType == CellTypes.Target)
                {
                    if (GetTileState(tile.position) == TileState.Normal)
                    {
                        TileInfo tileInfo = tilesInfo[tile.position];
                        SetUniqTileCount(tileInfo.tile, tileInfo.color, false);
                    }
                    SetTileInfo(tile.position, palette[tile.tileID], tile.color, TileState.Figure);

                }
                if (cellType == CellTypes.Background)
                {
                    if (GetTileState(tile.position) != TileState.Figure)
                    {
                        SetTileInfo(tile.position, palette[tile.tileID], tile.color, TileState.Normal);
                        SetUniqTileCount(palette[tile.tileID], tile.color, true);
                    }
                }
            }
        }
    }

    private void SetUniqTileCount(TileBase tile, Color color, bool increase = true)
    {
        var key = new TileInfo(tile, color);
        if (!uniqTiles.TryGetValue(key, out int count) || count < 1)
            uniqTiles[key] = 1;
        else
        {
            if(increase)
                uniqTiles[key] += 1;
            else
                uniqTiles[key] -= 1;
        }
    }

    private IEnumerator AnimateLoadLevel(FigureData data)
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
            dataTile.tilemap.SetColor(dataTile.tile.position, dataTile.tile.color);
        }

        gridLoaded = true;
        OnMapLoaded.Invoke();
    }

    private void CollectTilesForAnimation(FigureData data, Tilemap tilemap, List<TileData> tiles, TileBase[] palette, List<AnimatedTileData> list)
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
            if (GetTileState(pos) == TileState.Normal)
                return true;
        }
        return false;
    }

    public bool PlacePiece(Piece piece)
    {
        ClearPreview();
        if (!CanPlace(piece))
            return false;

        try 
        {
            Vector3Int[] positions = GetPieceGridPositions(piece);
            foreach (Vector3Int pos in positions)
            {
                if (GetTileState(pos) == TileState.Normal)
                {
                    backgroundTilemapLayer.SetTile(pos, previewCellTile); // todo инкапсулировать слои
                    animationTilemapLayer.SetTile(pos, explodeTile);
                    SetTileState(pos, TileState.Destroyed);
                    TileInfo tileInfo = tilesInfo[pos];
                    SetUniqTileCount(tileInfo.tile, tileInfo.color, false);
                }
            }
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
        foreach (var (pos, tileInfo) in tilesInfo)
        {
            if (tileInfo.state == TileState.Normal)
                positions.Add(pos);
        }

        return positions.ToArray();
    }

    public void SetTileInfo(Vector3Int pos, TileInfo info)
    {
        tilesInfo[pos] = info;
    }

    public void SetTileInfo(Vector3Int pos, TileBase tile, Color color, TileState state)
    {
        TileInfo tileInfo = tilesInfo.TryGetValue(pos, out var value) ? value : new TileInfo(tile, color, state);
        tilesInfo[pos] = tileInfo;
    }

    public void SetTileState(Vector3Int pos, TileState state)
    {
        if(GetTileState(pos) == TileState.None)
            return;
        TileInfo tileInfo = tilesInfo[pos];
        tileInfo.state = state;
        tilesInfo[pos] = tileInfo;
    }

    public TileState GetTileState(Vector3Int pos)
    {
        return tilesInfo.TryGetValue(pos, out var info) ? info.state : TileState.None;
    }

    public bool IsTileDestroyed(Vector3Int pos)
    {
        return GetTileState(pos) == TileState.Destroyed;
    }

    public bool IsTileExist(Vector3Int pos)
    {
        return GetTileState(pos) != TileState.None;
    }
}