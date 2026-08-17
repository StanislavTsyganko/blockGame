using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PieceData))]
public class PieceDataEditor : Editor
{
    private PieceData pieceData;
    private Color backgroundColor = new Color(0.18f, 0.18f, 0.18f);
    private Color anchorColor = Color.green;

    private void OnEnable()
    {
        pieceData = (PieceData)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(5);

        // Размер сетки
        Vector2Int newSize = pieceData.size;
        newSize.x = EditorGUILayout.IntField("Width", newSize.x);
        newSize.y = EditorGUILayout.IntField("Height", newSize.y);
        newSize.x = Mathf.Clamp(newSize.x, 1, 8);
        newSize.y = Mathf.Clamp(newSize.y, 1, 8);

        if (newSize != pieceData.size)
        {
            pieceData.size = newSize;
            pieceData.ResizeCells();
            if (!pieceData.HasAnchor ||
                pieceData.anchorPosition.x >= newSize.x ||
                pieceData.anchorPosition.y >= newSize.y)
            {
                // Если якорь выходит за новые границы — сбрасываем
                pieceData.anchorPosition = new Vector2Int(-1, -1);
            }
            EditorUtility.SetDirty(pieceData);
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Clear", GUILayout.Width(80)))
        {
            pieceData.Clear();
            pieceData.anchorPosition = new Vector2Int(-1, -1);
            EditorUtility.SetDirty(pieceData);
        }

        EditorGUILayout.Space(10);

        // Отрисовка сетки
        DrawGrid();

        // Информация о якоре
        EditorGUILayout.Space(5);
        if (pieceData.HasAnchor)
            EditorGUILayout.LabelField($"Anchor: ({pieceData.anchorPosition.x}, {pieceData.anchorPosition.y})");
        else
            EditorGUILayout.LabelField("Anchor: not set (double-click on a filled cell)");

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Blocks: {pieceData.BlockCount}");

        if (GUI.changed)
        {
            EditorUtility.SetDirty(pieceData);
        }
    }

    private void DrawGrid()
    {
        int width = pieceData.size.x;
        int height = pieceData.size.y;

        float cellSize = Mathf.Min(40f, 300f / Mathf.Max(width, height));
        float gridWidth = width * cellSize;
        float gridHeight = height * cellSize;

        Rect gridRect = GUILayoutUtility.GetRect(gridWidth, gridHeight);
        gridRect.x = (gridRect.width - gridWidth) / 2f;
        gridRect.width = gridWidth;
        gridRect.height = gridHeight;

        EditorGUI.DrawRect(gridRect, backgroundColor);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                bool isFilled = index < pieceData.cells.Count && pieceData.cells[index];
                bool isAnchor = pieceData.HasAnchor && pieceData.anchorPosition.x == x && pieceData.anchorPosition.y == y;

                Rect cellRect = new Rect(
                    gridRect.x + x * cellSize,
                    gridRect.y + (height - 1 - y) * cellSize,
                    cellSize,
                    cellSize
                );

                // Цвет: якорь — зелёный, заполнено — белый, пусто — серый
                Color cellColor;
                if (isAnchor)
                    cellColor = anchorColor;
                else if (isFilled)
                    cellColor = Color.white;
                else
                    cellColor = new Color(0.3f, 0.3f, 0.3f);

                EditorGUI.DrawRect(cellRect, cellColor);
                Handles.DrawSolidRectangleWithOutline(cellRect, Color.clear, new Color(0.5f, 0.5f, 0.5f));

                // --- Обработка событий ---
                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.clickCount == 2 && isFilled)
                    {
                        // Двойной клик по заполненной клетке → устанавливаем якорь
                        pieceData.anchorPosition = new Vector2Int(x, y);
                        EditorUtility.SetDirty(pieceData);
                        Event.current.Use();
                        Repaint();
                        Debug.Log($"[PieceDataEditor] Anchor set to ({x}, {y})");
                    }
                    else if (Event.current.clickCount == 1)
                    {
                        // Одиночный клик — переключение состояния клетки (если не якорь)
                        if (!isAnchor)
                        {
                            Debug.Log(index);
                            Debug.Log(pieceData.cells);
                            pieceData.cells[index] = !isFilled;
                            EditorUtility.SetDirty(pieceData);
                            Event.current.Use();
                            Repaint();
                        }
                    }
                }

                // Подсветка при наведении
                if (cellRect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(cellRect, new Color(1f, 1f, 1f, 0.2f));
                    Repaint();
                }
            }
        }
    }
}