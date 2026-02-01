using Game.Characters;
using UnityEngine;

namespace Eren
{
    public class CombatCharacter : BaseCharacter
    {
        [Header("Simple Setup")]
        [Tooltip("Doğrudan silahın olduğu obje (WeaponSprite)")]
        public Transform weaponTransform;

        [Tooltip("Silahın SpriteRenderer'ı (Ters dönmemesi için FlipY yapacağız)")]
        public SpriteRenderer weaponRenderer;

        [Tooltip("Gövde SpriteRenderer'ı (Sağa/Sola FlipX yapacağız)")]
        public SpriteRenderer bodyRenderer;

        [Header("Combat Settings")]
        public WeaponSystem weaponSystem;
        public Transform firePoint;

        public override void LookAt(Vector2 targetPoint)
        {
            if (weaponTransform == null) return;

            // 1. AÇI HESAPLAMA (360 Derece)
            Vector2 direction = targetPoint - (Vector2)weaponTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Silahı o açıya döndür (Z ekseninde)
            weaponTransform.rotation = Quaternion.Euler(0, 0, angle);

            // 2. SİLAH FLIP (BAŞ AŞAĞI DURMASIN)
            // Eğer açı sol taraftaysa (90 ile -90 dışı), silahı Y ekseninde ters çevir
            if (weaponRenderer != null)
            {
                // Silah soldaysa FlipY yap, sağdaysa düzelt
                weaponRenderer.flipY = Mathf.Abs(angle) > 90f;
            }

            // 3. GÖVDE FLIP (KARAKTER YÖNÜ)
            // Gövde sadece sağa/sola bakar
            if (bodyRenderer != null)
            {
                // Mouse karakterin solunda mı?
                bool lookLeft = direction.x < 0;

                // Sprite orijinalde sağa bakıyorsa: Sola bakınca flipX = true
                bodyRenderer.flipX = lookLeft;
            }
        }

        // --- ATEŞ ETME ---
        public override void Action()
        {
            if (weaponSystem != null) weaponSystem.Shoot(firePoint, null);
        }

        public void Action(Vector2 targetPos)
        {
            if (weaponSystem != null) weaponSystem.Shoot(firePoint, targetPos);
        }
    }
}