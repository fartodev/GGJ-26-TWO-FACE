using UnityEngine;
using TestTunax;

namespace Eren
{
    public class Guard : CombatCharacter
    {
        [Header("AI References")]
        [Tooltip("GuardSystem scripti Child objede ise buraya sürükleyin.")]
        [SerializeField] private GuardSystem _guardSystem;

        protected override void Awake()
        {
            characterName = "Security Guard";

            // Eğer Inspector'dan atamayı unuttuysan, kod otomatik bulmaya çalışsın:
            if (_guardSystem == null)
            {
                // Önce kendi üzerinde ara
                _guardSystem = GetComponent<GuardSystem>();

                // Bulamazsa Child objelerde ara (SENİN SORUNUNU ÇÖZEN SATIR BU)
                if (_guardSystem == null)
                {
                    _guardSystem = GetComponentInChildren<GuardSystem>();
                }
            }

            base.Awake();
        }

        public override void OnPossess()
        {
            base.OnPossess();
            if (_guardSystem != null)
            {
                _guardSystem.StopMove();
                _guardSystem.enabled = false; // AI'yı sustur
            }
            else
            {
                Debug.LogError($"{name}: GuardSystem bulunamadı! AI durdurulamıyor.");
            }
        }

        public override void OnDepossess()
        {
            base.OnDepossess();
            if (_guardSystem != null)
                _guardSystem.enabled = true;
        }
    }
}