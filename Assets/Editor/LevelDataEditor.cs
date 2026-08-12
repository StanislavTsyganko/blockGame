using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData levelData;

    [SerializeField] private Tilemap sourceTilemapBackgroundLayer;
    [SerializeField] private Tilemap sourceTilemapTargetFigureLayer;

    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;

    private void OnEnable()
    {
        levelData = (LevelData)target;
    }

    public override void OnInspectorGUI()
    {
        InitStyles();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("==== LEVEL SETTINGS ====", headerStyle);
        EditorGUILayout.Space(5);

        DrawDefaultInspector();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("==== TILEMAP TOOLS ====", headerStyle);
        EditorGUILayout.Space(5);

        // ============================================
        // LAYER 1
        // ============================================
        EditorGUILayout.LabelField("Layer 1 - Background/Grid", EditorStyles.boldLabel);
        sourceTilemapBackgroundLayer = (Tilemap)EditorGUILayout.ObjectField(
            "Tilemap",
            sourceTilemapBackgroundLayer,
            typeof(Tilemap),
            true
        );

        if (sourceTilemapBackgroundLayer != null)
        {
            EditorGUILayout.HelpBox(
                $"{sourceTilemapBackgroundLayer.name} — {GetTileCount(sourceTilemapBackgroundLayer)} тайлов",
                GetTileCount(sourceTilemapBackgroundLayer) > 0 ? MessageType.Info : MessageType.Warning
            );
        }

        // Кнопки для слоя 1
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📥 Load L1"))
        {
            ImportLayer1();
        }
        if (GUILayout.Button("📤 Export L1"))
        {
            ExportLayer1();
        }
        if (GUILayout.Button("🧹 Clear L1"))
        {
            ClearLayer1();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // ============================================
        // LAYER 2
        // ============================================
        EditorGUILayout.LabelField("Layer 2 - Target Shape/Figure", EditorStyles.boldLabel);
        sourceTilemapTargetFigureLayer = (Tilemap)EditorGUILayout.ObjectField(
            "Tilemap",
            sourceTilemapTargetFigureLayer,
            typeof(Tilemap),
            true
        );

        if (sourceTilemapTargetFigureLayer != null)
        {
            EditorGUILayout.HelpBox(
                $"{sourceTilemapTargetFigureLayer.name} — {GetTileCount(sourceTilemapTargetFigureLayer)} тайлов",
                GetTileCount(sourceTilemapTargetFigureLayer) > 0 ? MessageType.Info : MessageType.Warning
            );
        }

        // Кнопки для слоя 2
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📥 Load L2"))
        {
            ImportLayer2();
        }
        if (GUILayout.Button("📤 Export L2"))
        {
            ExportLayer2();
        }
        if (GUILayout.Button("🧹 Clear L2"))
        {
            ClearLayer2();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(15);

        // ============================================
        // ОБЩИЕ КНОПКИ
        // ============================================
        EditorGUILayout.LabelField("==== ALL LAYERS ====", headerStyle);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("📥 LOAD ALL LAYERS → TILEMAPS", GUILayout.Height(35)))
        {
            ImportAllLayers();
        }
        GUI.backgroundColor = Color.white;

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("📦 EXPORT ALL TILEMAPS → DATA", buttonStyle, GUILayout.Height(40)))
        {
            ExportAllLayers();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        if (GUILayout.Button("🗑️ Clear ALL Data", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Очистить все данные?",
                "Удалить все данные о тайлах?", "Да", "Отмена"))
            {
                levelData.ClearAllData();
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
            }
        }

        EditorGUILayout.Space(15);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🧹 Clear ALL Tilemaps", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Очистить все Tilemap?",
                "Вы уверены, что хотите очистить ВСЕ выбранные Tilemap?\n\n" +
                "Это действие необратимо!",
                "Да, очистить всё", "Отмена"))
            {
                ClearAllTilemaps();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(15);

        // ============================================
        // ИНФОРМАЦИЯ
        // ============================================
        EditorGUILayout.LabelField("==== LEVEL INFO ====", headerStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField($"📊 Layer 1: {levelData.tilesLayer1.Count} тайлов");
        EditorGUILayout.LabelField($"📊 Layer 2: {levelData.tilesLayer2.Count} тайлов");
        EditorGUILayout.LabelField($"📊 Всего: {levelData.TotalTiles}");
        EditorGUILayout.LabelField($"📐 Сетка: {levelData.rows} x {levelData.columns}");
        EditorGUILayout.LabelField($"🎯 Цель: {levelData.targetScore} очков");
    }

    // ============================================
    // МЕТОДЫ ДЛЯ LAYER 1
    // ============================================

    private void ExportLayer1()
    {
        if (sourceTilemapBackgroundLayer == null)
        {
            EditorUtility.DisplayDialog("Ошибка!", "Выберите Tilemap для Layer 1.", "OK");
            return;
        }

        var result = TilemapSerializer.Export(sourceTilemapBackgroundLayer);
        levelData.tilesLayer1 = result.tiles;
        levelData.tilePaletteLayer1 = result.palette;
        levelData.rows = result.rows;
        levelData.columns = result.columns;

        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();

        Debug.Log($"[LevelDataEditor] Экспортирован Layer 1: {result.tiles.Count} тайлов");
        EditorUtility.DisplayDialog("Готово!", $"✅ Layer 1 сохранён ({result.tiles.Count} тайлов)", "OK");
    }

    private void ImportLayer1()
    {
        if (sourceTilemapBackgroundLayer == null)
        {
            EditorUtility.DisplayDialog("Ошибка!", "Выберите Tilemap для Layer 1.", "OK");
            return;
        }

        if (levelData.tilesLayer1.Count == 0)
        {
            EditorUtility.DisplayDialog("Нет данных!", "Layer 1 пуст.", "OK");
            return;
        }

        TilemapSerializer.Import(sourceTilemapBackgroundLayer, levelData.tilePaletteLayer1, levelData.tilesLayer1);
        Debug.Log($"[LevelDataEditor] Загружен Layer 1: {levelData.tilesLayer1.Count} тайлов");
        EditorUtility.DisplayDialog("Готово!", $"✅ Layer 1 загружен ({levelData.tilesLayer1.Count} тайлов)", "OK");
    }

    private void ClearLayer1()
    {
        if (sourceTilemapBackgroundLayer == null) return;

        if (EditorUtility.DisplayDialog("Очистить Layer 1?",
            $"Очистить {sourceTilemapBackgroundLayer.name}?", "Да", "Отмена"))
        {
            sourceTilemapBackgroundLayer.ClearAllTiles();
            Debug.Log("[LevelDataEditor] Layer 1 очищен");
        }
    }

    // ============================================
    // МЕТОДЫ ДЛЯ LAYER 2
    // ============================================

    private void ExportLayer2()
    {
        if (sourceTilemapTargetFigureLayer == null)
        {
            EditorUtility.DisplayDialog("Ошибка!", "Выберите Tilemap для Layer 2.", "OK");
            return;
        }

        var result = TilemapSerializer.Export(sourceTilemapTargetFigureLayer);
        levelData.tilesLayer2 = result.tiles;
        levelData.tilePaletteLayer2 = result.palette;

        if (levelData.rows == 0 && levelData.columns == 0)
        {
            levelData.rows = result.rows;
            levelData.columns = result.columns;
        }

        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();

        Debug.Log($"[LevelDataEditor] Экспортирован Layer 2: {result.tiles.Count} тайлов");
        EditorUtility.DisplayDialog("Готово!", $"✅ Layer 2 сохранён ({result.tiles.Count} тайлов)", "OK");
    }

    private void ImportLayer2()
    {
        if (sourceTilemapTargetFigureLayer == null)
        {
            EditorUtility.DisplayDialog("Ошибка!", "Выберите Tilemap для Layer 2.", "OK");
            return;
        }

        if (levelData.tilesLayer2.Count == 0)
        {
            EditorUtility.DisplayDialog("Нет данных!", "Layer 2 пуст.", "OK");
            return;
        }

        TilemapSerializer.Import(sourceTilemapTargetFigureLayer, levelData.tilePaletteLayer2, levelData.tilesLayer2);
        Debug.Log($"[LevelDataEditor] Загружен Layer 2: {levelData.tilesLayer2.Count} тайлов");
        EditorUtility.DisplayDialog("Готово!", $"✅ Layer 2 загружен ({levelData.tilesLayer2.Count} тайлов)", "OK");
    }

    private void ClearLayer2()
    {
        if (sourceTilemapTargetFigureLayer == null) return;

        if (EditorUtility.DisplayDialog("Очистить Layer 2?",
            $"Очистить {sourceTilemapTargetFigureLayer.name}?", "Да", "Отмена"))
        {
            sourceTilemapTargetFigureLayer.ClearAllTiles();
            Debug.Log("[LevelDataEditor] Layer 2 очищен");
        }
    }

    // ============================================
    // МЕТОДЫ ДЛЯ ВСЕХ СЛОЁВ
    // ============================================

    private void ExportAllLayers()
    {
        Tilemap[] tilemaps = new Tilemap[] { sourceTilemapBackgroundLayer, sourceTilemapTargetFigureLayer };
        TilemapSerializer.ExportAll(tilemaps, levelData);

        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Готово!",
            $"✅ Экспортировано:\n" +
            $"Layer 1: {levelData.tilesLayer1.Count} тайлов\n" +
            $"Layer 2: {levelData.tilesLayer2.Count} тайлов",
            "OK");
    }

    private void ImportAllLayers()
    {
        Tilemap[] tilemaps = new Tilemap[] { sourceTilemapBackgroundLayer, sourceTilemapTargetFigureLayer };
        TilemapSerializer.ImportAll(tilemaps, levelData);

        EditorUtility.DisplayDialog("Готово!",
            $"✅ Загружено:\n" +
            $"Layer 1: {levelData.tilesLayer1.Count} тайлов\n" +
            $"Layer 2: {levelData.tilesLayer2.Count} тайлов",
            "OK");
    }

    private void ClearAllTilemaps()
    {
        bool hasAny = false;

        if (sourceTilemapBackgroundLayer != null)
        {
            sourceTilemapBackgroundLayer.ClearAllTiles();
            hasAny = true;
        }

        if (sourceTilemapTargetFigureLayer != null)
        {
            sourceTilemapTargetFigureLayer.ClearAllTiles();
            hasAny = true;
        }

        if (!hasAny)
        {
            EditorUtility.DisplayDialog("Ошибка!", "Нет выбранных Tilemap для очистки.", "OK");
            return;
        }

        Debug.Log("[LevelDataEditor] Все Tilemap очищены.");
        EditorUtility.DisplayDialog("Готово!", "✅ Все Tilemap очищены.", "OK");
    }

    // ============================================
    // ВСПОМОГАТЕЛЬНОЕ
    // ============================================

    private int GetTileCount(Tilemap tilemap)
    {
        if (tilemap == null) return 0;

        BoundsInt bounds = tilemap.cellBounds;
        int count = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                    count++;
            }
        }

        return count;
    }

    private void InitStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            ColorUtility.TryParseHtmlString("#2A9D8F", out Color green);
            buttonStyle.normal.background = MakeTex(2, 2, green);
        }
    }

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}