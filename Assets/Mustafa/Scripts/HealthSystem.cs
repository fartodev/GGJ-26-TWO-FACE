using UnityEngine;
using Eren; // IDamageable ve Combat sistemleri için
using Can;  // PossessionManager için

namespace Mustafa
{
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        [Header("Status")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Death Settings")]
        [SerializeField] private GameObject deathEffect; // Patlama efekti (Opsiyonel)
        [SerializeField] private bool destroyOnDeath = true;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        // Eren'in Bullet.cs'i veya WeaponSystem'i burayý çaðýracak
        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"{gameObject.name} hasar aldý! [{currentHealth}/{maxHealth}]");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 1. Ölen kiþi þu an oyuncunun kontrol ettiði karakter mi?
            // (Interface olduðu için cast edip kontrol ediyoruz)
            MonoBehaviour currentPossessedObj = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;

            if (currentPossessedObj != null && currentPossessedObj.gameObject == this.gameObject)
            {
                Debug.LogWarning("OYUNCU ÖLDÜ! Ruh serbest kalýyor...");
                PossessionManager.Instance.Depossess();

                // Ýleride buraya "Game Over" veya "Ruh Moduna Geç" ekraný eklenecek
            }
            else
            {
                Debug.Log($"Düþman öldü: {gameObject.name}");
            }

            // 2. Efekt varsa oluþtur
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            // 3. Objeyi yok et
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        // Test için dýþarýdan iyileþtirme fonksiyonu
        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }
    }
}