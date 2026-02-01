using Game.Characters;
using UnityEngine;

namespace Eren
{
    public class CombatCharacter : BaseCharacter
    {
        [Header("Combat Settings")]
        public WeaponSystem weaponSystem;
        public Transform firePoint;

        // 1. PLAYER İÇİN (Mouse Takip)
        // IPossessable'dan gelen parametresiz metod
        public override void Action()
        {
            if (weaponSystem != null)
                // Null gönderiyoruz, böylece Bullet.cs Mouse pozisyonunu kullanacak
                weaponSystem.Shoot(firePoint, null);
        }

        // 2. NPC (GUARD/SOLDIER) İÇİN (Otomatik Hedef)
        // AI scriptleri bu metodu çağıracak
        public void Action(Vector2 targetPosition)
        {
            if (weaponSystem != null)
                // AI'nın belirlediği hedefi WeaponSystem'e iletiyoruz
                weaponSystem.Shoot(firePoint, targetPosition);
        }

        // Flip karakteri hedef noktaya göre
        public override void LookAt(Vector2 targetPoint)
        {
            Vector2 direction = targetPoint - (Vector2)transform.position;

            // Sadece önemli bir fark varsa flip yap
            if (direction.x > 0.1f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}