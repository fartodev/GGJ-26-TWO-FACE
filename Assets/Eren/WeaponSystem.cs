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

        public void Shoot(Transform firePoint)
        {
            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                nextFireTime = Time.time + currentWeapon.fireRate;
                currentAmmo--;

                // Mermi oluþturma
                Instantiate(currentWeapon.bulletPrefab, firePoint.position, firePoint.rotation);
                Debug.Log($"{currentWeapon.weaponName} ateþlendi! Kalan mermi: {currentAmmo}");
            }
        }

        public void Reload()
        {
            currentAmmo = currentWeapon.maxAmmo;
            Debug.Log("Þarjör yenilendi.");
        }
    }
}