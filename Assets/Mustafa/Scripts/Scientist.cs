using Game.Characters;
using UnityEngine;

namespace Mustafa
{
    public class Scientist : BaseCharacter
    {
        [Header("Scientist Settings")]
        [SerializeField] private float interactRange = 2f;
        [SerializeField] private LayerMask interactableLayer;

        protected override void Awake()
        {
            characterName = "Head Scientist";
            base.Awake();
        }

        public override void Action()
        {
            // Tek bir obje yerine, alandaki TÜM objeleri alýyoruz
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);

            bool interactFound = false;

            foreach (Collider2D hit in hits)
            {
                // Eðer çarptýðýmýz obje kendimizsek (Scientist), bunu atla ve döngüye devam et
                if (hit.gameObject == this.gameObject) continue;

                IInteractable target = hit.GetComponent<IInteractable>();
                if (target != null)
                {
                    Debug.Log($"Etkileþim Baþarýlý: {hit.name}");
                    target.Interact(this); // Kendimizi (this) gönderiyoruz
                    interactFound = true;
                    break; // Ýlk geçerli etkileþimde döngüyü kýr (ayný anda 5 kapý açýlmasýn)
                }
            }

            if (!interactFound)
            {
                Debug.Log("Menzilde etkileþime girilecek geçerli bir nesne yok.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}