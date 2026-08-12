using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class PieceDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Piece piece;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 startAnchorPosition;
    private PieceManager pieceManager;
    private GridManager gridManager;
    private const float FIXED_Z = 1f;

    public void Initialize(Piece pieceRef, PieceManager manager = null)
    {
        piece = pieceRef;
        pieceManager = manager;
        mainCamera = Camera.main;
        gridManager = FindFirstObjectByType<GridManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (piece == null || mainCamera == null) return;

        isDragging = true;
        startAnchorPosition = piece.GetWorldPosition();

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, FIXED_Z)
        );
        mousePos.z = FIXED_Z;

        piece.SetPosition(mousePos, null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || piece == null || mainCamera == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, FIXED_Z)
        );
        mousePos.z = FIXED_Z;

        piece.SetPosition(mousePos, null);

        gridManager?.UpdatePreview(piece);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        if (gridManager != null)
            gridManager.ClearPreview();

        if (pieceManager != null && piece != null)
        {
            bool placed = pieceManager.PlacePiece(piece);
            if (!placed)
            {
                piece.SetPosition(startAnchorPosition, gridManager?.tilemap);
            }
        }
        else
        {
            piece?.SetPosition(startAnchorPosition, gridManager?.tilemap);
        }
    }
}