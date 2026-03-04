using UnityEngine;
using System;
using __GAME__.Source.Game.Gameplay.Root;
using __GAME__.Source.Game.GameRoot;
using __GAME__.Source.Game.MainMenu.Root.View;
using BaCon;
using R3;

namespace __GAME__.Source.Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;
        
        public Observable<MainMenuExitParams> Run(DIContainer mainMenuContainer, MainMenuEntryParams entryParams)
        {
            var uiRoot = mainMenuContainer.Resolve<UIRootView>();
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            var exitSceneSignalSubject = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubject);
            
            var gameplayEnterParams = new GameplayEntryParams(4,4,"12","11");
            var exitParams = new MainMenuExitParams(gameplayEnterParams);
            var exitToGameplaySceneSignal = exitSceneSignalSubject.Select(_ => exitParams);

            return exitToGameplaySceneSignal;
        }
    }
}