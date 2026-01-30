using UnityEngine;

namespace Eren
{
    // Guard: Daha yavaþ ateþ eden, düþük menzilli birim (Pistol)
    public class Guard : CombatCharacter
    {

        protected override void Awake()
        {
            // Önce veriyi set ediyoruz, sonra base logic'i çalýþtýrýyoruz
            characterName = "Security Guard";

            // Eðer BaseCharacter veya CombatCharacter içinde Awake mantýðý varsa onu çaðýrýyoruz
            base.Awake();
        }
    }

    // Soldier: Hýzlý ateþ eden, yüksek menzilli birim (Rifle)
    public class Soldier : CombatCharacter
    {
        protected override void Awake()
        {
            characterName = "Elite Soldier";
            base.Awake();
        }
    }
}