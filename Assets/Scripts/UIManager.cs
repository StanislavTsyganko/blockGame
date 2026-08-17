using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _backgroundTilemap;
    public Sprite piecePlacementFrame;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject SpawnFigurePlacementFrame(float height, float width, Vector3 position)
    {
        if (piecePlacementFrame == null)
        {
            Debug.LogWarning("[PieceManager] piecePlacementFrame = null!");
            return null;
        }

        GameObject frame = new GameObject("Frame");
        SpriteRenderer sr = frame.AddComponent<SpriteRenderer>();
        sr.sprite = piecePlacementFrame;
        Vector2 spriteSize = sr.sprite.bounds.size;

        frame.transform.localScale = new Vector3(width / spriteSize.x, height / spriteSize.y, 1f);
        frame.transform.position = position;

        return frame;
    }
}
