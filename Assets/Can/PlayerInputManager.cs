using UnityEngine;
using Game.Core;
using Game.Characters;

namespace Can
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask groundLayer; // Mouse ile bakış yönü için zemin layer'ı

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleMovementInput();
            HandleLookInput();
            HandleActionInput();
            HandlePossessionInput(); // Test amaçlı veya SoulProjectile fırlatma için
        }

        private void HandleMovementInput()
        {
            // Aktif bir karakter yoksa işlem yapma
            if (PossessionManager.Instance.CurrentPossessed == null) return;

            // Şu anki possessed obje bir BaseCharacter mi?
            // (Hareket mantığı BaseCharacter ve UnitMotor üzerinde olduğu için cast ediyoruz)
            BaseCharacter character = PossessionManager.Instance.CurrentPossessed as BaseCharacter;

            if (character != null)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");

                // Girdiyi vektöre çevir
                Vector3 moveDir = new Vector3(h, 0, v).normalized;

                // Karaktere (UnitMotor'a) hareket emrini ilet
                character.Move(moveDir);
            }
        }

        private void HandleLookInput()
        {
            if (PossessionManager.Instance.CurrentPossessed == null) return;

            // Raycast ile mouse'un dünyadaki yerini bul
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                Vector3 lookPoint = hit.point;
                lookPoint.y = 0; // Y eksenini kilitle (Top-down olduğu için)

                // Interface üzerindeki LookAt metodunu çağır
                PossessionManager.Instance.CurrentPossessed.LookAt(lookPoint);
            }
        }

        private void HandleActionInput()
        {
            if (PossessionManager.Instance.CurrentPossessed == null) return;

            // 'E' tuşu ile etkileşim (Interact/Action)
            if (Input.GetKeyDown(KeyCode.E))
            {
                BaseCharacter character = PossessionManager.Instance.CurrentPossessed as BaseCharacter;
                if (character != null)
                {
                    character.Action(); // Örn: Kapı açma, bilgisayar kullanma
                }
            }

            // Sol Tık ile Ateş etme (CombatCharacter ise)
            if (Input.GetButton("Fire1"))
            {
                // Burası daha sonra Mustafa'nın CombatCharacter'i ile bağlanacak.
                // Şimdilik sadece BaseCharacter üzerinden gidiyoruz, ileride cast edip Attack çağıracağız.
            }
        }

        private void HandlePossessionInput()
        {
            // ŞİMDİLİK TEST İÇİN: Sağ tık ile mouse'un altındaki objeyi possess et.
            // İLERİDE: Burası SoulProjectile fırlatacak.
            if (Input.GetMouseButtonDown(1)) // Sağ Tık
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Tıklanan objede IPossessable var mı?
                    IPossessable target = hit.collider.GetComponent<IPossessable>();
                    if (target != null)
                    {
                        PossessionManager.Instance.Possess(target);
                    }
                }
            }
        }
    }
}