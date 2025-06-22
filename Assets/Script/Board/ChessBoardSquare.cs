using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    private Vector2Int _boardPosition;

    public void SetBoardPosition(Vector2Int position)
    {
        _boardPosition = position;
        gameObject.name = $"Square ({position.x}, {position.y})";
    }

    public Vector2Int GetBoardPosition()
    {
        return _boardPosition;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked on square: {_boardPosition}");
    }
}