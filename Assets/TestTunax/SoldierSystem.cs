using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Can; // PossessionManager için
using Eren; // CombatCharacter için

namespace TestTunax
{
    [RequireComponent(typeof(CombatCharacter))]
    public class SoldierSystem : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f;

        [Header("Combat AI Settings")]
        public float chaseSpeed = 3.5f; // Guard'dan daha hýzlý
        public float detectionRange = 8f; // Oyuncuyu fark etme mesafesi
        public float attackRange = 5f;    // Ateþ etme mesafesi

        private int currentIndex = 0;
        private Tween _moveTween;
        private CombatCharacter _combatChar;

        // State kontrolü
        private bool _isChasing = false;

        /// <summary>
        /// Hedefi bulucu (PossessionManager veya Fallback)
        /// </summary>
        private Transform CurrentTarget
        {
            get
            {
                if (PossessionManager.Instance != null && PossessionManager.Instance.CurrentPossessed != null)
                {
                    var possessed = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;
                    // Kendimizi hedef almayalým ve aktif bir hedef varsa dönelim
                    if (possessed != null && possessed.gameObject != this.gameObject && possessed.gameObject.activeInHierarchy)
                    {
                        return possessed.transform;
                    }
                }
                return null;
            }
        }

        private void Awake()
        {
            _combatChar = GetComponent<CombatCharacter>();
        }

        private void Start()
        {
            MoveStart();
        }

        private void Update()
        {
            // Eðer karakter possess edildiyse bu script Soldier.cs tarafýndan kapatýlýr (enabled = false).
            // O yüzden burada ekstra kontrole gerek yok.
            CheckTargetAndAct();
        }

        private void CheckTargetAndAct()
        {
            Transform target = CurrentTarget;

            // Hedef yoksa normal devriyeye dön
            if (target == null)
            {
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart(); // Devriyeye geri dön
                }
                return;
            }

            float distance = Vector2.Distance(target.position, transform.position);

            // -- MANTIK: GÖR -> KOVALA -> SALDIR --

            if (distance < detectionRange)
            {
                // Hedef menzilde, devriyeyi durdur
                if (!_isChasing)
                {
                    _isChasing = true;
                    StopMove(); // Tween'i öldür
                }

                // Silahý hedefe çevir (CombatCharacter içindeki geliþmiþ LookAt çalýþýr)
                _combatChar.LookAt(target.position);

                if (distance <= attackRange)
                {
                    // SALDIRI MODU: Dur ve Ateþ Et
                    // Hareketi durdur (Pozisyonu sabitle veya hýzý sýfýrla)
                    // DOTween kullanmadýðýmýz için transform'u ellemiyoruz, olduðu yerde durur.

                    // Silahý ateþle
                    _combatChar.Action(target.position);
                }
                else
                {
                    // KOVALAMA MODU: Hedefe doðru koþ
                    // GuardSystem'deki gibi MoveTowards kullanýyoruz ama biraz daha hýzlý
                    transform.position = Vector2.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
                }
            }
            else
            {
                // Hedef menzilden çýktý, devriyeye geri dön
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart();
                }
            }
        }

        #region Patrol Logic (GuardSystem ile Ayný)

        [Button]
        public void MoveStart()
        {
            if (movePoints == null || movePoints.Length == 0) return;
            CheckMovePoint();
        }

        private void CheckMovePoint()
        {
            _moveTween?.Kill();

            Transform targetPoint = movePoints[currentIndex % movePoints.Length];

            // Devriye sýrasýnda da baktýðý yöne dönsün
            _combatChar.LookAt(targetPoint.position);

            _moveTween = transform.DOMove(targetPoint.position, patrolSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    currentIndex++;
                    CheckMovePoint();
                });
        }

        [Button]
        public void StopMove()
        {
            _moveTween?.Kill();
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}