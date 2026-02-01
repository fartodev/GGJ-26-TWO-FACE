using Game.Characters;
using UnityEngine;

namespace Eren
{
    public class WeaponSystem : MonoBehaviour
    {
        public Weapon currentWeapon;
        private float nextFireTime;
        private int currentAmmo;

        void Start() => currentAmmo = currentWeapon.maxAmmo;

        public void Shoot(Transform firePoint, Vector2? targetPosition = null)
        {
            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                nextFireTime = Time.time + currentWeapon.fireRate;
                currentAmmo--;

                // Mermiyi oluştur
                var bulletObj = Instantiate(currentWeapon.bulletPrefab, firePoint.position, firePoint.rotation);
                var bulletScript = bulletObj.GetComponent<Bullet>();

                // 1. YENİ: Sahibini (Bu silahı tutan karakteri) mermiye ata
                // "gameObject" bu scriptin bağlı olduğu karakterdir (Guard/Soldier)
                bulletScript.SetOwner(gameObject);

                // 2. Ateşle
                bulletScript.Shot(targetPosition);

                Debug.Log($"{currentWeapon.weaponName} ateşlendi! Kalan: {currentAmmo}");
            }
        }

        public void Reload()
        {
            currentAmmo = currentWeapon.maxAmmo;
        }
    }
}