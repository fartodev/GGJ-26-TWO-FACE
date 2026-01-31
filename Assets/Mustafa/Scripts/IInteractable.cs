using Game.Characters; // Bunu eklemeyi unutma

namespace Mustafa
{
    public interface IInteractable
    {
        // CombatCharacter yerine BaseCharacter olmalý
        void Interact(BaseCharacter actor);
        string GetInteractText();
    }
}