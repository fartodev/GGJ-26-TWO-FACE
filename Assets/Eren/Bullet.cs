using UnityEngine;
using Can;

namespace Eren
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public float damage = 10f;
        public float lifeTime = 3f;
        public Vector2 dir;

        private bool shoting;
        private GameObject _owner; // Mermiyi ateşleyen kişi

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        // 1. YENİ: Merminin sahibini belirle
        public void SetOwner(GameObject owner)
        {
            _owner = owner;

            // Ekstra Güvenlik: Fizik motoruna bu iki collider'ın çarpışmasını yoksaymasını söyle
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D ownerCollider = owner.GetComponent<Collider2D>();

            if (myCollider != null && ownerCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, ownerCollider);
            }
        }

        public void Shot(Vector2? targetPosition = null)
        {
            Vector2 targetWorld;

            if (targetPosition.HasValue)
            {
                targetWorld = targetPosition.Value;
            }
            else
            {
                targetWorld = Can.PlayerInputManager.LastMouseWorldPosition;
            }

            Vector2 bulletPos = transform.position;
            dir = (targetWorld - bulletPos).normalized;
            shoting = true;
        }

        void Update()
        {
            if (!shoting) return;
            // Space.Self yerine World space kullanıyoruz ki yön karışmasın
            transform.position += (Vector3)dir * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 2. KONTROL: Eğer çarptığım şey sahibimse (kendimse) işlem yapma
            if (_owner != null && other.gameObject == _owner) return;

            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"{other.name} hedefine {damage} hasar verildi!");
            }

            Destroy(gameObject);
        }
    }
}