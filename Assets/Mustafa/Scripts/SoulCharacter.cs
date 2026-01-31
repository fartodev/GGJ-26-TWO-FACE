using Game.Characters;
using UnityEngine;

namespace Mustafa
{
    public class SoulCharacter : BaseCharacter
    {
        [Header("Soul Settings")]
        [SerializeField] private float soulSpeed = 8f; // Ruh daha hýzlý olabilir
        [SerializeField] private Collider2D soulCollider;

        protected override void Awake()
        {
            base.Awake(); // UnitMotor'u alýr
            characterName = "The Soul";
            
            // Ruhun baþlangýç hýzý ayarý
            if (TryGetComponent(out UnitMotor motor))
            {
                motor._moveSpeed = soulSpeed;
            }
        }

        // Ruhun özel bir Action'ý olabilir (örneðin baðýrýp dikkat çekmek)
        // Þimdilik boþ býrakýyoruz, sað týk (Possess) zaten InputManager'da.
        public override void Action() 
        {
            Debug.Log("Ruh formundasýn, bir bedene girmelisin!");
        }

        // Ruh hasar almaz (Ölümsüzlük)
        // Eðer HealthSystem eklersen, TakeDamage metodunu override edip boþ býrakabilirsin.
    }
}