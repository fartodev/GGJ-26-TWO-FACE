// FILE: Scripts/Core/IPossessable.cs
using UnityEngine;

namespace Game.Core
{
    public interface IPossessable
    {
        // Possession State Management
        void OnPossess();
        void OnDepossess();

        // Locomotion & Physics
        void Move(Vector2 direction);
        void LookAt(Vector2 targetPoint);

        // Gameplay Actions
        void Action(); // Context sensitive (Shoot or Interact)
    }
}