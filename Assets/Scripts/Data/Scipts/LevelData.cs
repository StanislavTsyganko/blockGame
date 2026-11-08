using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game levels/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Layer 1 - Background / Grid")]
    public FigureData figureData;
    public bool isActive = false;

    [Header("Available Figures")]
    public PieceData[] availablePieces;
    public int maxPieces;
    public bool randomPieces = false;

    public TileBase[] BackgroundTilesPaletteLayer => figureData.BackgroundTilesPaletteLayer;
    public List<TileData> BackgroundTilesLayer => figureData.BackgroundTilesLayer;
    public TileBase[] TargetTilesPaletteLayer => figureData.TargetTilesPaletteLayer;
    public List<TileData> TargetTilesLayer => figureData.TargetTilesLayer;

    public void CopyData(LevelData original)
    {
        this.figureData = ScriptableObject.CreateInstance<FigureData>();
        this.figureData.CopyData(original.figureData);
        this.availablePieces = original.availablePieces;
        this.maxPieces = original.maxPieces;
        this.isActive = original.isActive;
        this.randomPieces = original.randomPieces;
    }
}