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
    }
}