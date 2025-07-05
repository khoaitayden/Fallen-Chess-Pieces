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
        // Instead of starting the game directly, show the difficulty selection panel.
        UIManager.Instance.ShowAIDifficultyPanel();
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