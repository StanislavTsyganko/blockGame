using System.Linq;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(ColorBrush))]
public class ColorBrushEditor : GridBrushEditorBase
{
    // Этот код принудительно заставляет Unity показывать в списке Active Tilemap 
    // только те объекты на сцене, у которых есть компонент Tilemap
    public override GameObject[] validTargets
    {
        get
        {
            return Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None)
                         .Select(t => t.gameObject)
                         .ToArray();
        }
    }
}
