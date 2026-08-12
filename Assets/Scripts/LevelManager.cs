using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public Tilemap targetTilemapLayer1;
    public Tilemap targetTilemapLayer2;
    public LevelData currentLevel;

    [Header("Animation")]
    public float tileAppearDelay = 0.03f;
    public bool animateTiles = true;

    private void Start()
    {
        if (currentLevel != null)
            LoadLevel(currentLevel);
    }

    public void LoadLevel(LevelData levelData)
    {
        currentLevel = levelData;

        ClearTilemaps();

        if (animateTiles)
            StartCoroutine(AnimateLoadLevel(levelData));
        else
            InstantLoadLevel(levelData);

        Debug.Log($"[LevelManager] Уровень загружен: {levelData.targetScore} очков");
    }

    private void ClearTilemaps()
    {
        targetTilemapLayer1?.ClearAllTiles();
        targetTilemapLayer2?.ClearAllTiles();
    }

    private void InstantLoadLevel(LevelData data)
    {
        LoadLayer(targetTilemapLayer1, data.tilesLayer1, data.tilePaletteLayer1);
        LoadLayer(targetTilemapLayer2, data.tilesLayer2, data.tilePaletteLayer2);
    }

    private void LoadLayer(Tilemap tilemap, List<TileData> tiles, TileBase[] palette)
    {
        if (tilemap == null || tiles == null || palette == null) return;
        foreach (var tile in tiles)
            if (tile.tileID >= 0 && tile.tileID < palette.Length)
                tilemap.SetTile(tile.position, palette[tile.tileID]);
    }

    private IEnumerator AnimateLoadLevel(LevelData data)
    {
        var allTiles = new List<AnimatedTileData>();
        CollectTilesForAnimation(data, targetTilemapLayer1, data.tilesLayer1, data.tilePaletteLayer1, allTiles);
        CollectTilesForAnimation(data, targetTilemapLayer2, data.tilesLayer2, data.tilePaletteLayer2, allTiles);

        allTiles.Sort((a, b) =>
        {
            if (a.tile.position.y != b.tile.position.y)
                return a.tile.position.y.CompareTo(b.tile.position.y);
            return a.tile.position.x.CompareTo(b.tile.position.x);
        });

        foreach (var dataTile in allTiles)
        {
            if (dataTile.tilemap == null) continue;
            dataTile.tilemap.SetTile(dataTile.tile.position, dataTile.palette[dataTile.tile.tileID]);
            dataTile.tilemap.SetTileFlags(dataTile.tile.position, TileFlags.None);
            dataTile.tilemap.SetColor(dataTile.tile.position, Color.yellow);
            yield return new WaitForSeconds(tileAppearDelay);
            dataTile.tilemap.SetColor(dataTile.tile.position, Color.white);
        }

        Debug.Log($"[LevelManager] Анимация завершена. Тайлов: {allTiles.Count}");
    }

    private void CollectTilesForAnimation(LevelData data, Tilemap tilemap, List<TileData> tiles, TileBase[] palette, List<AnimatedTileData> list)
    {
        if (tilemap == null || tiles == null || palette == null) return;
        foreach (var tile in tiles)
            list.Add(new AnimatedTileData { tile = tile, tilemap = tilemap, palette = palette });
    }

    private class AnimatedTileData
    {
        public TileData tile;
        public Tilemap tilemap;
        public TileBase[] palette;
    }
}