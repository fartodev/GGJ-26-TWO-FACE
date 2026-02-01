using UnityEngine;
using Game.Core;

namespace Game.Characters
{
    [RequireComponent(typeof(UnitMotor))]
    [RequireComponent(typeof(Animator))]
    public abstract class BaseCharacter : MonoBehaviour, IPossessable
    {
        [SerializeField] private UnitMotor _motor;
        [SerializeField] protected string characterName;
        [SerializeField] private bool _isPossessed = false; // Ele geçirilme durumu

        protected Animator _animator;
        private Rigidbody2D _rb;

        protected virtual void Awake()
        {
            if (_motor == null) _motor = GetComponent<UnitMotor>(); //
            _animator = GetComponent<Animator>(); //
            _rb = GetComponent<Rigidbody2D>(); //
        }

        protected virtual void Update()
        {
            HandleAnimation(); //
        }

        private void HandleAnimation()
        {
            if (_animator == null || _rb == null) return; //

            // 1. Hız ve Hareket Kontrolü
            float vx = _rb.linearVelocity.x; //
            bool isMoving = _rb.linearVelocity.magnitude > 0.1f; //

            // 2. Kesin Flip Mantığı (Scale kullanarak)
            // Sadece hareket varsa yön değiştir, dururken son baktığı yönde kalsın
            if (vx > 0.1f) transform.localScale = new Vector3(1 , 1 , 1);
            else if (vx < -0.1f) transform.localScale = new Vector3(-1 , 1 , 1);

            // 3. Animator Parametrelerini Gönder
            // Animator'da 'isWalking' (Bool) ve 'isPossessed' (Bool) tanımlı olmalı
            _animator.SetBool("isWalking", isMoving);
            _animator.SetBool("isPossessed", _isPossessed);
        }

        public virtual void OnPossess()
        {
            _isPossessed = true; //
            _motor.SetPlayerControl(true); //
        }

        public virtual void OnDepossess()
        {
            _isPossessed = false; //
            _motor.SetPlayerControl(false); //
        }

        public virtual void Move(Vector2 direction) => _motor.MoveByInput(direction); //
        public virtual void LookAt(Vector2 targetPoint) { } //
        public abstract void Action(); //
    }
}