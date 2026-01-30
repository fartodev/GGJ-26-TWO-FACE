using UnityEngine;

namespace Eren
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public float damage = 10f;
        public float lifeTime = 3f;

        void Start() => Destroy(gameObject, lifeTime);

        void Update() => transform.Translate(Vector3.forward * speed * Time.deltaTime);

        private void OnTriggerEnter2D(Collider2D other) // 2D versiyonu
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}