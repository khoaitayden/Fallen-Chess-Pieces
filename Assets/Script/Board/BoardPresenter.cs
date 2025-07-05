using UnityEngine;

public class BoardPresenter : MonoBehaviour
{
    public static BoardPresenter Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    private float rotationSpeed = 5f;

    private Quaternion targetBoardRotation;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;

    private readonly Quaternion whiteViewBoardRotation = Quaternion.Euler(0, 0, 0);
    private readonly Quaternion blackViewBoardRotation = Quaternion.Euler(0, 0, 180);

    private void Awake()
    {
        Instance = this;
        targetBoardRotation = whiteViewBoardRotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetBoardRotation, Time.deltaTime * rotationSpeed);
    }

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