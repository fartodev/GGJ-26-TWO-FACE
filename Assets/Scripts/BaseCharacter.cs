// FILE: Scripts/Characters/BaseCharacter.cs
// Updated: LookAt and Move are now virtual
using UnityEngine;
using Game.Core;

namespace Game.Characters
{
    [RequireComponent(typeof(UnitMotor))]
    public abstract class BaseCharacter : MonoBehaviour, IPossessable
    {
        [SerializeField] private UnitMotor _motor;

        // Debugging / State tracking
        [SerializeField] private bool _isPossessed = false;

        protected UnitMotor Motor => _motor;

        protected virtual void Awake()
        {
            if (_motor == null) _motor = GetComponent<UnitMotor>();
        }

        // --- IPossessable Implementation ---

        public virtual void OnPossess()
        {
            _isPossessed = true;
            _motor.SetPlayerControl(true);
            // Kamera odaklanma mantığı Event sistemi ile Module A tarafından dinlenebilir
            Debug.Log($"{gameObject.name} Possessed!");
        }

        public virtual void OnDepossess()
        {
            _isPossessed = false;
            _motor.SetPlayerControl(false);
            // AI moduna dönüş
            Debug.Log($"{gameObject.name} Depossessed! Switching to AI.");
        }

        public virtual void Move(Vector3 direction)
        {
            _motor.MoveByInput(direction);
        }

        public virtual void LookAt(Vector3 targetPoint)
        {
            _motor.RotateTowards(targetPoint);
        }

        // Abstract: Her karakter kendi aksiyonunu tanımlamak zorunda
        public abstract void Action();
    }
}