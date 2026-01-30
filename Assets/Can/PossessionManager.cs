using UnityEngine;
using Game.Core;
// Ortak scriptlere erişim için namespace gerekmiyorsa (global namespace) direkt kullanıyoruz.
// Eğer ortak scriptler 'Core' gibi bir namespace'de ise 'using Core;' eklenmeli.

namespace Can
{
    public class PossessionManager : MonoBehaviour
    {
        public static PossessionManager Instance { get; private set; }

        [Header("Debug Info")]
        [SerializeField] private GameObject currentPossessedObject; // Inspector'da görmek için

        // Şu an kontrol edilen varlık (Interface üzerinden tutuyoruz)
        public IPossessable CurrentPossessed { get; private set; }

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

        /// <summary>
        /// Hedef varlığın kontrolünü ele alır.
        /// </summary>
        /// <param name="target">Ele geçirilecek hedef</param>
        public void Possess(IPossessable target)
        {
            if (target == null) return;

            // 1. Eğer zaten birini kontrol ediyorsak, onu bırak (Depossess)
            if (CurrentPossessed != null)
            {
                CurrentPossessed.OnDepossess();
            }

            // 2. Yeni hedefi ayarla
            CurrentPossessed = target;

            // MonoBehaviour referansını da inspector için tutalım (Cast işlemi)
            currentPossessedObject = (target as MonoBehaviour)?.gameObject;

            // 3. Hedefe ele geçirildiğini bildir
            CurrentPossessed.OnPossess();

            Debug.Log($"<color=green>POSSESSED:</color> {currentPossessedObject.name}");

            // TODO: CameraController'a yeni hedefi bildir (Daha sonra eklenecek)
        }

        /// <summary>
        /// Mevcut bedeni terk et (Örn: Öldüğünde veya manuel çıkışta)
        /// </summary>
        public void Depossess()
        {
            if (CurrentPossessed != null)
            {
                CurrentPossessed.OnDepossess();
                Debug.Log($"<color=red>DEPOSSESSED:</color> {(CurrentPossessed as MonoBehaviour)?.name}");

                CurrentPossessed = null;
                currentPossessedObject = null;
            }
        }
    }
}