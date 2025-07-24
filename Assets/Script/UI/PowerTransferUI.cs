using UnityEngine;
using TMPro;

public class PowerTransferUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;

    public void Show()
    {
        instructionText.text = "Choose Piece";
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}