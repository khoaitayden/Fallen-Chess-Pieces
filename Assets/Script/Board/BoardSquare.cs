using UnityEngine;

public class BoardSquare : MonoBehaviour, IClickable
{
    private Vector2Int _boardPosition;
    [SerializeField] private GameObject highlightOverlay;
    private SpriteRenderer _highlightRenderer;

    private void Awake()
    {
        if (highlightOverlay != null)
        {
            _highlightRenderer = highlightOverlay.GetComponent<SpriteRenderer>();
        }
        SetHighlight(false, Color.clear);
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

    public void SetHighlight(bool isActive, Color color)
    {
        if (highlightOverlay != null && _highlightRenderer != null)
        {
            highlightOverlay.SetActive(isActive);
            if (isActive)
            {
                _highlightRenderer.color = color;
            }
            else
            {
                _highlightRenderer.color = Color.clear;
            }
        }
    }

    public void ClearHighlight()
    {
        SetHighlight(false, Color.clear);
    }
}