using System;
using DG.Tweening;
using UnityEngine;
using Can; // PossessionManager için
using Sirenix.OdinInspector; // GuardSystem'de vardý, butonlar için ekledim

namespace Mustafa
{
    public enum ScientistState
    {
        Patrolling,
        Fleeing,
        Alarm
    }

    public class ScientistSystem : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f;
        private int _currentPointIndex = 0;
        private Tween _moveTween;

        [Header("Flee Settings")]
        public float fleeSpeed = 4f;

        [Tooltip("Ruh formundaki oyuncuyu ne kadar uzaktan fark edip kaçacak? (Geniþ Menzil)")]
        public float soulDetectionRange = 8f;

        [Tooltip("Ele geçirilmiþ bir bedeni ne kadar uzaktan fark edecek? (Kýsa Menzil)")]
        public float possessedDetectionRange = 3f;

        [Header("Debug")]
        [ReadOnly] public ScientistState currentState = ScientistState.Patrolling;

        // Hedef takibi için dinamik property
        private Transform CurrentThreat
        {
            get
            {
                if (PossessionManager.Instance != null && PossessionManager.Instance.CurrentPossessed != null)
                {
                    var possessed = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;
                    // Eðer oyuncu aktifse ve sahnedeyse onun transformunu döndür
                    if (possessed != null && possessed.gameObject.activeInHierarchy)
                    {
                        return possessed.transform;
                    }
                }
                return null;
            }
        }

        private void Start()
        {
            StartPatrol();
        }

        private void Update()
        {
            // Eðer oyuncu yoksa veya oyun durduysa iþlem yapma
            if (CurrentThreat == null) return;

            CheckThreats();
        }

        private void CheckThreats()
        {
            Transform threat = CurrentThreat;
            float distance = Vector3.Distance(threat.position, transform.position);

            // Tehdit tipini belirle (Ruh mu yoksa Beden mi?)
            // PossessionManager.Instance.CurrentPossessed bir IPossessable olduðu için cast ediyoruz.
            // Not: SoulCharacter class'ýný kontrol etmek daha güvenlidir ama þimdilik isimlendirme veya type üzerinden gidiyoruz.
            bool isSoul = (threat.name.Contains("Soul") || threat.GetComponent<SoulCharacter>() != null);

            // Hangi menzili kullanacaðýz?
            float activeDetectionRange = isSoul ? soulDetectionRange : possessedDetectionRange;

            if (distance < activeDetectionRange)
            {
                // -- KAÇMA DURUMU (FLEE) --
                if (currentState != ScientistState.Fleeing)
                {
                    StopPatrol();
                    EnterAlarmState();
                }

                FleeFrom(threat.position);
            }
            else
            {
                // -- DEVRÝYE DURUMU (RETURN TO PATROL) --
                if (currentState == ScientistState.Fleeing || currentState == ScientistState.Alarm)
                {
                    // Tehdit uzaklaþtý, devriyeye dön
                    currentState = ScientistState.Patrolling;
                    StartPatrol();
                }
            }
        }

        #region Movement Logic

        [Button]
        public void StartPatrol()
        {
            if (movePoints == null || movePoints.Length == 0) return;

            currentState = ScientistState.Patrolling;
            GoToNextPoint();
        }

        private void GoToNextPoint()
        {
            // Önceki tween'i temizle
            _moveTween?.Kill();

            // Sýradaki nokta
            Transform targetPoint = movePoints[_currentPointIndex % movePoints.Length];

            // DOTween ile hareket
            _moveTween = transform.DOMove(targetPoint.position, patrolSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _currentPointIndex++;
                    GoToNextPoint(); // Bir sonraki noktaya git
                });
        }

        [Button]
        public void StopPatrol()
        {
            _moveTween?.Kill();
        }

        private void FleeFrom(Vector3 threatPosition)
        {
            currentState = ScientistState.Fleeing;

            // Tehditten zýt yöne doðru vektör hesapla
            Vector3 directionToThreat = transform.position - threatPosition;
            Vector3 fleeDirection = directionToThreat.normalized;

            // Karakteri tehdidin tersine hareket ettir
            // DOTween yerine anlýk hareket kullanýyoruz ki tepkisel olsun
            transform.position += fleeDirection * fleeSpeed * Time.deltaTime;

            // Karakter kaçtýðý yöne baksýn (Görsel çevirme)
            // 2D projesinde Scale.x ile oynayarak flip yapýyorduk, burada da basitçe:
            if (fleeDirection.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else transform.localScale = new Vector3(-1, 1, 1);
        }

        #endregion

        #region State Logic

        private void EnterAlarmState()
        {
            // Burasý "Ünlem Ýþareti" çýkartmak veya ses çalmak için ideal yer
            if (currentState != ScientistState.Alarm)
            {
                Debug.Log($"<color=red>ALARM!</color> Scientist tehdit algýladý! ({CurrentThreat.name})");
                // TODO: Play Alarm Sound
                // TODO: Show '!' UI above head
            }
        }

        #endregion

        // Editörde menzilleri görmek için
        private void OnDrawGizmosSelected()
        {
            // Ruh Algýlama Alaný (Kýrmýzý)
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, soulDetectionRange);

            // Beden Algýlama Alaný (Sarý)
            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, possessedDetectionRange);
        }
    }
}