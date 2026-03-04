using System;
using UnityEngine;

namespace __GAME__.Source.Game.MainMenu.Root.View
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        public event Action GotoGameplayClicked;

        public void HandleGotoGameplayClicked()
        {
            GotoGameplayClicked?.Invoke();
        }
    }
}