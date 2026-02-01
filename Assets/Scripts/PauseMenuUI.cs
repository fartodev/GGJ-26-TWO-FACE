using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{
    /// <summary>
    /// Pause Menu yönetimi (ESC tuşu ile açılır)
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private bool _isPaused = false;

        private void Awake()
        {
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
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            SetupButtons();
        }

        private void Update()
        {
            // ESC tuşu ile pause toggle
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void SetupButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("[PauseMenu] Oyun duraklatıldı.");
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            Cursor.visible = true; // 2D top-down oyunda cursor görünür kalabilir
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("[PauseMenu] Oyun devam ediyor.");
        }

        private void OnRestartClicked()
        {
            Resume(); // TimeScale'i düzelt
            SceneLoader.RestartCurrentScene();
        }

        private void OnMainMenuClicked()
        {
            Resume(); // TimeScale'i düzelt
            SceneLoader.LoadMainMenu();
        }
    }
}
