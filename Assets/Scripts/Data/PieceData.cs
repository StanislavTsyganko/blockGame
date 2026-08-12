using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Game levels/PieceData")]
public class PieceData : ScriptableObject
{
    public Vector2Int size = new Vector2Int(3, 3);
    public List<bool> cells = new List<bool>();


    public Vector2Int anchorPosition = new Vector2Int(-1, -1);

    public bool HasAnchor => anchorPosition.x >= 0 && anchorPosition.y >= 0;

    // ============================================
    // лерндш
    // ============================================

    public int[,] GetShapeMatrix()
    {
        int[,] matrix = new int[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int index = y * size.x + x;
                matrix[x, y] = (index < cells.Count && cells[index]) ? 1 : 0;
            }
        }
        return matrix;
    }

    public int BlockCount
    {
        get
        {
            int count = 0;
            foreach (bool cell in cells)
                if (cell) count++;
            return count;
        }
    }

    public void Clear()
    {
        cells.Clear();
        for (int i = 0; i < size.x * size.y; i++)
            cells.Add(false);
    }

    public void ResizeCells()
    {
        int targetSize = size.x * size.y;
        while (cells.Count < targetSize) cells.Add(false);
        while (cells.Count > targetSize) cells.RemoveAt(cells.Count - 1);
    }

    private void OnValidate()
    {
        ResizeCells();
    }
}
