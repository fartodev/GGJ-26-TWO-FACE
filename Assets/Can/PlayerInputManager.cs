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
                Vector2 moveDir = new Vector2(h, v).normalized;

                // Karaktere (UnitMotor'a) hareket emrini ilet
                character.Move(moveDir);
            }
        }

        // HandleLookInput içindeki değişikliği yap:
        private void HandleLookInput()
        {
            if (PossessionManager.Instance.CurrentPossessed == null) return;

            // Mouse'un dünyadaki 2D koordinatını al
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookPoint = new Vector2(mousePos.x, mousePos.y);

            PossessionManager.Instance.CurrentPossessed.LookAt(lookPoint);
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
            // HandleActionInput içindeki sol tık kısmı:
            if (Input.GetButton("Fire1"))
            {
                // Eğer possessed olan şey bir CombatCharacter ise Action() çağır (Ateş et)
                if (PossessionManager.Instance.CurrentPossessed is Eren.CombatCharacter combatChar)
                {
                    combatChar.Action();
                }
            }
        }

        // HandlePossessionInput (Sağ tık ile seçme) değişikliği:
        private void HandlePossessionInput()
        {
            if (Input.GetMouseButtonDown(1)) // Sağ Tık
            {
                Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

                // Raycast atıyoruz
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                // SENARYO 1: Bir şeye tıkladık mı?
                if (hit.collider != null)
                {
                    IPossessable target = hit.collider.GetComponent<IPossessable>();

                    // Tıkladığımız şey ele geçirilebilir bir şeyse, ona geç
                    if (target != null)
                    {
                        PossessionManager.Instance.Possess(target);
                        return; // İşlem tamam, fonksiyondan çık
                    }
                }

                // SENARYO 2: Boşluğa tıkladık veya geçersiz bir şeye tıkladık
                // Eğer şu an zaten Ruh değilsek (yani bir bedendeysek), bedenden çık.
                // (PossessionManager'da playerSoul tanımlı olmalı)
                if (PossessionManager.Instance.CurrentPossessed != null)
                {
                    // Buraya "Ruh formunda değilsek" kontrolü eklemek daha güvenli olur
                    // Ancak basitçe Depossess çağırmak da iş görür, Manager kontrol eder.
                    PossessionManager.Instance.Depossess();
                }
            }
        }
    }
}