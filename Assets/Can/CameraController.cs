// MODULE A - Core & Player
// FILE: CameraController.cs
using UnityEngine;
using Can; // PossessionManager'a erişim için

namespace Game.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target References")]
        [Tooltip("Possession durumunda değilken (Ruh halindeyken) takip edilecek obje.")]
        [SerializeField] private Transform soulTransform;

        [Header("Camera Settings")]
        [SerializeField] private float smoothSpeed = 5f; // Takip yumuşaklığı
        [Tooltip("2D için Z değeri genellikle -10 olmalıdır.")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Boundaries (Optional)")]
        [SerializeField] private bool useBoundaries = false;
        [SerializeField] private float minX = -20f;
        [SerializeField] private float maxX = 20f;
        [SerializeField] private float minY = -20f;
        [SerializeField] private float maxY = 20f;

        private void LateUpdate()
        {
            Transform target = GetCurrentTarget();

            // Eğer takip edilecek hiçbir şey yoksa işlem yapma
            if (target == null) return;

            // Hedef pozisyonu hesapla (Target + Offset)
            Vector3 desiredPosition = target.position + offset;

            // Sınırlandırma (Boundaries) - Sadece X ve Y eksenleri (2D)
            if (useBoundaries)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            }

            // Kameranın Z pozisyonunu offset'e sadık kalarak koru (Titremeyi önler)
            // 2D'de target.position.z genellikle 0'dır ama biz offset.z'yi garantiye alalım.
            desiredPosition.z = offset.z;

            // Smooth Follow (Lerp)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
        }

        /// <summary>
        /// Mantık: 
        /// 1. Beden ele geçirilmişse (isPossessed) -> O bedenin Transform'u.
        /// 2. Beden yoksa (!isPossessed) -> Soul Transform.
        /// </summary>
        private Transform GetCurrentTarget()
        {
            // 1. PossessionManager kontrolü: Aktif bir beden var mı?
            if (PossessionManager.Instance != null &&
                PossessionManager.Instance.CurrentPossessed != null)
            {
                // Interface'i MonoBehaviour'a çevirip transformunu alıyoruz
                MonoBehaviour possessedBody = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;

                if (possessedBody != null)
                {
                    return possessedBody.transform;
                }
            }

            // 2. Hiçbir beden yoksa Ruh'u takip et
            return soulTransform;
        }

        /// <summary>
        /// Ruh objesi değişirse (örn: spawn olduğunda) dışarıdan atamak için.
        /// </summary>
        public void SetSoulTarget(Transform newSoulTarget)
        {
            soulTransform = newSoulTarget;
        }
    }
}