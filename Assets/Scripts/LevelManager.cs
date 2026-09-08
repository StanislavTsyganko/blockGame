using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    [Header("Animation")]
    public float tileAppearDelay = 0.03f;
    public bool animateTiles = true;

    private void Start()
    {
        _UIManager.ShowLoading();
        if (currentLevel == null)
            ResolveCurrentLevel();
        _UIManager.ShowMainMenu();
    }

    public void OnPlayButtonClickedHandler()
    {
        if (currentLevel == null) 
            ResolveCurrentLevel();
        LoadLevel();
    }

    public void LoadLevel(LevelData levelData = null)
    {
        if (levelData == null)
            ResolveCurrentLevel();
        else
            currentLevel = levelData;

        gridManager.Initialize(currentLevel);
        cameraScaler.Initialize(currentLevel, gridManager, _UIManager);
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