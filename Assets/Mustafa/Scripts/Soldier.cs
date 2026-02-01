using UnityEngine;
using TestTunax; // SoldierSystem burada

namespace Eren
{
    public class Soldier : CombatCharacter
    {
        [Header("AI References")]
        // SoldierSystem'i buraya sürükle veya otomatik bulmasýna izin ver
        [SerializeField] private SoldierSystem _soldierSystem;

        protected override void Awake()
        {
            characterName = "Elite Soldier";

            // AI Scriptini otomatik bul
            if (_soldierSystem == null)
            {
                _soldierSystem = GetComponent<SoldierSystem>();
                // Eðer kendinde yoksa child objelerde ara
                if (_soldierSystem == null)
                    _soldierSystem = GetComponentInChildren<SoldierSystem>();
            }

            base.Awake();
        }

        // 1. Ele geçirildiðinde: AI'yý KAPAT
        public override void OnPossess()
        {
            base.OnPossess();

            if (_soldierSystem != null)
            {
                _soldierSystem.StopMove(); // Devriyeyi durdur (Tween kill)
                _soldierSystem.enabled = false; // Update fonksiyonunu durdur
            }
        }

        // 2. Býrakýldýðýnda: AI'yý AÇ
        public override void OnDepossess()
        {
            base.OnDepossess();

            if (_soldierSystem != null)
            {
                _soldierSystem.enabled = true; // Update fonksiyonunu baþlat
                _soldierSystem.MoveStart(); // Devriyeyi tekrar baþlat
            }
        }
    }
}