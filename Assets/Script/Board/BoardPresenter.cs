using UnityEngine;

public class BoardPresenter : MonoBehaviour
{
    public static BoardPresenter Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    private float rotationSpeed = 5f;

    private Quaternion targetBoardRotation;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;

    // Store the default "White's View" rotations
    private readonly Quaternion whiteViewBoardRotation = Quaternion.Euler(0, 0, 0);
    private readonly Quaternion blackViewBoardRotation = Quaternion.Euler(0, 0, 180);

    private void Awake()
    {
        Instance = this;
        // Set initial target to White's view
        targetBoardRotation = whiteViewBoardRotation;
    }

    private void Update()
    {
        // This now rotates the BoardPivot, which is at the center of the board.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetBoardRotation, Time.deltaTime * rotationSpeed);
    }

    // Orients the board to the perspective of the given player color.
    public void OrientBoardToPlayer(bool isWhitePlayer)
    {
        if (isWhitePlayer)
        {
            targetBoardRotation = whiteViewBoardRotation;
        }
        else
        {
            targetBoardRotation = blackViewBoardRotation;
        }
    }
}