using DG.Tweening;
using UnityEngine;
using Can;
using Sirenix.OdinInspector;

namespace Mustafa
{
    public enum ScientistState
    {
        Patrolling,
        Fleeing,
        Searching
    }

    [RequireComponent(typeof(Rigidbody2D))] // Rigidbody şart
    public class ScientistSystem : MonoBehaviour
    {
        [Header("Movement Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f;
        public float fleeSpeed = 6f; // Kaçış hızı
        private int _currentPointIndex = 0;
        private Tween _moveTween;
        private Rigidbody2D _rb;

        [Header("Detection Ranges (Trigger)")]
        [Tooltip("Ruhu fark etme mesafesi")]
        public float soulDetectionRange = 8f;
        [Tooltip("İnsan bedenini fark etme mesafesi")]
        public float possessedDetectionRange = 3f;

        [Header("Safety Ranges (Exit)")]
        [Tooltip("Ruh'tan kaçarken durma mesafesi")]
        public float soulSafeDistance = 15f;
        [Tooltip("İnsan bedeninden kaçarken durma mesafesi")]
        public float possessedSafeDistance = 8f;

        [Header("Cooldown")]
        public float searchDuration = 2f;
        private float _searchTimer;

        [Header("Debug")]
        [ReadOnly] public ScientistState currentState = ScientistState.Patrolling;
        [ReadOnly] public Transform currentThreat;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            StartPatrol();
        }

        private void Update()
        {
            // 1. Tehdit Analizi
            currentThreat = GetCurrentThreat();
            if (currentThreat == null) return;

            float distance = Vector2.Distance(transform.position, currentThreat.position);

            // Tehdit tipine göre menzil seçimi
            bool isSoul = currentThreat.GetComponent<SoulCharacter>() != null || currentThreat.name.Contains("Soul");
            float activeDetectionRange = isSoul ? soulDetectionRange : possessedDetectionRange;
            float activeSafeDistance = isSoul ? soulSafeDistance : possessedSafeDistance;

            // =================================================================================
            // 🛑 GLOBAL DANGER OVERRIDE (FİZİK TABANLI)
            // =================================================================================
            if (distance < activeDetectionRange)
            {
                if (currentState != ScientistState.Fleeing)
                {
                    StartFleeing();
                }

                // Kaçış mantığı her frame çalışmalı (Velocity uyguluyoruz)
                FleeBehavior(currentThreat.position);
                return;
            }

            // 2. Durum Makinesi
            switch (currentState)
            {
                case ScientistState.Fleeing:
                    // Güvenli mesafeye ulaştık mı?
                    if (distance < activeSafeDistance)
                    {
                        // Hala kaçmamız lazım
                        FleeBehavior(currentThreat.position);
                    }
                    else
                    {
                        // Güvendeyiz, dur ve etrafı dinle
                        StartSearching();
                    }
                    break;

                case ScientistState.Searching:
                    _searchTimer -= Time.deltaTime;
                    // Beklerken olduğu yerde kalmasını sağla
                    _rb.linearVelocity = Vector2.zero;

                    if (_searchTimer <= 0)
                    {
                        StartPatrol();
                    }
                    break;

                case ScientistState.Patrolling:
                    // DOTween çalıştığı için burada fizik kullanmıyoruz
                    // Sadece güvenliği sağlamak için hızı sıfırlayalım ki kaymasın
                    _rb.linearVelocity = Vector2.zero;
                    break;
            }
        }

        #region Actions

        private void StartFleeing()
        {
            if (currentState == ScientistState.Fleeing) return;

            _moveTween?.Kill(); // Tween'i kesinlikle öldür
            currentState = ScientistState.Fleeing;

            Debug.Log($"<color=red>TEHLİKE!</color> Kaçış Başladı: {currentThreat.name}");
        }

        private void FleeBehavior(Vector3 threatPos)
        {
            // 1. Yön Hesapla
            Vector2 direction = ((Vector2)transform.position - (Vector2)threatPos).normalized;

            // 2. FİZİK KULLAN (Transform yerine Rigidbody Velocity)
            // Bu sayede duvara çarpınca durur ama sıkışmaz, colliderlar iç içe geçmez.
            _rb.linearVelocity = direction * fleeSpeed;

            // 3. Görsel Flip
            if (direction.x > 0.1f) transform.localScale = Vector3.one;
            else if (direction.x < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
        }

        private void StartSearching()
        {
            currentState = ScientistState.Searching;
            _searchTimer = searchDuration;

            // Fiziği durdur
            _rb.linearVelocity = Vector2.zero;

            Debug.Log("<color=yellow>GÜVENLİ.</color> Etrafı dinliyor...");
        }

        public void StartPatrol()
        {
            // Zaten devriyedeysek elleme
            if (currentState == ScientistState.Patrolling && _moveTween != null && _moveTween.IsActive()) return;

            currentState = ScientistState.Patrolling;
            // Fizik hızını sıfırla ki Tween ile çakışmasın
            _rb.linearVelocity = Vector2.zero;

            GoToNextPoint();
        }

        private void GoToNextPoint()
        {
            _moveTween?.Kill();
            if (movePoints == null || movePoints.Length == 0) return;

            Transform target = movePoints[_currentPointIndex % movePoints.Length];

            // Flip mantığı
            Vector3 dir = target.position - transform.position;
            if (dir.x > 0) transform.localScale = Vector3.one;
            else transform.localScale = new Vector3(-1, 1, 1);

            _moveTween = transform.DOMove(target.position, patrolSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _currentPointIndex++;
                    GoToNextPoint();
                });
        }

        #endregion

        // Singleton check helper
        private Transform GetCurrentThreat()
        {
            if (PossessionManager.Instance != null && PossessionManager.Instance.CurrentPossessed != null)
            {
                var possessed = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;
                if (possessed != null && possessed.gameObject.activeInHierarchy)
                    return possessed.transform;
            }
            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, soulDetectionRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, soulSafeDistance);
        }
    }
}