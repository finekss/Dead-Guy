using UnityEngine;
using System;
using __GAME__.Source.Game.GameRoot;
using __GAME__.Source.Game.MainMenu.Root.View;

namespace __GAME__.Source.Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;
        
        public event Action GotoGameplayRequested;
        public void Run(UIRootView uiRoot, MainMenuEntryParams entryParams)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            uiScene.GotoGameplayClicked += () =>
            {
                GotoGameplayRequested?.Invoke();
            };
        }
    }
}