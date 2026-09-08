using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent<Piece> OnPiecePlacing;

    public void Initialize(Piece pieceRef, PieceManager pieceManager, GridManager gridManager)
    {
        piece = pieceRef;
        this.pieceManager = pieceManager;
        this.gridManager = gridManager;
        mainCamera = Camera.main;
        //if (OnPiecePlacing == null)
            //OnPiecePlacing = new UnityEvent<Piece>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (piece == null || mainCamera == null) return;
        if (piece.isActive == false)
            return;

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
        //gridManager.PlacePiece(piece);
        if (!gridManager.PlacePiece(piece))
            piece.SetPosition(startAnchorPosition, null);
        //OnPiecePlaced.Invoke(piece); // add eventListener Remove if init here
    }
}