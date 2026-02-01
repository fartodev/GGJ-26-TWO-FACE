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
        void Start()
        {
            // Belirlenen s�re sonunda mermiyi yok et
            Destroy(gameObject, lifeTime);
        }

        public void Shot()
        {
            Vector2 mouseWorld = Can.PlayerInputManager.LastMouseWorldPosition;
            Vector2 bulletPos = transform.position;
            dir = (mouseWorld - bulletPos).normalized;
            shoting = true;
        }

        void Update()
        {
            if (!shoting)
            {
                return;
            }
            // Mouse pozisyonuna doğru ilerle

            transform.position += (Vector3)dir * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // IDamageable aray�z�ne sahip bir objeye �arpt�k m�?
            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"{other.name} hedefine {damage} hasar verildi!");
            }

            // �arp�nca mermiyi yok et (Duvara veya d��mana)
            Destroy(gameObject);
        }
    }
}