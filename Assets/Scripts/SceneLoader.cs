using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    /// <summary>
    /// Scene yönetimi için statik yardımcı sınıf
    /// </summary>
    public static class SceneLoader
    {
        // Sahne isimleri - Buradan yönetilir (Inspector'da yazım hatası riskini azaltır)
        public const string MAIN_MENU = "MainMenu";
        public const string GAME_SCENE = "GameScene"; // veya oyununuzun ana sahne adı

        /// <summary>
        /// Mevcut sahneyi yeniden yükler
        /// </summary>
        public static void RestartCurrentScene()
        {
            Time.timeScale = 1f; // TimeScale'i normale döndür
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        /// <summary>
        /// İsme göre sahne yükler
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            Time.timeScale = 1f; // TimeScale'i normale döndür
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Ana menüye döner
        /// </summary>
        public static void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(MAIN_MENU);
        }

        /// <summary>
        /// Oyunu başlatır (Ana oyun sahnesine gider)
        /// </summary>
        public static void StartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(GAME_SCENE);
        }

        /// <summary>
        /// Oyundan çıkar (Sadece build'de çalışır)
        /// </summary>
        public static void QuitGame()
        {
            Debug.Log("Oyundan çıkılıyor...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
