using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button queenButton;
    [SerializeField] private Button rookButton;
    [SerializeField] private Button bishopButton;
    [SerializeField] private Button knightButton;

    [Header("White Piece Sprites")]
    [SerializeField] private Sprite whiteQueenSprite;
    [SerializeField] private Sprite whiteRookSprite;
    [SerializeField] private Sprite whiteBishopSprite;
    [SerializeField] private Sprite whiteKnightSprite;

    [Header("Black Piece Sprites")]
    [SerializeField] private Sprite blackQueenSprite;
    [SerializeField] private Sprite blackRookSprite;
    [SerializeField] private Sprite blackBishopSprite;
    [SerializeField] private Sprite blackKnightSprite;

    private void Awake()
    {
        queenButton.onClick.AddListener(() => OnPromotionChoice(PieceType.Queen));
        rookButton.onClick.AddListener(() => OnPromotionChoice(PieceType.Rook));
        bishopButton.onClick.AddListener(() => OnPromotionChoice(PieceType.Bishop));
        knightButton.onClick.AddListener(() => OnPromotionChoice(PieceType.Knight));
    }

    public void ShowPanel(bool isWhite)
    {
        if (isWhite)
        {
            queenButton.GetComponent<Image>().sprite = whiteQueenSprite;
            rookButton.GetComponent<Image>().sprite = whiteRookSprite;
            bishopButton.GetComponent<Image>().sprite = whiteBishopSprite;
            knightButton.GetComponent<Image>().sprite = whiteKnightSprite;
        }
        else
        {
            queenButton.GetComponent<Image>().sprite = blackQueenSprite;
            rookButton.GetComponent<Image>().sprite = blackRookSprite;
            bishopButton.GetComponent<Image>().sprite = blackBishopSprite;
            knightButton.GetComponent<Image>().sprite = blackKnightSprite;
        }

        gameObject.SetActive(true);
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    private void OnPromotionChoice(PieceType type)
    {
        GameManager.Instance.FinalizePawnPromotion(type);
    }
}