using UnityEngine;
using Game.Core;
using Mustafa; // SoulCharacter'i tanımak için

namespace Can
{
    public class PossessionManager : MonoBehaviour
    {
        public static PossessionManager Instance { get; private set; }

        [Header("Soul Reference")]
        [SerializeField] private SoulCharacter playerSoul; // Sahnedeki Ruh objesini buraya sürükle

        [Header("Debug Info")]
        [SerializeField] private GameObject currentPossessedObject;
        public IPossessable CurrentPossessed { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Oyun başladığında otomatik olarak Ruh formuna geç
            if (playerSoul != null)
            {
                Possess(playerSoul);
            }
        }

        public void Possess(IPossessable target)
        {
            if (target == null) return;

            // 1. Eğer şu an bir bedendeysek, ondan çıkış işlemlerini yap
            if (CurrentPossessed != null)
            {
                CurrentPossessed.OnDepossess();
            }

            // 2. Eğer hedef Ruh değilse (yani bir düşmana giriyorsak), Ruhu gizle
            if (target != playerSoul)
            {
                playerSoul.gameObject.SetActive(false);
            }

            // 3. Yeni hedefi ayarla
            CurrentPossessed = target;
            currentPossessedObject = (target as MonoBehaviour)?.gameObject;

            // 4. Hedefe ele geçirildiğini bildir
            CurrentPossessed.OnPossess();

            Debug.Log($"<color=green>POSSESSED:</color> {currentPossessedObject.name}");
        }

        /// <summary>
        /// Bedenden çıkıp Ruh formuna döner.
        /// </summary>
        public void Depossess()
        {
            // 1. Ayrılacağımız bedeni geçici bir değişkende tutalım
            IPossessable oldHost = CurrentPossessed;

            // 2. Eğer şu anki karakter zaten Ruh ise işlem yapma (Ruh kendini öldürmesin)
            // 'playerSoul' değişkeninin sınıfın başında tanımlı olduğunu varsayıyorum
            if (oldHost == playerSoul) return;

            // 3. Bedenden çıkış işlemlerini yap (Resetleme vs.)
            if (oldHost != null)
            {
                oldHost.OnDepossess();
            }

            // 4. Ruhu, eski bedenin konumuna getir ve aktif et
            if (currentPossessedObject != null)
            {
                playerSoul.transform.position = currentPossessedObject.transform.position;
            }

            playerSoul.gameObject.SetActive(true);

            // 5. Yönetimi Ruha geçir
            CurrentPossessed = playerSoul;
            currentPossessedObject = playerSoul.gameObject;
            playerSoul.OnPossess();

            Debug.Log("Ruh serbest kaldı, eski beden yok ediliyor...");

            // 6. KRİTİK ADIM: Eski bedeni öldür! ☠️
            // oldHost bir MonoBehaviour olduğu için GetComponent diyebiliriz
            if (oldHost is MonoBehaviour hostMono)
            {
                // Eğer o bedende Can Sistemi varsa öldür
                if (hostMono.TryGetComponent(out Mustafa.HealthSystem health))
                {
                    health.Kill();
                }
                else
                {
                    // Can sistemi yoksa direkt yok et (Fallback)
                    Destroy(hostMono.gameObject);
                }
            }
        }
    }
}