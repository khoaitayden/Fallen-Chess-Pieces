using UnityEngine;
using TMPro; 

public class GameplayUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI turnIndicatorText;
    [SerializeField] private TextMeshProUGUI whiteChecked;
    [SerializeField] private TextMeshProUGUI blackChecked;
    [SerializeField] private TextMeshProUGUI whiteTimerText;
    [SerializeField] private TextMeshProUGUI blackTimerText;

    private void Update()
    {
        UpdateTurnIndicator();
        UpdateStatusText();
        UpdateTimers();
    }

    private void UpdateTurnIndicator()
    {
        if (TurnManager.Instance.IsWhiteTurn)
        {
            turnIndicatorText.text = "White's Turn";
            turnIndicatorText.color = Color.white;
            turnIndicatorText.outlineColor = Color.black;
        }
        else
        {
            turnIndicatorText.text = "Black's Turn";
            turnIndicatorText.color = Color.black;
            turnIndicatorText.outlineWidth = 0.3f;
            turnIndicatorText.outlineColor = Color.white;
        }
    }

    private void UpdateStatusText()
    {
        whiteChecked.text = MoveValidator.Instance.IsInCheck(true) ? "Checked!" : "";
        blackChecked.text = MoveValidator.Instance.IsInCheck(false) ? "Checked!" : "";
    }

    private void UpdateTimers()
    {
        if (TurnManager.Instance != null)
        {
            whiteTimerText.text = FormatTime(TurnManager.Instance.WhiteTime);
            blackTimerText.text = FormatTime(TurnManager.Instance.BlackTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}