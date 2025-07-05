// Create new script: ChooseAIDifficultyUI.cs
using UnityEngine;
using UnityEngine.UI;

public class ChooseAIDifficultyUI : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button backButton; // Optional, but highly recommended

    private void Awake()
    {
        // Add listeners to each button. They all call the same method, but pass a different difficulty.
        easyButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Easy));
        normalButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Normal));
        hardButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Hard));

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void OnDifficultySelected(AIDifficulty difficulty)
    {
        // Tell the GameManager to start a new game with the chosen mode and difficulty.
        GameManager.Instance.StartNewGame(GameMode.AI, difficulty);
    }

    private void OnBackButtonClicked()
    {
        // Tell the main UIManager to go back to the main menu.
        UIManager.Instance.ShowMenuPanel();
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