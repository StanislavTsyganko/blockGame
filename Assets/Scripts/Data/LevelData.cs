using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game levels/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Layer 1 - Background / Grid")]
    public TileBase[] BackgroundTilesPaletteLayer;
    public List<TileData> BackgroundTilesLayer = new List<TileData>();

    [Header("Layer 2 - Target Shape / Figure")]
    public TileBase[] TargetTilesPaletteLayer;
    public List<TileData> TargetTilesLayer = new List<TileData>();

    [Header("Available Figures")]
    public PieceData[] availablePieces;
    public int maxPieces;

    [Header("Win Conditions")]
    public int targetScore = 1000;
    public int movesLimit = 30;

    [Header("Grid Settings")]
    public int rows;
    public int columns;

    public int levelId;
    public string levelDificulty;

    public int TotalTiles => BackgroundTilesLayer.Count + TargetTilesLayer.Count;

    public void ClearAllData()
    {
        BackgroundTilesLayer.Clear();
        BackgroundTilesPaletteLayer = new TileBase[0];
        TargetTilesLayer.Clear();
        TargetTilesPaletteLayer = new TileBase[0];
        rows = 0;
        columns = 0;
    }

    // Ìועמהû הכÿ סמגלוסעטלמסעט ס TilemapSerializer.ExportAll/ImportAll
    public void AddLayerData(int index, TileBase[] palette, List<TileData> tiles)
    {
        switch (index)
        {
            case 0:
                BackgroundTilesPaletteLayer = palette;
                BackgroundTilesLayer = tiles;
                break;
            case 1:
                TargetTilesPaletteLayer = palette;
                TargetTilesLayer = tiles;
                break;
        }
    }

    public TileBase[] GetPalette(int index)
    {
        switch (index)
        {
            case 0: return BackgroundTilesPaletteLayer;
            case 1: return TargetTilesPaletteLayer;
            default: return new TileBase[0];
        }
    }

    public List<TileData> GetTiles(int index)
    {
        switch (index)
        {
            case 0: return BackgroundTilesLayer;
            case 1: return TargetTilesLayer;
            default: return new List<TileData>();
        }
    }
}