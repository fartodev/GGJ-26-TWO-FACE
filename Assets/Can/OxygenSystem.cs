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
        [SerializeField] private float soulFormDrainRate = 200f;

        [Tooltip("Ele geçirilmiş bedende oksijen kaybı hızı (birim/saniye)")]
        [SerializeField] private float possessedDrainRate = 4f;

        [Header("Bonuses")]
        [Tooltip("Bir bedene girince eklenen bonus oksijen")]
        [SerializeField] private float oxygenGainOnPossess = 30f;

        [Tooltip("Bedenden çıkınca geri kazanılan oksijen miktarı")]
        [SerializeField] private float oxygenOnDepossess = 20f;

        [Header("UI Reference")]
        [SerializeField] private Image oxygenBarFill;

        private bool _isSoulMode = true;        
        private Can.PossessionManager _possessionManager;

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

            _possessionManager = Can.PossessionManager.Instance;
        }

        private void OnEnable()
        {
            if (_possessionManager != null)
            {
                _possessionManager.OnPossessed += HandlePossession;
                _possessionManager.OnDepossessed += HandleDepossession;
            }
        }

        private void OnDisable()
        {
            if (_possessionManager != null)
            {
                _possessionManager.OnPossessed -= HandlePossession;
                _possessionManager.OnDepossessed -= HandleDepossession;
            }
        }

        private void Start()
        {
            currentOxygen = maxOxygen;
            UpdateUI();
        }

        private void FixedUpdate()
        {
            float drainRate = _isSoulMode ? soulFormDrainRate : possessedDrainRate;
            DrainOxygen(drainRate * Time.fixedDeltaTime);
        }

        private void HandlePossession(IPossessable target)
        {
            _isSoulMode = false;
            AddOxygen(oxygenGainOnPossess);

            Debug.Log($"<color=cyan>[OxygenSystem]</color> Bedene girildi. Bonus +{oxygenGainOnPossess} oksijen eklendi.");
        }

        private void HandleDepossession()
        {
            _isSoulMode = true;
            AddOxygen(oxygenOnDepossess);

            Debug.Log($"<color=cyan>[OxygenSystem]</color> Ruh formuna dönüldü. +{oxygenOnDepossess} oksijen eklendi.");
        }

        private void DrainOxygen(float amount)
        {
            currentOxygen -= amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

            UpdateUI();

            if (currentOxygen <= 0f)
            {
                HandleOxygenDepletion();
            }
        }

        public void AddOxygen(float amount)
        {
            currentOxygen += amount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (oxygenBarFill != null)
            {
                oxygenBarFill.fillAmount = currentOxygen / maxOxygen;
            }
        }

        private void HandleOxygenDepletion()
        {
            if (!_isSoulMode && _possessionManager != null)
            {
                Debug.LogWarning("<color=yellow>[OxygenSystem]</color> Bedendeyken oksijen bitti! Ruh formuna dönülüyor...");
                AddOxygen(oxygenOnDepossess);
                _possessionManager.Depossess();
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            Debug.LogError("<color=red>[OxygenSystem]</color> OKSİJEN BİTTİ! Oyun bitti.");

            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ShowGameOver("OXYGEN DEPLETED\nYOU DIED");
            }
            else
            {
                Debug.LogError("[OxygenSystem] GameOverUI instance bulunamadı! Sahneye GameOverUI ekleyin.");
            }

            Time.timeScale = 0f;
        }

        public float CurrentOxygen => currentOxygen;
        public float MaxOxygen => maxOxygen;
        public float OxygenPercentage => currentOxygen / maxOxygen;
        public bool IsSoulMode => _isSoulMode;
    }
}
