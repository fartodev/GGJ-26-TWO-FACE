using UnityEngine;
using System.Collections.Generic;
using Game.Characters; // UnitMotor ve BaseCharacter için

namespace Eren
{
    public enum AIState { Idle, Patrolling, Chasing, Attacking }

    [RequireComponent(typeof(CombatCharacter))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI State")]
        public AIState currentState = AIState.Patrolling;

        [Header("Detection Settings")]
        public float detectionRange = 7f;
        public float attackRange = 5f;
        public LayerMask targetLayer; // Player layer'ýný seçmelisin

        [Header("Patrol Settings")]
        public List<Transform> patrolWaypoints;
        public float waypointStopDistance = 0.5f;
        private int _currentWaypointIndex = 0;

        private CombatCharacter _combatChar;
        private Transform _playerTransform;

        private void Awake()
        {
            _combatChar = GetComponent<CombatCharacter>();
        }

        private void Start()
        {
            // Basitlik adýna sahnedeki Player'ý buluyoruz. 
            // PossessionManager.Instance.CurrentPossessed üzerinden de çekilebilir.
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _playerTransform = playerObj.transform;
        }

        private void Update()
        {
            // Eðer oyuncu bu karakteri ele geçirdiyse AI hesaplamasý yapma
            // BaseCharacter içindeki _isPossessed deðiþkeni korumalý (protected) olmalý 
            // veya bir public property üzerinden kontrol edilmeli.
            // Þimdilik CombatCharacter üzerinden dolaylý kontrol:
            if (IsBeingControlled()) return;

            HandleAIStateLogic();
        }

        private void HandleAIStateLogic()
        {
            float distanceToPlayer = _playerTransform != null
                ? Vector2.Distance(transform.position, _playerTransform.position)
                : float.MaxValue;

            switch (currentState)
            {
                case AIState.Patrolling:
                    PatrolLogic();
                    if (distanceToPlayer <= detectionRange) currentState = AIState.Chasing;
                    break;

                case AIState.Chasing:
                    MoveTowards(_playerTransform.position);
                    if (distanceToPlayer <= attackRange) currentState = AIState.Attacking;
                    if (distanceToPlayer > detectionRange) currentState = AIState.Patrolling;
                    break;

                case AIState.Attacking:
                    AttackLogic(distanceToPlayer);
                    break;
            }
        }

        private void PatrolLogic()
        {
            if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

            Vector2 targetPos = patrolWaypoints[_currentWaypointIndex].position;
            MoveTowards(targetPos);

            if (Vector2.Distance(transform.position, targetPos) < waypointStopDistance)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % patrolWaypoints.Count;
            }
        }

        private void AttackLogic(float distance)
        {
            // Hedefe bak (2D LookAt)
            Vector2 direction = (Vector2)_playerTransform.position - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Hareket etmeyi býrak ve ateþ et
            _combatChar.Move(Vector2.zero);
            _combatChar.Action(); // CombatCharacter.Action() -> WeaponSystem.Shoot() tetikler

            if (distance > attackRange) currentState = AIState.Chasing;
        }

        private void MoveTowards(Vector2 targetPos)
        {
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            _combatChar.Move(direction);

            // Hareket yönüne bakýþ (Opsiyonel)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private bool IsBeingControlled()
        {
            // PossessionManager üzerinden kontrol
            return Can.PossessionManager.Instance.CurrentPossessed == (Game.Core.IPossessable)_combatChar;
        }
    }
}