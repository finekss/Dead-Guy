using __GAME__.Source.Game.Gameplay.Root.View;
using __GAME__.Source.Game.GameRoot;
using __GAME__.Source.Game.MainMenu.Root;
using BaCon;
using R3;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Root
{
    public class GameplayEntryPoint: MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        
        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEntryParams entryParams)
        {
            var uiRoot = gameplayContainer.Resolve<UIRootView>();
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);
            
            var exitSceneSignalSubject = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubject);

            var mainMenuEnterParams = new MainMenuEntryParams(true);
            var exitParams = new GameplayExitParams(mainMenuEnterParams);
            var exitToMainMenuSceneSignal = exitSceneSignalSubject.Select(_ => exitParams);
            
            return exitToMainMenuSceneSignal;
        }
    }
}