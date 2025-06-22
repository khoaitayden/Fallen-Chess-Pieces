using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    private Vector2Int _boardPosition;
    
    [SerializeField] private GameObject highlightOverlay;

    private void Awake()
    {
        SetHighlight(false);
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

    public void SetHighlight(bool isActive)
    {
        if (highlightOverlay != null)
        {
            highlightOverlay.SetActive(isActive);
        }
    }
}