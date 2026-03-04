using System;
using __GAME__.Source.Game.Gameplay.Root.View;
using __GAME__.Source.Game.GameRoot;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Root
{
    public class GameplayEntryPoint: MonoBehaviour

    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        
        public event Action GotoMainMenuRequested;
        public void Run(UIRootView uiRoot)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            uiScene.GotoMainMenuClicked += () =>
            {
                GotoMainMenuRequested?.Invoke();
            };
        }
    }
}