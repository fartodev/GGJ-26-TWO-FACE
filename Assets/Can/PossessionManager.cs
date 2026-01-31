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
            // Şu anki bedenden çık
            if (CurrentPossessed != null)
            {
                CurrentPossessed.OnDepossess();
                Debug.Log($"<color=red>DEPOSSESSED:</color> {(CurrentPossessed as MonoBehaviour)?.name}");
            }

            // Ruhu, son bulunduğumuz bedenin konumuna getir
            if (currentPossessedObject != null)
            {
                playerSoul.transform.position = currentPossessedObject.transform.position;
            }

            // Ruhu aktif et ve kontrolü ona ver
            playerSoul.gameObject.SetActive(true);

            // Doğrudan Possess metodunu çağırmıyoruz (döngüye girmesin diye), manuel set ediyoruz
            CurrentPossessed = playerSoul;
            currentPossessedObject = playerSoul.gameObject;
            playerSoul.OnPossess();

            Debug.Log("Ruh formuna dönüldü.");
        }
    }
}