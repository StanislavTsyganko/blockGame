using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game levels/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Layer 1 - Background / Grid")]
    public TileBase[] tilePaletteLayer1;
    public List<TileData> tilesLayer1 = new List<TileData>();

    [Header("Layer 2 - Target Shape / Figure")]
    public TileBase[] tilePaletteLayer2;
    public List<TileData> tilesLayer2 = new List<TileData>();

    [Header("Available Figures")]
    public PieceData[] availablePieces;

    [Header("Win Conditions")]
    public int targetScore = 1000;
    public int movesLimit = 30;

    [Header("Grid Settings")]
    public int rows = 8;
    public int columns = 8;

    public int TotalTiles => tilesLayer1.Count + tilesLayer2.Count;

    public void ClearAllData()
    {
        tilesLayer1.Clear();
        tilePaletteLayer1 = new TileBase[0];
        tilesLayer2.Clear();
        tilePaletteLayer2 = new TileBase[0];
        rows = 0;
        columns = 0;
    }

    // Ìועמהû הכÿ סמגלוסעטלמסעט ס TilemapSerializer.ExportAll/ImportAll
    public void AddLayerData(int index, TileBase[] palette, List<TileData> tiles)
    {
        switch (index)
        {
            case 0:
                tilePaletteLayer1 = palette;
                tilesLayer1 = tiles;
                break;
            case 1:
                tilePaletteLayer2 = palette;
                tilesLayer2 = tiles;
                break;
        }
    }

    public TileBase[] GetPalette(int index)
    {
        switch (index)
        {
            case 0: return tilePaletteLayer1;
            case 1: return tilePaletteLayer2;
            default: return new TileBase[0];
        }
    }

    public List<TileData> GetTiles(int index)
    {
        switch (index)
        {
            case 0: return tilesLayer1;
            case 1: return tilesLayer2;
            default: return new List<TileData>();
        }
    }
}