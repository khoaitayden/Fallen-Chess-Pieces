using UnityEngine;
using UnityEngine.UI;

public class ChooseAIDifficultyUI : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
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
        GameManager.Instance.StartNewGame(GameMode.AI, difficulty);
    }

    private void OnBackButtonClicked()
    {
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