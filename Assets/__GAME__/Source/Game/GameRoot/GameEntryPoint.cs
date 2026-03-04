using System.Collections;
using __GAME__.Source.Game.Gameplay.Root;
using __GAME__.Source.Game.MainMenu.Root;
using __GAME__.Source.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __GAME__.Source.Game.GameRoot
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private Coroutines _coroutines;
        private UIRootView _uiRoot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AfterStartGame()
        {
            Application.targetFrameRate = 60; 
            
            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[Coroutines]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            
            var prefabUiRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUiRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            
        }
        private void RunGame()
        {
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;
            
            if (sceneName == Scenes.GAMEPLAY)
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
                return; 
            }

            if (sceneName == Scenes.MAINMENU)
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
            }

            if (sceneName != Scenes.BOOT)
            {
                return;
            }
#endif

            _coroutines.StartCoroutine(LoadAndStartMainMenu());
        }

        private IEnumerator LoadAndStartGameplay()
        {
            _uiRoot.ShowLoadingScreen();
            
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);
            yield return null;

            var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_uiRoot);

            sceneEntryPoint.GotoMainMenuRequested += () =>
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
            };
            
            _uiRoot.HideLoadingScreen();
        }
        
        private IEnumerator LoadAndStartMainMenu()
        {
            _uiRoot.ShowLoadingScreen();
            
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAINMENU);
            yield return null;

            var sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot);
            
            sceneEntryPoint.GotoGameplayRequested += () =>
            {
                _coroutines.StartCoroutine(LoadAndStartGameplay());
            };
            
            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}