using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class PieceManager : MonoBehaviour
{
    [Header("References")]
    public LevelData currentLevel;
    private GridManager gridManager;
    public Tilemap targetTilemap;        
    public TileBase placedCellTile;             
    public TileBase backgroundCellTile;             

    [Header("Sprites")]
    public Sprite cellSprite;             
    public float cellSize = 1f;
    private Vector3 spawnOffset = new Vector3(0, 3, 1);

    [Header("Level Settings")]
    public int maxPieces;
    private List<PieceData> availablePieces = new List<PieceData>();

    public List<Piece> activePieces = new List<Piece>();
    private Piece activePiece;

    public void Initialize(LevelData levelData, GridManager gridManager, Vector3 figureSpawnPosition)
    {
        currentLevel = levelData;
        this.gridManager = gridManager;
        spawnOffset = figureSpawnPosition;
        maxPieces = currentLevel.maxPieces;
        availablePieces.Clear();

        if (currentLevel.availablePieces != null)
            availablePieces.AddRange(currentLevel.availablePieces);

        GeneratePieces();        
    }

    public void GeneratePieces() // after level loaded
    {
        // Удаляем старые
        foreach (Piece piece in activePieces)
        {
            if (piece != null && !piece.isPlaced)
                piece.DestroyPiece();
        }
        activePieces.Clear();

        int targetCellsCount = currentLevel.BackgroundTilesLayer.Count - currentLevel.TargetTilesLayer.Count;
        int currentPieceCellsCount = 0;
        // todo checkIfPieceIsolated -> may be more pieces needed
        // todo dont spawn all pieces until neead may be 2 (curr + next)
        for (int i = 0; i < maxPieces || currentPieceCellsCount < targetCellsCount * 1.2; i++)
        {
            PieceData randomPiece = GetRandomPiece();
            if (randomPiece != null)
            {
                //Vector3 offset = new Vector3(i * 2.5f, 0, 1);
                //Vector3 pos = spawnOffset + offset;

                Piece piece = SpawnPiece(randomPiece, spawnOffset);
                currentPieceCellsCount += piece.pieceData.BlockCount;
                if (piece != null)
                {
                    piece.SetActive(false);
                    activePieces.Add(piece);
                }
            }
        }

        // Активируем первую
        if (activePieces.Count > 0)
        {
            activePiece = activePieces[0];
            activePiece.SetActive(true);
            Debug.Log($"[PieceManager] Активна: {activePiece.pieceData.name}");
        }
    }

    public Piece SpawnPiece(PieceData pieceData, Vector3 position)
    {
        if (pieceData == null) return null;

        Piece piece = new Piece(pieceData);
        piece.pieceObj.transform.position = position;

        // Инициализируем визуал и передаём менеджер
        piece.Initialize(cellSprite, cellSize, this, gridManager);
        if(gridManager)
            piece.dragHandler.OnPiecePlacing.AddListener(gridManager.PlacePiece);

        return piece;
    }

    public void ResolveCurrentPiece()
    {
        if(activePiece == null)
        {
            if(activePieces.Count == 0)
            {
                Debug.Log(activePieces);
                return;
            }
            Piece piece = activePieces[Random.Range(0, activePieces.Count)];
            piece.SetActive(true);
            if (piece.isActive)
                activePiece = piece;
        }
    }

    public PieceData GetRandomPiece()
    {
        if (availablePieces.Count == 0)
        {
            Debug.LogWarning("[PieceManager] Нет доступных фигур!");
            return null;
        }
        return availablePieces[Random.Range(0, availablePieces.Count)];
    }

    public bool PlacePiece(Piece piece)
    {
        //List<Vector3Int> cellsPositions = piece.GetGridCellsPositions(targetTilemap);
        //foreach(Vector3Int cellPos in cellsPositions)
        //{
        //    TileBase targetTile = targetTilemap.GetTile(cellPos);
        //    if (targetTile != null && targetTile == backgroundCellTile)
        //        targetTilemap.SetTile(cellPos, placedCellTile);
        //}
        

        piece.Place();
        activePieces.Remove(piece);
        activePiece = null;
        ResolveCurrentPiece();
        Debug.Log($"[PieceManager] Фигура размещена: {piece.pieceData.name}");
        return true;
    }

    // ============================================
    // ДОПОЛНИТЕЛЬНО
    // ============================================

    public Piece GetActivePiece() => activePiece;
}