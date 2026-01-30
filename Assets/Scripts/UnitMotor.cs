// FILE: Scripts/Characters/UnitMotor.cs
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMotor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        private Rigidbody _rb;
        private NavMeshAgent _agent;
        private bool _isPlayerControlled = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();

            // Default setup
            _rb.isKinematic = true;
            _agent.enabled = true;
        }

        public void SetPlayerControl(bool isPlayer)
        {
            _isPlayerControlled = isPlayer;

            if (_isPlayerControlled)
            {
                _agent.enabled = false;
                _rb.isKinematic = false;
            }
            else
            {
                _rb.isKinematic = true;
                _agent.enabled = true;
            }
        }

        public void MoveByInput(Vector3 direction)
        {
            if (!_isPlayerControlled) return;

            // Physics-based movement for Player
            Vector3 velocity = direction.normalized * _moveSpeed;
            velocity.y = _rb.linearVelocity.y; // Keep gravity
            _rb.linearVelocity = velocity;
        }

        public void RotateTowards(Vector3 point)
        {
            Vector3 direction = point - transform.position;
            direction.y = 0; // Keep rotation flat

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        public void MoveByNavMesh(Vector3 target)
        {
            if (_isPlayerControlled || !_agent.isActiveAndEnabled) return;
            _agent.SetDestination(target);
        }
    }
}