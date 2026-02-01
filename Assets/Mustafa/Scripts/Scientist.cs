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

        // Flip karakteri hedef noktaya göre (player kontrolündeyken)
        public override void LookAt(Vector2 targetPoint)
        {
            Vector2 direction = targetPoint - (Vector2)transform.position;

            // Sadece önemli bir fark varsa flip yap
            if (direction.x > 0.1f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        public override void Action()
        {
            // Tek bir obje yerine, alandaki T�M objeleri al�yoruz
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);

            bool interactFound = false;

            foreach (Collider2D hit in hits)
            {
                // E�er �arpt���m�z obje kendimizsek (Scientist), bunu atla ve d�ng�ye devam et
                if (hit.gameObject == this.gameObject) continue;

                IInteractable target = hit.GetComponent<IInteractable>();
                if (target != null)
                {
                    Debug.Log($"Etkile�im Ba�ar�l�: {hit.name}");
                    target.Interact(this); // Kendimizi (this) g�nderiyoruz
                    interactFound = true;
                    break; // �lk ge�erli etkile�imde d�ng�y� k�r (ayn� anda 5 kap� a��lmas�n)
                }
            }

            if (!interactFound)
            {
                Debug.Log("Menzilde etkile�ime girilecek ge�erli bir nesne yok.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}