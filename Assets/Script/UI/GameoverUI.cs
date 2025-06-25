// using UnityEngine;
// using UnityEngine.UI; // Required for UI elements like Text
// using TMPro; // Required for TextMeshPro elements

// public class GameoverUI : MonoBehaviour
// {
//     [Header("Game Over UI")]
//     [SerializeField] private TextMeshProUGUI winnerText;
//     [SerializeField] private Button rematchButton;
//     [SerializeField] private Button quitButton;
//     private void Awake()
//     {
//         rematchButton.onClick.AddListener(OnRematchClicked);
//         quitButton.onClick.AddListener(OnQuitClicked);
        
//     }
//     private void OnRematchClicked()
//     {
//         UnityEngine.SceneManagement.SceneManager.LoadScene(0);
//     }



//     private void OnQuitClicked()
//     {
//         Application.Quit();
//         #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
//         #endif
//     }
// }
