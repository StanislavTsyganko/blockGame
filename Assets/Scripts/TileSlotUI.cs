using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileSlotUI : MonoBehaviour
{
    [SerializeField] private Image tileImage;
    [SerializeField] private TextMeshProUGUI countText;
    public TileBase tileBase;
    public Color tileColor;

    public void Setup(TileBase tileBase, Color tileColor, int count)
    {
        this.tileBase = tileBase;
        this.tileColor = tileColor;
        tileImage.sprite = GetSpriteFromTile(tileBase);
        tileImage.color = tileColor;
        countText.text = count.ToString();
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }

    Sprite GetSpriteFromTile(TileBase tileBase)
    {
        if (tileBase is Tile standardTile)
        {
            return standardTile.sprite;
        }
        return null;
    }
}
