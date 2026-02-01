using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Can; // PossessionManager i�in
using Eren; // CombatCharacter i�in

namespace TestTunax
{
    [RequireComponent(typeof(CombatCharacter))]
    public class SoldierSystem : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f;

        [Header("Combat AI Settings")]
        public float chaseSpeed = 3.5f; // Guard'dan daha h�zl�
        public float detectionRange = 8f; // Oyuncuyu fark etme mesafesi
        public float attackRange = 5f;    // Ate� etme mesafesi

        private int currentIndex = 0;
        private Tween _moveTween;
        private CombatCharacter _combatChar;

        // State kontrol�
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
                    // Kendimizi hedef almayal�m ve aktif bir hedef varsa d�nelim
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
            // E�er karakter possess edildiyse bu script Soldier.cs taraf�ndan kapat�l�r (enabled = false).
            // O y�zden burada ekstra kontrole gerek yok.
            CheckTargetAndAct();
        }

        private void CheckTargetAndAct()
        {
            Transform target = CurrentTarget;

            // Hedef yoksa normal devriyeye d�n
            if (target == null)
            {
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart(); // Devriyeye geri d�n
                }
                return;
            }

            float distance = Vector2.Distance(target.position, transform.position);

            // -- MANTIK: G�R -> KOVALA -> SALDIR --

            if (distance < detectionRange)
            {
                // Hedef menzilde, devriyeyi durdur
                if (!_isChasing)
                {
                    _isChasing = true;
                    StopMove(); // Tween'i �ld�r
                }

                // Silah� hedefe �evir (CombatCharacter i�indeki geli�mi� LookAt �al���r)
                _combatChar.LookAt(target.position);

                if (distance <= attackRange)
                {
                    // SALDIRI MODU: Dur ve Ate� Et
                    // Hareketi durdur (Pozisyonu sabitle veya h�z� s�f�rla)
                    // DOTween kullanmad���m�z i�in transform'u ellemiyoruz, oldu�u yerde durur.
                    _combatChar.SetAIWalking(false); // Duruyoruz

                    // Silahı ateşle
                    _combatChar.Action(target.position);
                }
                else
                {
                    // KOVALAMA MODU: Hedefe doğru koş
                    // GuardSystem'deki gibi MoveTowards kullanıyoruz ama biraz daha hızlı
                    _combatChar.SetAIWalking(true); // Koşuyoruz
                    Vector2 newPos = Vector2.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
                    transform.position = newPos;
                }
            }
            else
            {
                // Hedef menzilden ��kt�, devriyeye geri d�n
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart();
                }
            }
        }

        #region Patrol Logic (GuardSystem ile Ayn�)

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

            // Devriye s�ras�nda da bakt��� y�ne d�ns�n
            _combatChar.LookAt(targetPoint.position); _combatChar.SetAIWalking(true); // Devriye başladı, yürüyoruz
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
            _combatChar.SetAIWalking(false); // Devriye durdu
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