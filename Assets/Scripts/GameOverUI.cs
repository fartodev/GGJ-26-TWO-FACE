using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Core
{
    /// <summary>
    /// GameOver ekranını yöneten UI Controller
    /// Canvas > GameOverPanel şeklinde organize edilmeli
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject gameOverPanel; // Ana panel
        [SerializeField] private TextMeshProUGUI gameOverText; // "GAME OVER" yazısı (Opsiyonel)

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Settings")]
        [SerializeField] private string gameOverMessage = "OXYGEN DEPLETED";

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Başlangıçta paneli gizle
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            // Button event'leri bağla
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            else
            {
                Debug.LogWarning("[GameOverUI] Restart Button referansı eksik!");
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }
            else
            {
                Debug.LogWarning("[GameOverUI] Main Menu Button referansı eksik!");
            }
        }

        /// <summary>
        /// Game Over ekranını gösterir
        /// </summary>
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Mesaj varsa güncelle
            if (gameOverText != null)
            {
                gameOverText.text = gameOverMessage;
            }

            // Cursor'u göster ve serbest bırak (UI için)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("<color=red>[GameOverUI]</color> Game Over ekranı gösterildi.");
        }

        /// <summary>
        /// Özel mesajla Game Over göster
        /// </summary>
        public void ShowGameOver(string customMessage)
        {
            gameOverMessage = customMessage;
            ShowGameOver();
        }

        // --- Button Callbacks ---

        private void OnRestartClicked()
        {
            Debug.Log("Restart butonuna tıklandı.");
            SceneLoader.RestartCurrentScene();
        }

        private void OnMainMenuClicked()
        {
            Debug.Log("Main Menu butonuna tıklandı.");
            SceneLoader.LoadMainMenu();
        }

        // Test için (Editor'de)
        [ContextMenu("Test Game Over")]
        private void TestGameOver()
        {
            ShowGameOver("TEST GAME OVER");
        }
    }
}
