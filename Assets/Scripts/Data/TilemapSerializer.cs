using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class TilemapSerializer
{
    [System.Serializable]
    public struct ExportResult
    {
        public TileBase[] palette;
        public List<TileData> tiles;
        public int rows;
        public int columns;
    }

    /// <summary>
    /// Ёкспорт одного Tilemap в данные
    /// </summary>
    public static ExportResult Export(Tilemap tilemap)
    {
        ExportResult result = new ExportResult
        {
            tiles = new List<TileData>(),
            palette = new TileBase[0]
        };

        if (tilemap == null) return result;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        List<TileBase> palette = new List<TileBase>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    int id = palette.IndexOf(tile);
                    if (id == -1) { palette.Add(tile); id = palette.Count - 1; }

                    result.tiles.Add(new TileData
                    {
                        position = new Vector3Int(bounds.x + x, bounds.y + y, 0),
                        tileID = id
                    });
                }
            }
        }

        result.palette = palette.ToArray();
        result.rows = bounds.size.y;
        result.columns = bounds.size.x;

        return result;
    }

    /// <summary>
    /// »мпорт одного сло€ в Tilemap
    /// </summary>
    public static void Import(Tilemap tilemap, TileBase[] palette, List<TileData> tiles)
    {
        if (tilemap == null || tiles == null) return;

        tilemap.ClearAllTiles();
        foreach (TileData tile in tiles)
        {
            if (tile.tileID >= 0 && tile.tileID < palette.Length)
            {
                tilemap.SetTile(tile.position, palette[tile.tileID]);
            }
        }
    }

    /// <summary>
    /// Ёкспорт всех Tilemap в LevelData (по массиву)
    /// </summary>
    public static void ExportAll(Tilemap[] tilemaps, LevelData levelData)
    {
        if (tilemaps == null || tilemaps.Length == 0) return;

        // ќчищаем старые данные
        levelData.ClearAllData();

        for (int i = 0; i < tilemaps.Length; i++)
        {
            ExportResult result = Export(tilemaps[i]);

            // ƒобавл€ем данные в LevelData по индексу сло€
            levelData.AddLayerData(i, result.palette, result.tiles);

            // –азмеры сетки (берЄм из первого непустого сло€)
            if (levelData.rows == 0 && levelData.columns == 0 && result.rows > 0 && result.columns > 0)
            {
                levelData.rows = result.rows;
                levelData.columns = result.columns;
            }
        }
    }

    /// <summary>
    /// »мпорт всех слоЄв из LevelData в Tilemap
    /// </summary>
    public static void ImportAll(Tilemap[] tilemaps, LevelData levelData)
    {
        if (tilemaps == null || tilemaps.Length == 0) return;

        for (int i = 0; i < tilemaps.Length; i++)
        {
            if (i < tilemaps.Length && tilemaps[i] != null)
            {
                Import(tilemaps[i], levelData.GetPalette(i), levelData.GetTiles(i));
            }
        }
    }

    /// <summary>
    /// Ёкспорт всех Tilemap в LevelData (с автоматическим определением количества слоЄв)
    /// </summary>
    public static void ExportAllFromFields(Tilemap layer1, Tilemap layer2, params Tilemap[] additionalLayers)
    {
        // —обираем все Tilemap в массив
        List<Tilemap> allLayers = new List<Tilemap>();
        if (layer1 != null) allLayers.Add(layer1);
        if (layer2 != null) allLayers.Add(layer2);
        if (additionalLayers != null)
        {
            foreach (var layer in additionalLayers)
            {
                if (layer != null) allLayers.Add(layer);
            }
        }

        // TODO: передавать LevelData в метод
        // ExportAll(allLayers.ToArray(), levelData);
    }
}