using System;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        public event Action GotoMainMenuClicked;

        public void HandleGotoMainMenuClicked()
        {
            GotoMainMenuClicked?.Invoke();
        }
    }
}