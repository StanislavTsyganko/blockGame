using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceManager : MonoBehaviour
{
    [Header("References")]
    public LevelData currentLevel;
    public Tilemap targetTilemap;        
    public TileBase placedCellTile;             

    [Header("Sprites")]
    public Sprite cellSprite;             
    public Sprite piecePlacementFrame;    
    public float cellSize = 1f;
    public Vector3 spawnOffset = new Vector3(0, 3, 0);

    [Header("Level Settings")]
    public int maxPieces = 3;

    private List<PieceData> availablePieces = new List<PieceData>();
    private List<Piece> activePieces = new List<Piece>();
    private Piece activePiece;

    private void Start()
    {
        if (currentLevel == null)
        {
            Debug.LogError("[PieceManager] currentLevel = null!");
            return;
        }

        Initialize(currentLevel);
        SpawnFigurePlacementFrame();
    }

    public void Initialize(LevelData levelData)
    {
        currentLevel = levelData;
        availablePieces.Clear();

        if (currentLevel.availablePieces != null)
            availablePieces.AddRange(currentLevel.availablePieces);

        GeneratePieces();
    }

    public void GeneratePieces()
    {
        // Удаляем старые
        foreach (Piece piece in activePieces)
        {
            if (piece != null && !piece.isPlaced)
                Destroy(piece.pieceObj);
        }
        activePieces.Clear();

        for (int i = 0; i < maxPieces; i++)
        {
            PieceData randomPiece = GetRandomPiece();
            if (randomPiece != null)
            {
                Vector3 offset = new Vector3(i * 2.5f, 0, 1);
                Vector3 pos = spawnOffset + offset;

                Piece piece = SpawnPiece(randomPiece, pos);
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
        piece.Initialize(cellSprite, cellSize, this);

        return piece;
    }

    public GameObject SpawnFigurePlacementFrame()
    {
        if (piecePlacementFrame == null)
        {
            Debug.LogWarning("[PieceManager] piecePlacementFrame = null!");
            return null;
        }

        GameObject frame = new GameObject("Frame");
        SpriteRenderer sr = frame.AddComponent<SpriteRenderer>();
        sr.sprite = piecePlacementFrame;
        frame.transform.position = new Vector3(1, 1, 0);
        frame.transform.localScale = Vector3.one;

        return frame;
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
        PieceData pieceData = piece.pieceData;
        int[,] shape = pieceData.GetShapeMatrix();
        int w = pieceData.size.x;
        int h = pieceData.size.y;

        // Размещение
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (shape[x, y] == 1)
                {
                    Vector3Int pos = piece.GetGridPosition(targetTilemap) - piece.a - new Vector3Int(x, y, 0);
                    targetTilemap.SetTile(pos, placedCellTile);
                }

        piece.Place();

        activePieces.Remove(piece);
        Debug.Log($"[PieceManager] Фигура размещена: {piece.pieceData.name}");
        //GeneratePieces();

        return true;
    }

    // ============================================
    // ДОПОЛНИТЕЛЬНО
    // ============================================

    public Piece GetActivePiece() => activePiece;
}