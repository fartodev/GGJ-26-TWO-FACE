using Game.Characters;
using UnityEngine;

namespace Eren
{
    public class CombatCharacter : BaseCharacter
    {
        [Header("Combat Settings")]
        public WeaponSystem weaponSystem;
        public Transform firePoint;

        // IPossessable'dan gelen Action metodunu silah ate�lemek i�in override ediyoruz
        public override void Action()
        {
            if (weaponSystem != null)
                weaponSystem.Shoot(firePoint);
        }
    }
}