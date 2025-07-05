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
        GameManager.Instance.StartNewGame(GameMode.Local);
    }

    private void OnPlayerVsAIClicked()
    {
        UIManager.Instance.ShowAIDifficultyPanel();
    }

    private void OnOnlineClicked()
    {
        Debug.Log("Online mode is not yet implemented.");
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