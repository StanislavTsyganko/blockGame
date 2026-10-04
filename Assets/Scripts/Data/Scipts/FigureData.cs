using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "FigureData", menuName = "Scriptable Objects/FigureData")]
public class FigureData : ScriptableObject
{
    [Header("Layer 1 - Background / Grid")]
    public TileBase[] BackgroundTilesPaletteLayer;
    public List<TileData> BackgroundTilesLayer = new List<TileData>();

    [Header("Layer 2 - Target Shape / Figure")]
    public TileBase[] TargetTilesPaletteLayer;
    public List<TileData> TargetTilesLayer = new List<TileData>();

    public int rows;
    public int columns;

    public int figureId;

    public int TotalTiles => BackgroundTilesLayer.Count + TargetTilesLayer.Count;

    public void CopyData(FigureData original)
    {
        this.BackgroundTilesPaletteLayer = original.BackgroundTilesPaletteLayer;
        this.BackgroundTilesLayer = new List<TileData>(original.BackgroundTilesLayer);
        this.TargetTilesPaletteLayer = original.TargetTilesPaletteLayer;
        this.TargetTilesLayer = new List<TileData>(original.TargetTilesLayer);
        this.rows = original.rows;
        this.columns = original.columns;
    }

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
