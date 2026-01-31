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
            // Null checks
            if (currentWeapon == null)
            {
                Debug.LogError("currentWeapon is null!");
                return;
            }

            if (currentWeapon.bulletPrefab == null)
            {
                Debug.LogError($"{currentWeapon.weaponName} bulletPrefab is null!");
                return;
            }

            if (firePoint == null)
            {
                Debug.LogError("firePoint is null!");
                return;
            }

            if (Time.time >= nextFireTime && currentAmmo > 0)
            {
                // Mermi olu�turma - First create the bullet
                GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, firePoint.rotation);

                // Only update ammo and fire time if bullet was successfully created
                if (bullet != null)
                {
                    currentAmmo--;
                    nextFireTime = Time.time + currentWeapon.fireRate;
                    Debug.Log($"{currentWeapon.weaponName} ate�lendi! Kalan mermi: {currentAmmo}");
                }
                else
                {
                    Debug.LogError("Failed to instantiate bullet!");
                }
            }
        }

        public void Reload()
        {
            currentAmmo = currentWeapon.maxAmmo;
            Debug.Log("�arj�r yenilendi.");
        }
    }
}