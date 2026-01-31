using Game.Characters;
using UnityEngine;

namespace Mustafa
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool isOpen = false;

        public void Interact(BaseCharacter actor)
        {
            isOpen = !isOpen;
            // Burada animasyon veya sprite deðiþimi tetiklenir
            Debug.Log(isOpen ? "Kapý Açýldý" : "Kapý Kapandý");
        }

        public string GetInteractText() => isOpen ? "Kapat" : "Aç";
    }
}