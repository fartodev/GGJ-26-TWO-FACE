using UnityEngine;

namespace Eren
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
    public class Weapon : ScriptableObject
    {
        public string weaponName;
        public float damage = 10f;
        public float fireRate = 0.5f;
        public int maxAmmo = 30;
        public float range = 50f;
        public GameObject bulletPrefab;
    }
}