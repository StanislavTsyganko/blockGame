using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Audio.ProcessorInstance;

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
    private Vector3 currentPieceSpawnOffset = new Vector3(0, 3, 1);
    private Vector3 nextPieceSpawnOffset = new Vector3(2, 3, 1);

    [Header("Level Settings")]
    public int maxPieces;
    private List<PieceData> availablePieces = new List<PieceData>();

    public List<PieceData> activePieces = new List<PieceData>();
    private Piece activePiece;
    private Piece nextPiece;

    public void Initialize(LevelData levelData, GridManager gridManager, Vector3 currentPieceSpawnOffset, Vector3 nextPieceSpawnOffset)
    {
        currentLevel = levelData;
        this.gridManager = gridManager;
        this.currentPieceSpawnOffset = currentPieceSpawnOffset;
        this.nextPieceSpawnOffset = nextPieceSpawnOffset;
        maxPieces = currentLevel.maxPieces;
        availablePieces.Clear();

        if (currentLevel.availablePieces != null)
            availablePieces.AddRange(currentLevel.availablePieces);

        GeneratePieces();
    }

    public void GeneratePieces()
    {
        // Удаляем старые
        if(activePiece != null)
            activePiece.DestroyPiece();
        if(nextPiece != null)
            nextPiece.DestroyPiece();
        activePieces.Clear();

        int targetCellsCount = currentLevel.BackgroundTilesLayer.Count - currentLevel.TargetTilesLayer.Count;
        int currentPieceCellsCount = 0;
        // todo checkIfPieceIsolated -> may be more pieces needed
        for (int i = 0; i < maxPieces || currentPieceCellsCount < targetCellsCount * 1.2; i++)
        {
            PieceData randomPiece = GetRandomPiece();
            if (randomPiece != null)
            {
                currentPieceCellsCount += randomPiece.BlockCount;
                activePieces.Add(randomPiece);
            }
        }
    }

    public Piece SpawnPiece(PieceData pieceData, Vector3 position)
    {
        if (pieceData == null) return null;

        Piece piece = new Piece(pieceData);
        piece.pieceObj.transform.position = position;

        piece.Initialize(cellSprite, cellSize, this, gridManager);

        return piece;
    }

    public void ResolveCurrentPiece()
    {
        if(activePiece == null)
        {
            if(activePieces.Count == 0)
            {
                Debug.Log("activePieces.Count == 0");
                return;
            }

            int pieceDataIndex = Random.Range(0, activePieces.Count - 1);
            PieceData pieceData = activePieces[pieceDataIndex];
            if (nextPiece != null)
            {
                activePiece = nextPiece;
                activePiece.SetPosition(currentPieceSpawnOffset);
                nextPiece = SpawnPiece(pieceData, nextPieceSpawnOffset);
                activePieces.RemoveAt(pieceDataIndex);
                nextPiece.SetVisible(true);
                nextPiece.SetScale(new Vector3(0.5f, 0.5f, 1f));
            }
            else 
            {
                activePiece = SpawnPiece(pieceData, currentPieceSpawnOffset);
                activePieces.RemoveAt(pieceDataIndex);
                if (activePieces.Count > 0)
                {
                    int nextPieceDataIndex = Random.Range(0, activePieces.Count);
                    PieceData nextPieceData = activePieces[nextPieceDataIndex];
                    nextPiece = SpawnPiece(nextPieceData, nextPieceSpawnOffset);
                    activePieces.RemoveAt(nextPieceDataIndex);
                    nextPiece.SetVisible(true);
                    nextPiece.SetScale(new Vector3(0.5f, 0.5f, 1f));
                }
            }
        }
        activePiece.SetActive(true);
        activePiece.SetScale(new Vector3(1f, 1f, 1f));
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

    public void PlacePiece(Piece piece)
    {

        piece.Place();
        activePiece = null;
    }

    public Piece GetActivePiece() => activePiece;
}