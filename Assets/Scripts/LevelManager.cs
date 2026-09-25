using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Audio.ProcessorInstance;

public class LevelManager : MonoBehaviour
{
    [Header("Managers")]
    public PieceManager pieceManager;
    public GridManager gridManager;
    public CameraScaler cameraScaler;
    [SerializeField] public UIManager _UIManager;

    [Header("References")]
    public Tilemap backgroundTilemapLayer;
    public Tilemap targetTilemapLayer;
    public LevelData currentLevel;
    public LevelData mainMenuLevel;

    [Header("Animation")]
    public float tileAppearDelay = 0.03f;
    public bool animateTiles = true;

    private void Start()
    {
        _UIManager.ShowLoading();
        if (currentLevel == null)
            ResolveCurrentLevel();
        if (!currentLevel)
            Debug.LogError("Ошибка загркузки текущего уровня");
        LoadMainMenu();
        _UIManager.ShowMainMenu();
    }

    public void OnPlayButtonClickedHandler()
    {
        if (currentLevel == null) 
            ResolveCurrentLevel();
        LoadLevel();
    }

    public void LoadMainMenu()
    {
        if (!mainMenuLevel)
            return;
        TileBase[] backgroundPalette;
        TileBase[] targetPalette;

        LevelData mainMenuLevelCopy = ScriptableObject.CreateInstance<LevelData>();
        mainMenuLevelCopy.CopyData(mainMenuLevel);

        if (currentLevel != null)
        {
            backgroundPalette = currentLevel.BackgroundTilesPaletteLayer;
            int backgroundTilesCount = currentLevel.BackgroundTilesLayer.Count;
            TileData randomTileData = currentLevel.BackgroundTilesLayer[Random.Range(0, backgroundTilesCount)];
            for (int i = 0; i < mainMenuLevelCopy.BackgroundTilesLayer.Count; i++)
            {
                mainMenuLevelCopy.BackgroundTilesLayer[i].color = randomTileData.color;
                mainMenuLevelCopy.BackgroundTilesLayer[i].tileID = randomTileData.tileID;
            }
            targetPalette = currentLevel.TargetTilesPaletteLayer;
            int targetTilesCount = currentLevel.TargetTilesLayer.Count;
            randomTileData = currentLevel.TargetTilesLayer[Random.Range(0, targetTilesCount)];
            for (int i = 0; i < mainMenuLevelCopy.TargetTilesLayer.Count; i++)
            {
                mainMenuLevelCopy.TargetTilesLayer[i].color = randomTileData.color;
                mainMenuLevelCopy.TargetTilesLayer[i].tileID = randomTileData.tileID;
            }
        }
        else
        {
            backgroundPalette = mainMenuLevel.BackgroundTilesPaletteLayer;
            targetPalette = mainMenuLevel.TargetTilesPaletteLayer;
        }
        mainMenuLevelCopy.figureData.BackgroundTilesPaletteLayer = backgroundPalette;
        mainMenuLevelCopy.figureData.TargetTilesPaletteLayer = targetPalette;
        gridManager.Initialize(mainMenuLevelCopy.figureData);
        cameraScaler.Initialize(gridManager, _UIManager);
        cameraScaler.AdaptCameraToLevel(mainMenuLevelCopy.figureData, true); // todo add adapt to menu //todo to LevelData add level type - уже сделано. соталось легаси использование удаленных levelData полей перенесенных в FigureData обновить на использование FigureData
        gridManager.SpawnGrid();
    }

    public void LoadLevel(LevelData levelData = null)
    {
        if (levelData == null)
            ResolveCurrentLevel();
        else
            currentLevel = levelData;

        gridManager.Initialize(currentLevel.figureData);
        //cameraScaler.Initialize(currentLevel, gridManager, _UIManager);
        cameraScaler.AdaptCameraToLevel(currentLevel.figureData);
        pieceManager.Initialize(currentLevel, gridManager, _UIManager.placementObject.transform.position, _UIManager.placementObject.transform.position + new Vector3(5,0,0)); //todo get next spawn position + level mode globalizating

        gridManager.SpawnGrid();
    }

    public void HoldPiecePlacedEvent(Piece piece)
    {
        if (CheckIfPassed())
        {
            _UIManager.OnWin();
            return;
        }
        if (CheckIfFailed())
        {
            _UIManager.OnLose();
            return;
        }
        pieceManager.ResolveCurrentPiece();
    }

    public void ResolveCurrentLevel() // todo add level from memory
    {

    }

    public bool CheckIfPassed()
    {
        int notDectroyedCount = gridManager.GetNotDestroyedBackgorund().Count();

        if(notDectroyedCount == 0)
           return true;
        return false;
    }

    public bool CheckIfFailed()
    {
        int notDectroyedCount = gridManager.GetNotDestroyedBackgorund().Count();
        int activePiecesCount = pieceManager.GetActivePieceCount();

        if (notDectroyedCount > 0 && activePiecesCount == 0)
            return true;
        return false;
    }

    public void ClearAll()
    {
        gridManager.ClearTilemaps();
        pieceManager.ClearAll();
        _UIManager.ClearAll();
    }
}