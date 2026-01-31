using Game.Characters;
using UnityEngine;
using System.Collections;

namespace Mustafa
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private bool isOpen = false;
        [SerializeField] private Vector2 openOffset = new Vector2(1.5f, 0f); // Saða doðru 1.5 birim kaydýr
        [SerializeField] private float slideSpeed = 5f;

        private Vector2 _closedPos;
        private Vector2 _targetPos;
        private BoxCollider2D _doorCollider;

        private void Awake()
        {
            _doorCollider = GetComponent<BoxCollider2D>();
            _closedPos = transform.position;
            _targetPos = isOpen ? _closedPos + openOffset : _closedPos;

            UpdateCollider();
        }

        public void Interact(BaseCharacter actor)
        {
            isOpen = !isOpen;
            _targetPos = isOpen ? _closedPos + openOffset : _closedPos;

            UpdateCollider();

            // Kapýyý kaydýrmak için Coroutine baþlatýyoruz
            StopAllCoroutines();
            StartCoroutine(SlideDoor());

            Debug.Log(isOpen ? "Kapý Saða Açýldý" : "Kapý Kapandý");
        }

        private IEnumerator SlideDoor()
        {
            while (Vector2.Distance(transform.position, _targetPos) > 0.01f)
            {
                transform.position = Vector2.Lerp(transform.position, _targetPos, Time.deltaTime * slideSpeed);
                yield return null;
            }
            transform.position = _targetPos;
        }

        private void UpdateCollider()
        {
            if (_doorCollider != null)
            {
                // Kapý tam kapalýyken engel olmalý, açýlmaya baþladýðýnda geçilebilmeli
                _doorCollider.enabled = !isOpen;
            }
        }

        public string GetInteractText() => isOpen ? "Kapat" : "Aç";
    }
}