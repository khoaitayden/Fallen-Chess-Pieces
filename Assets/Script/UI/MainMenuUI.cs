// Create new script: MenuUI.cs
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playerVsPlayerButton;
    [SerializeField] private Button playerVsAIButton;
    [SerializeField] private Button onlineButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // Add listeners to each button to call the appropriate method when clicked.
        playerVsPlayerButton.onClick.AddListener(OnPlayerVsPlayerClicked);
        playerVsAIButton.onClick.AddListener(OnPlayerVsAIClicked);
        onlineButton.onClick.AddListener(OnOnlineClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayerVsPlayerClicked()
    {
        // Tell the GameManager to start a new game in Local mode.
        GameManager.Instance.StartNewGame(GameMode.Local);
    }

    private void OnPlayerVsAIClicked()
    {
        // Tell the GameManager to start a new game in AI mode.
        GameManager.Instance.StartNewGame(GameMode.AI);
    }

    private void OnOnlineClicked()
    {
        // This is the placeholder. For now, it can just show a message.
        Debug.Log("Online mode is not yet implemented.");
        // You could also disable this button or show a "Coming Soon" popup.
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}