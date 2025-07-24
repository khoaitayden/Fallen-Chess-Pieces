using UnityEngine;

public class BoardSquare : MonoBehaviour, IClickable
{
    private Vector2Int _boardPosition;
    [SerializeField] private GameObject highlightOverlay;
    private SpriteRenderer _highlightRenderer; // We'll get this once for efficiency

    private void Awake()
    {
        // Get the SpriteRenderer component from the overlay GameObject.
        if (highlightOverlay != null)
        {
            _highlightRenderer = highlightOverlay.GetComponent<SpriteRenderer>();
        }
        SetHighlight(false, Color.clear); // Start with highlight off
    }

    public void SetBoardPosition(Vector2Int position)
    {
        _boardPosition = position;
        gameObject.name = $"Square ({position.x}, {position.y})";
    }

    public Vector2Int GetBoardPosition()
    {
        return _boardPosition;
    }

    // This is the new, more powerful SetHighlight method.
    public void SetHighlight(bool isActive, Color color)
    {
        if (highlightOverlay != null && _highlightRenderer != null)
        {
            highlightOverlay.SetActive(isActive);
            if (isActive)
            {
                _highlightRenderer.color = color;
            }
        }
    }
}