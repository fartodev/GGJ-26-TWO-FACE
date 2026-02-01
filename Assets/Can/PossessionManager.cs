using System;
using UnityEngine;
using Game.Core;
using Mustafa; // SoulCharacter'i tanımak için

namespace Can
{
    public class PossessionManager : MonoBehaviour
    {
        public static PossessionManager Instance { get; private set; }

        // EVENTS - Observer Pattern Implementation
        public event Action<IPossessable> OnPossessed;
        public event Action OnDepossessed;

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

            // NERF: Eğer zaten bir bedendeyken (Ruh değilken) başka birine geçmeye çalışırsak,
            // direkt possess etmek yerine depossess ol (Ruh formuna dön)
            if (CurrentPossessed != null && (object)CurrentPossessed != playerSoul && (object)target != playerSoul)
            {
                Debug.Log("<color=yellow>Zaten bir bedende bulunuyorsun! Önce çıkmalısın.</color>");
                Depossess();
                return;
            }

            // 1. ESKİ BEDEN İŞLEMLERİ
            if (CurrentPossessed != null)
            {
                // Önce standart çıkış işlemini yap (AI açma vs.)
                CurrentPossessed.OnDepossess();

                // --- YENİ EKLENEN KISIM: ÖLDÜRME MANTIĞI ---

                // Eğer şu an içinde olduğumuz şey RUH DEĞİLSE (yani bir bedenden başka bedene atlıyorsak)
                if ((object)CurrentPossessed != playerSoul)
                {
                    // Eski bedeni bir MonoBehaviour olarak al
                    MonoBehaviour oldHost = CurrentPossessed as MonoBehaviour;

                    if (oldHost != null)
                    {
                        // HealthSystem'i bul ve öldür
                        if (oldHost.TryGetComponent(out Mustafa.HealthSystem health))
                        {
                            health.Kill(); // Bu fonksiyon HealthSystem içinde Die() çağırır
                            Debug.Log($"<color=red>BEDEN TERK EDİLDİ VE ÖLDÜRÜLDÜ:</color> {oldHost.name}");
                        }
                        else
                        {
                            // Can sistemi yoksa direkt yok et
                            Destroy(oldHost.gameObject);
                        }
                    }
                }
                // Eğer Ruh'tan çıkıyorsak (Soul -> Guard), Ruh'u öldürme, sadece gizle
                else
                {
                    playerSoul.gameObject.SetActive(false);
                }
            }

            // 2. YENİ BEDEN İŞLEMLERİ (Burada değişiklik yok)

            // Eğer hedef Ruh değilse (yani bir düşmana giriyorsak), Ruhu gizle
            // (Yukarıda zaten yaptık ama güvenlik için kalsın)
            if ((object)target != playerSoul)
            {
                playerSoul.gameObject.SetActive(false);
            }

            // Yeni hedefi ayarla
            CurrentPossessed = target;
            currentPossessedObject = (target as MonoBehaviour)?.gameObject;

            // Hedefe ele geçirildiğini bildir
            CurrentPossessed.OnPossess();

            // Event fırlat
            OnPossessed?.Invoke(CurrentPossessed);

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
            if ((object)oldHost == playerSoul) return;

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

            // Event fırlat - Dinleyiciler tepki verecek
            OnDepossessed?.Invoke();

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