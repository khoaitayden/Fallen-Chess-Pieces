using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private ChessControls _chessControls;
    [SerializeField] private SettingsUI settingsPanel;

    private void Awake()
    {
        _chessControls = new ChessControls();
    }

    private void OnEnable()
    {
        _chessControls.Enable();
        _chessControls.Player.Menu.performed += HandleMenuToggle;
    }

    private void OnDisable()
    {
        _chessControls.Player.Menu.performed -= HandleMenuToggle;
        _chessControls.Disable();
    }

    private void HandleMenuToggle(InputAction.CallbackContext context)
    {

        if (settingsPanel != null)
        {
            settingsPanel.ToggleVisibility();
        }
    }
}