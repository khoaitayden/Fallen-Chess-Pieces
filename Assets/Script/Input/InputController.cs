using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private Camera mainCamera;

    private ChessControls _chessControls;

    public event Action<Vector2Int> OnBoardClick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        _chessControls = new ChessControls();
    }

    private void OnEnable()
    {
        _chessControls.Enable();
        _chessControls.Player.Click.performed += HandleClick;
    }

    private void OnDisable()
    {
        _chessControls.Player.Click.performed -= HandleClick;
        _chessControls.Disable();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = _chessControls.Player.PointerPosition.ReadValue<Vector2>();
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IClickable clickable))
            {
                OnBoardClick?.Invoke(clickable.GetBoardPosition());
            }
        }
        else
        {
            OnBoardClick?.Invoke(new Vector2Int(-1, -1));
        }
    }
}