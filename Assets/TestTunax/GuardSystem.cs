using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Can; // PossessionManager için
using Eren; // CombatCharacter için

namespace TestTunax
{
    [RequireComponent(typeof(CombatCharacter))]
    public class GuardSystem : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] movePoints;
        public float patrolSpeed = 2f; // Devriye hýzý

        [Header("Combat AI Settings")]
        public float chaseSpeed = 2.5f;   // Soldier'dan (3.5) daha yavaþ
        public float detectionRange = 6f; // Soldier'dan (8) daha kör
        public float attackRange = 4f;    // Soldier'dan (5) daha kýsa menzilli

        private int currentIndex = 0;
        private Tween _moveTween;
        private CombatCharacter _combatChar;

        // State kontrolü
        private bool _isChasing = false;

        /// <summary>
        /// Hedef bulucu (PossessionManager öncelikli, yoksa null)
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
            // Eðer karakter possess edildiyse Guard.cs tarafýndan enabled=false yapýlýr.
            CheckTargetAndAct();
        }

        private void CheckTargetAndAct()
        {
            Transform target = CurrentTarget;

            // Hedef yoksa veya kaybolduysa devriyeye dön
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

            // -- MANTIK: GÖR -> KOVALA -> SALDIR --

            if (distance < detectionRange)
            {
                // Hedef menzilde, devriyeyi durdur ve kovalama moduna geç
                if (!_isChasing)
                {
                    _isChasing = true;
                    StopMove(); // Tween'i öldür
                }

                // 1. Silahý hedefe çevir
                _combatChar.LookAt(target.position);

                if (distance <= attackRange)
                {
                    // 2. SALDIRI MODU: Dur ve Ateþ Et
                    // DOTween kullanmadýðýmýz için transform'u ellemiyoruz, olduðu yerde durur.

                    // Hedefin pozisyonunu gönderiyoruz ki mermi oraya gitsin
                    _combatChar.Action(target.position);
                }
                else
                {
                    // 3. KOVALAMA MODU: Hedefe doðru yürü
                    transform.position = Vector2.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
                }
            }
            else
            {
                // Hedef menzilden çýktý, takibi býrak ve devriyeye dön
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

            // Devriye sýrasýnda da gidilen noktaya baksýn
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
            // Görsel Debug
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange); // Fark etme alaný

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);    // Ateþ etme alaný
        }
    }
}