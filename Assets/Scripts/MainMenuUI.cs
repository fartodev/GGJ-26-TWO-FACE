using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{
    /// <summary>
    /// Ana Menü UI Controller
    /// Main Menu sahnesinde kullanılır
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button settingsButton; // İleride eklenebilir

        private void Start()
        {
            // Cursor'u göster
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Buttonları bağla
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
        }

        private void OnPlayClicked()
        {
            Debug.Log("Play butonuna tıklandı.");
            SceneLoader.StartGame();
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit butonuna tıklandı.");
            SceneLoader.QuitGame();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings butonuna tıklandı (Henüz implement edilmedi).");
            // TODO: Settings panelini aç
        }
    }
}
