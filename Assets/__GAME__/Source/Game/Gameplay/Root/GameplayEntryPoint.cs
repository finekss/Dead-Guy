using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Root
{
    public class GameplayEntryPoint: MonoBehaviour

    {
        [SerializeField] private GameObject _sceneRootBinder;

        public void Run()
        {
            print("Gameplay started");
        }
    }
}