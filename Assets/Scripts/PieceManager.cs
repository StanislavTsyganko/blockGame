using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        if (currentLevel.availablePieces != null && currentLevel.availablePieces.Length > 0)
            availablePieces.AddRange(currentLevel.availablePieces);
        else
            return;
    }

    public void GeneratePieces()
    {
        ClearAll();
        float targetCellsCount = 0;
        foreach (var (tile, count) in gridManager.uniqTiles)
            targetCellsCount += count;
        targetCellsCount *= 1.5f;
        int currentPieceCellsCount = 0;
        // todo checkIfPieceIsolated -> may be more pieces needed
        for (int i = 0; i < maxPieces || currentPieceCellsCount < targetCellsCount; i++)
        {
            PieceData currPiece;
            if (currentLevel.randomPieces)
                currPiece = GetRandomPiece();
            else
                currPiece = availablePieces[i % availablePieces.Count];

            if (currPiece != null)
            {
                currentPieceCellsCount += currPiece.BlockCount;
                activePieces.Add(currPiece);
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
        if (activePiece == null)
        {
            if(activePieces.Count == 0)
            {
                if (nextPiece == null)
                {
                    Debug.Log("No more pieces!!!");
                    return;
                }
            }

            if (nextPiece != null)
            {
                activePiece = nextPiece;
                activePiece.SetPosition(currentPieceSpawnOffset);
                if (activePieces.Count > 0)
                {
                    int pieceDataIndex;
                    if(currentLevel.randomPieces)
                        pieceDataIndex = Random.Range(0, activePieces.Count - 1);
                    else
                        pieceDataIndex = 0;
                    PieceData pieceData = activePieces[pieceDataIndex];
                    nextPiece = SpawnPiece(pieceData, nextPieceSpawnOffset);
                    activePieces.RemoveAt(pieceDataIndex);
                    nextPiece.SetVisible(true);
                    nextPiece.SetScale(new Vector3(0.5f, 0.5f, 1f));
                }
                else
                    nextPiece = null;
            }
            else
            {
                int pieceDataIndex;
                if (currentLevel.randomPieces)
                    pieceDataIndex = Random.Range(0, activePieces.Count - 1);
                else
                    pieceDataIndex = 0;
                PieceData pieceData = activePieces[pieceDataIndex];
                activePiece = SpawnPiece(pieceData, currentPieceSpawnOffset);
                activePieces.RemoveAt(pieceDataIndex);
                if (activePieces.Count > 0)
                {
                    int nextPieceDataIndex;
                    if (currentLevel.randomPieces)
                        nextPieceDataIndex = Random.Range(0, activePieces.Count - 1);
                    else
                        nextPieceDataIndex = 0;
                    PieceData nextPieceData = activePieces[nextPieceDataIndex];
                    nextPiece = SpawnPiece(nextPieceData, nextPieceSpawnOffset);
                    activePieces.RemoveAt(nextPieceDataIndex);
                    nextPiece.SetVisible(true);
                    nextPiece.SetScale(new Vector3(0.5f, 0.5f, 1f));
                }
                else
                    nextPiece = null;
            }
        }
        if (activePiece != null)
        {
            activePiece.SetActive(true);
            activePiece.SetScale(new Vector3(1f, 1f, 1f));
        }
        else
        {
            Debug.LogError("activePiece NULL !!! activePieces.Count = " + activePieces.Count);
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

    public void PlacePiece(Piece piece)
    {

        piece.Place();
        activePiece = null;
    }

    public Piece GetActivePiece() => activePiece;

    public int GetActivePieceCount() 
    {
        int count = activePieces.Count;
        if (activePiece != null)
            count++;
        if (nextPiece != null)
            count++;

        return count;
    }

    public void ClearAll()
    {
        if (activePiece != null)
        {
            activePiece.DestroyPiece();
            activePiece = null;
        }
        if (nextPiece != null)
        {
            nextPiece.DestroyPiece();
            nextPiece = null;
        }
        activePieces.Clear();
    }
}