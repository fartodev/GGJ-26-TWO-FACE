using UnityEngine;
using TestTunax;

namespace Eren
{
    public class Guard : CombatCharacter
    {
        [Header("AI References")]
        [SerializeField] private GuardSystem _guardSystem;

        protected override void Awake()
        {
            characterName = "Security Guard";

            if (_guardSystem == null)
            {
                _guardSystem = GetComponent<GuardSystem>();
                if (_guardSystem == null)
                    _guardSystem = GetComponentInChildren<GuardSystem>();
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
        }

        public override void OnDepossess()
        {
            base.OnDepossess();
            if (_guardSystem != null)
            {
                _guardSystem.enabled = true;

                // EKLEMEN GEREKEN SATIR BU:
                _guardSystem.MoveStart(); // AI tekrar devriyeye başlasın
            }
        }
    }
}