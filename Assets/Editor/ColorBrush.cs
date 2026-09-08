using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Color Brush", menuName = "Brushes/Color Brush")]
[CustomGridBrush(false, true, false, "Color Brush")]
public class ColorBrush : GridBrush
{
    public Color paintColor = Color.white;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        // Строгая проверка: рисуем только если выбран целевой Tilemap
        if (brushTarget == null) return;

        Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            // Получаем тайл, который сейчас выбран в вашей Tile Palette
            // (используем ячейку кисти 0,0,0 чтобы избежать спавна паттернов)
            TileBase currentTile = cells[0].tile;

            if (currentTile != null)
            {
                // Устанавливаем строго один тайл в целевую позицию
                tilemap.SetTile(position, currentTile);

                // Снимаем блокировку цвета и перекрашиваем
                tilemap.SetTileFlags(position, TileFlags.None);
                tilemap.SetColor(position, paintColor);
            }
        }
    }

    // Этот метод отвечает за то, чтобы старые тайлы стирались корректно (инструмент ЛАСТИК)
    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null) return;
        Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            tilemap.SetTile(position, null);
        }
    }
}
