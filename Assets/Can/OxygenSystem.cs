using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{
    public class OxygenSystem : MonoBehaviour
    {
        public static OxygenSystem Instance { get; private set; }

        [Header("Oxygen Settings")]
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float currentOxygen = 100f;

        [Header("Drain Rates")]
        [Tooltip("Ruh formunda oksijen kaybı hızı (birim/saniye)")]
        [SerializeField] private float soulFormDrainRate = 10f;

        [Tooltip("Ele geçirilmiş bedende oksijen kaybı hızı (birim/saniye)")]
        [SerializeField] private float possessedDrainRate = 2f;

        [Header("Bonuses")]
        [Tooltip("Bir bedene girince eklenen bonus oksijen")]
        [SerializeField] private float oxygenGainOnPossess = 30f;

        [Header("UI Reference")]
        [SerializeField] private Image oxygenBarFill;

        // Durum kontrolü - Eventler tarafından güncellenir
        private bool _isSoulMode = true;

        private void Awake()
        {
            // Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // Event Aboneliği - PossessionManager'dan gelen olayları dinle
            if (Can.PossessionManager.Instance != null)
            {
                Can.PossessionManager.Instance.OnPossessed += HandlePossession;
                Can.PossessionManager.Instance.OnDepossessed += HandleDepossession;
            }
        }

        private void OnDisable()
        {
            // Memory Leak önlemek için abonelikten çık
            if (Can.PossessionManager.Instance != null)
            {
                Can.PossessionManager.Instance.OnPossessed -= HandlePossession;
                Can.PossessionManager.Instance.OnDepossessed -= HandleDepossession;
            }
        }

        private void Start()
        {
            // Başlangıçta oksijeni max yap
            currentOxygen = maxOxygen;
            UpdateUI();
        }

        private void Update()
        {
            // Moda göre drain rate belirle
            float drainRate = _isSoulMode ? soulFormDrainRate : possessedDrainRate;

            // Oksijeni azalt
            DrainOxygen(drainRate * Time.deltaTime);
        }

        /// <summary>
        /// Possession olayı gerçekleştiğinde çağrılır (Event Handler)
        /// </summary>
        private void HandlePossession(IPossessable target)
        {
            _isSoulMode = false; // Artık bedendeyiz
            AddOxygen(oxygenGainOnPossess); // Bonus oksijen ekle

            Debug.Log($"<color=cyan>[OxygenSystem]</color> Bedene girildi. Bonus +{oxygenGainOnPossess} oksijen eklendi.");
        }

        /// <summary>
        /// Depossession olayı gerçekleştiğinde çağrılır (Event Handler)
        /// </summary>
        private void HandleDepossession()
        {
            _isSoulMode = true; // Tekrar ruh formundayız

            Debug.Log("<color=cyan>[OxygenSystem]</color> Ruh formuna dönüldü. Hızlı oksijen kaybı aktif.");
        }

        /// <summary>
        /// Oksijen tüketimi
        /// </summary>
        private void DrainOxygen(float amount)
        {
            currentOxygen -= amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

            UpdateUI();

            // Oksijen bittiyse oyunu bitir
            if (currentOxygen <= 0f)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Oksijen ekleme (Possession bonus vs.)
        /// </summary>
        public void AddOxygen(float amount)
        {
            currentOxygen += amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

            UpdateUI();
        }

        /// <summary>
        /// UI Bar güncelleme
        /// </summary>
        private void UpdateUI()
        {
            if (oxygenBarFill != null)
            {
                oxygenBarFill.fillAmount = currentOxygen / maxOxygen;
            }
        }

        /// <summary>
        /// Oyun sonu - Oksijen bittiğinde
        /// </summary>
        private void GameOver()
        {
            Debug.LogError("<color=red>[OxygenSystem]</color> OKSİJEN BİTTİ! Oyun bitti.");

            // TODO: Oyun sonu ekranı veya sahne yenileme
            Time.timeScale = 0f; // Geçici olarak oyunu durdur
        }

        // Public getter'lar - Gerekirse dışarıdan erişim için
        public float CurrentOxygen => currentOxygen;
        public float MaxOxygen => maxOxygen;
        public float OxygenPercentage => currentOxygen / maxOxygen;
        public bool IsSoulMode => _isSoulMode;
    }
}
