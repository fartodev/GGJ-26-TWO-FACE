using UnityEngine;

namespace Game.Characters
{
    [RequireComponent(typeof(Rigidbody2D))] // 2D Fizik
    public class UnitMotor : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        //[SerializeField] private float _rotationSpeed = 720f; // 2D'de derece cinsinden

        private Rigidbody2D _rb;
        private bool _isPlayerControlled = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f; // Top-down olduðu için yerçekimini kapat
        }

        public void SetPlayerControl(bool isPlayer)
        {
            _isPlayerControlled = isPlayer;
            // 2D'de NavMeshAgent (varsayýlan) yerine basit hareket kullanacaðýz 
            // veya 2D NavMesh eklentisi kullanmalýsýn.
        }

        public void MoveByInput(Vector2 direction) // Vector3 -> Vector2
        {
            if (!_isPlayerControlled) return;
            _rb.linearVelocity = direction.normalized * _moveSpeed; // Unity 2023+ (Eski sürümse .velocity)
        }

       // public void RotateTowards(Vector2 targetPoint)
       // {
            // 2D Bakýþ Mantýðý: Karakterin baktýðý yönü (Right veya Up) hedefe çevir
            //Vector2 lookDir = targetPoint - (Vector2)transform.position;
            //float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            // Karakterin sprite'ý saða bakýyorsa 0, yukarý bakýyorsa -90 ofset gerekebilir
            //Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
       // }
    }
}