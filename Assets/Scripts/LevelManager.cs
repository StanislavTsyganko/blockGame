using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

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
    public TextMeshProUGUI endGameText;

    [Header("Animation")]
    public float tileAppearDelay = 0.03f;
    public bool animateTiles = true;

    //private void Start()
    //{
    //    if (currentLevel != null)
    //        ResolveCurrentLevel(currentLevel);
    //}

    private void Start()
    {
        if (currentLevel != null)
            LoadLevel(currentLevel);
    }

    public void LoadLevel(LevelData levelData)
    {
        currentLevel = levelData;

        if (!gridManager)
            return;
        gridManager.Initialize(currentLevel);
        cameraScaler.Initialize(currentLevel, gridManager, _UIManager);

        gridManager.OnPiecePlaced.AddListener(HoldPiecePlacedEvent);
        gridManager.SpawnGrid();

        // wait for gridManager.loaded

        if (pieceManager == null)
            Debug.LogError("pieceManager не установлен для LevelManager");
        else
            pieceManager.Initialize(currentLevel, gridManager, cameraScaler.placementObject.transform.position);
    }

    public void HoldPiecePlacedEvent(Piece piece)
    {
        bool placed = pieceManager.PlacePiece(piece);
        if (CheckIfPassed())
        {
            endGameText.text = "Win!";
            endGameText.gameObject.SetActive(true);
        }
        if (CheckIfFailed())
        {
            endGameText.text = "Lose";
            endGameText.gameObject.SetActive(true);
        }

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
        int activePiecesCount = pieceManager.activePieces.Count();

        if (notDectroyedCount > 0 && activePiecesCount == 0)
            return true;
        return false;
    }
}