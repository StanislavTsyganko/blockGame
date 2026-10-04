using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game levels/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Layer 1 - Background / Grid")] // todo  Доабвть isActive -  и редактор для  ЛЕвелДаата для валидации уровня
    public FigureData figureData;

    [Header("Available Figures")]
    public PieceData[] availablePieces;
    public int maxPieces;

    [Header("Win Conditions")]
    public int targetScore = 1000;
    public int movesLimit = 30;

    public int levelId;
    public string levelDificulty;

    public TileBase[] BackgroundTilesPaletteLayer => figureData.BackgroundTilesPaletteLayer;
    public List<TileData> BackgroundTilesLayer => figureData.BackgroundTilesLayer;
    public TileBase[] TargetTilesPaletteLayer => figureData.TargetTilesPaletteLayer;
    public List<TileData> TargetTilesLayer => figureData.TargetTilesLayer;

    public void CopyData(LevelData original)
    {
        this.figureData = new FigureData();
        this.figureData.CopyData(original.figureData);
        this.availablePieces = original.availablePieces;
        this.maxPieces = original.maxPieces;
        this.targetScore = original.targetScore;
        this.movesLimit = original.movesLimit;
        this.levelDificulty = original.levelDificulty;
    }
}