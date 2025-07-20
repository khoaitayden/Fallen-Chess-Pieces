using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI turnIndicatorText;
    [SerializeField] private TextMeshProUGUI whiteChecked;
    [SerializeField] private TextMeshProUGUI blackChecked;
    [SerializeField] private TextMeshProUGUI whiteTimerText;
    [SerializeField] private TextMeshProUGUI blackTimerText;

    [Header("Last Move Display")]
    [SerializeField] private TextMeshProUGUI moveNumberText;
    [SerializeField] private TextMeshProUGUI whiteLastMoveText;
    [SerializeField] private TextMeshProUGUI blackLastMoveText;

    [Header("Capture UI")]
    [SerializeField] private GameObject capturedPieceUIPrefab;
    [SerializeField] private Transform whiteCapturedPiecesContainer;
    [SerializeField] private Transform blackCapturedPiecesContainer;

    [Header("King Power UI")]
    [SerializeField] private GameObject kingPowerIconPrefab;
    [SerializeField] private Transform whiteKingPowersContainer;
    [SerializeField] private Transform blackKingPowersContainer;
    [SerializeField] private TextMeshProUGUI whiteKingLivesText;
    [SerializeField] private TextMeshProUGUI blackKingLivesText;

    private int moveCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        if (PieceCaptureManager.Instance != null)
            PieceCaptureManager.Instance.OnPieceCaptured += HandlePieceCaptured;
        if (MoveHistory.Instance != null)
            MoveHistory.Instance.OnMoveAdded += UpdateLastMoveDisplay;
        if (KingPowerManager.Instance != null)
        {
            KingPowerManager.Instance.OnPowerGained += HandlePowerGained;
            KingPowerManager.Instance.OnPowerLost += HandlePowerLost;
            KingPowerManager.Instance.OnExtraLifeGained += HandleExtraLifeGained;
        }
        ClearLastMoveDisplay();
        ClearKingPowerUI();
    }

    private void OnDestroy()
    {
        if (PieceCaptureManager.Instance != null)
            PieceCaptureManager.Instance.OnPieceCaptured -= HandlePieceCaptured;
        if (MoveHistory.Instance != null)
            MoveHistory.Instance.OnMoveAdded -= UpdateLastMoveDisplay;
        if (KingPowerManager.Instance != null)
        {
            KingPowerManager.Instance.OnPowerGained -= HandlePowerGained;
            KingPowerManager.Instance.OnPowerLost -= HandlePowerLost;
            KingPowerManager.Instance.OnExtraLifeGained -= HandleExtraLifeGained;
        }
    }

    private void Update()
    {
        UpdateTurnIndicator();
        UpdateStatusText();
        UpdateTimers();
    }

    private void UpdateTurnIndicator()
    {
        if (TurnManager.Instance == null) return;
        if (TurnManager.Instance.IsWhiteTurn)
        {
            turnIndicatorText.text = "White's Turn";
            turnIndicatorText.color = Color.white;
        }
        else
        {
            turnIndicatorText.text = "Black's Turn";
            turnIndicatorText.color = Color.black;
        }
    }

    private void UpdateStatusText()
    {
        if (MoveValidator.Instance == null) return;
        whiteChecked.enabled = MoveValidator.Instance.IsInCheck(true);
        blackChecked.enabled = MoveValidator.Instance.IsInCheck(false);
    }

    private void UpdateTimers()
    {
        if (TurnManager.Instance == null) return;
        whiteTimerText.text = FormatTime(TurnManager.Instance.WhiteTime);
        blackTimerText.text = FormatTime(TurnManager.Instance.BlackTime);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void HandlePieceCaptured(ChessPiece piece)
    {
        Transform container = piece.IsWhite ? blackCapturedPiecesContainer : whiteCapturedPiecesContainer;
        GameObject iconObject = Instantiate(capturedPieceUIPrefab, container);
        if (iconObject.TryGetComponent(out Image iconImage) && piece.TryGetComponent(out SpriteRenderer pieceSpriteRenderer))
        {
            iconImage.sprite = pieceSpriteRenderer.sprite;
        }
    }

    public void ClearCapturedPieceUI()
    {
        foreach (Transform child in whiteCapturedPiecesContainer) Destroy(child.gameObject);
        foreach (Transform child in blackCapturedPiecesContainer) Destroy(child.gameObject);
    }

    private void HandlePowerGained(bool isWhiteKing, PieceType powerType)
    {
        Transform container = isWhiteKing ? whiteKingPowersContainer : blackKingPowersContainer;
        GameObject iconObject = Instantiate(kingPowerIconPrefab, container);
        iconObject.name = $"{powerType}_PowerIcon";
        Sprite pieceSprite = ChessPieceManager.Instance.GetSpriteForPiece(powerType, isWhiteKing);
        if (iconObject.TryGetComponent(out Image iconImage) && pieceSprite != null)
        {
            iconImage.sprite = pieceSprite;
        }
    }

    private void HandlePowerLost(bool isWhiteKing, PieceType powerType)
    {
        Transform container = isWhiteKing ? whiteKingPowersContainer : blackKingPowersContainer;
        Transform iconToDestroy = container.Find($"{powerType}_PowerIcon");
        if (iconToDestroy != null)
        {
            Destroy(iconToDestroy.gameObject);
        }
    }

    private void HandleExtraLifeGained(bool isWhiteKing)
    {
        if (isWhiteKing) whiteKingLivesText.text = "Extra Lives: 1";
        else blackKingLivesText.text = "Extra Lives: 1";
    }

    public void ClearKingPowerUI()
    {
        foreach (Transform child in whiteKingPowersContainer) Destroy(child.gameObject);
        foreach (Transform child in blackKingPowersContainer) Destroy(child.gameObject);
        whiteKingLivesText.text = "Extra Lives: 0";
        blackKingLivesText.text = "Extra Lives: 0";
    }

    private void UpdateLastMoveDisplay(MoveData move)
    {
        bool isWhiteMove = (moveCount % 2 == 0);
        if (isWhiteMove)
        {
            moveNumberText.text = $"Move: {moveCount / 2 + 1}";
            whiteLastMoveText.text = move.Notation;
            blackLastMoveText.text = "...";
        }
        else
        {
            blackLastMoveText.text = move.Notation;
        }
        moveCount++;
    }

    public void ClearLastMoveDisplay()
    {
        moveNumberText.text = "Move: 1";
        whiteLastMoveText.text = "-";
        blackLastMoveText.text = "-";
        moveCount = 0;
    }

    public void ShowPanel() => gameObject.SetActive(true);
    public void HidePanel() => gameObject.SetActive(false);
}