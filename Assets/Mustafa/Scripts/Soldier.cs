using UnityEngine;
using TestTunax; // SoldierSystem burada

namespace Eren
{
    public class Soldier : CombatCharacter
    {
        [Header("AI References")]
        [Tooltip("SoldierSystem scripti Child objede veya bu objede ise buraya sürükleyin.")]
        [SerializeField] private SoldierSystem _soldierSystem;

        protected override void Awake()
        {
            // Soldier'a özel isim ve ayarlar
            characterName = "Elite Soldier";

            // AI Scriptini otomatik bul (Guard.cs'deki fix ile ayný mantýk)
            if (_soldierSystem == null)
            {
                _soldierSystem = GetComponent<SoldierSystem>();
                if (_soldierSystem == null)
                {
                    _soldierSystem = GetComponentInChildren<SoldierSystem>();
                }
            }

            base.Awake();
        }

        // Oyuncu bu bedene girdiðinde AI kapanmalý
        public override void OnPossess()
        {
            base.OnPossess();

            if (_soldierSystem != null)
            {
                _soldierSystem.StopMove(); // Devriyeyi durdur
                _soldierSystem.enabled = false; // AI beynini kapat
            }
            else
            {
                Debug.LogError($"{name}: SoldierSystem bulunamadý! AI durdurulamýyor.");
            }
        }

        // Oyuncu bedenden çýktýðýnda AI tekrar devreye girmeli
        public override void OnDepossess()
        {
            base.OnDepossess();

            if (_soldierSystem != null)
            {
                _soldierSystem.enabled = true; // AI beynini aç
                _soldierSystem.MoveStart(); // Devriyeye kaldýðý yerden veya baþtan baþla
            }
        }
    }
}