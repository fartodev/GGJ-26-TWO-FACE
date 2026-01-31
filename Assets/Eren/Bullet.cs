using UnityEngine;

namespace Eren
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public float damage = 10f;
        public float lifeTime = 3f;

        void Start()
        {
            // Belirlenen süre sonunda mermiyi yok et
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            // HATA DÜZELTME: Vector3.forward yerine Vector2.right kullanýyoruz.
            // 2D'de 'Space.Self' (yerel eksen) üzerinden merminin 'Sað' yönüne ilerlemesini saðlarýz.
            // Eðer mermi sprite'ýn yukarý bakýyorsa Vector2.up kullanabilirsin.
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // IDamageable arayüzüne sahip bir objeye çarptýk mý?
            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"{other.name} hedefine {damage} hasar verildi!");
            }

            // Çarpýnca mermiyi yok et (Duvara veya düþmana)
            Destroy(gameObject);
        }
    }
}