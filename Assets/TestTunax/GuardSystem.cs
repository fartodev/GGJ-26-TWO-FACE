using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Can; // PossessionManager i�in
using Eren; // CombatCharacter i�in

namespace TestTunax
{
    [RequireComponent(typeof(CombatCharacter))]
    public class GuardSystem : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f; // Devriye h�z�

        [Header("Combat AI Settings")]
        public float chaseSpeed = 2.5f;   // Soldier'dan (3.5) daha yava�
        public float detectionRange = 6f; // Soldier'dan (8) daha k�r
        public float attackRange = 4f;    // Soldier'dan (5) daha k�sa menzilli

        private int currentIndex = 0;
        private Tween _moveTween;
        private CombatCharacter _combatChar;

        // State kontrol�
        private bool _isChasing = false;

        /// <summary>
        /// Hedef bulucu (PossessionManager �ncelikli, yoksa null)
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
            // E�er karakter possess edildiyse Guard.cs taraf�ndan enabled=false yap�l�r.
            CheckTargetAndAct();
        }

        private void CheckTargetAndAct()
        {
            Transform target = CurrentTarget;

            // Hedef yoksa veya kaybolduysa devriyeye d�n
            if (target == null)
            {
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart();
                }
                return;
            }

            float distance = Vector2.Distance(target.position, transform.position);

            // -- MANTIK: G�R -> KOVALA -> SALDIR --

            if (distance < detectionRange)
            {
                // Hedef menzilde, devriyeyi durdur ve kovalama moduna ge�
                if (!_isChasing)
                {
                    _isChasing = true;
                    StopMove(); // Tween'i �ld�r
                }

                // 1. Silah� hedefe �evir
                _combatChar.LookAt(target.position);

                if (distance <= attackRange)
                {
                    // 2. SALDIRI MODU: Dur ve Ate� Et
                    // DOTween kullanmad���m�z i�in transform'u ellemiyoruz, oldu�u yerde durur.
                    _combatChar.SetAIWalking(false); // Duruyoruz

                    // Hedefin pozisyonunu gönderiyoruz ki mermi oraya gitsin
                    _combatChar.Action(target.position);
                }
                else
                {
                    // 3. KOVALAMA MODU: Hedefe doğru yürü
                    _combatChar.SetAIWalking(true); // Yürüyoruz
                    Vector2 newPos = Vector2.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
                    transform.position = newPos;
                }
            }
            else
            {
                // Hedef menzilden ��kt�, takibi b�rak ve devriyeye d�n
                if (_isChasing)
                {
                    _isChasing = false;
                    MoveStart();
                }
            }
        }

        #region Patrol Logic

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

            // Devriye s�ras�nda da gidilen noktaya baks�n
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
            // G�rsel Debug
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange); // Fark etme alan�

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);    // Ate� etme alan�
        }
    }
}