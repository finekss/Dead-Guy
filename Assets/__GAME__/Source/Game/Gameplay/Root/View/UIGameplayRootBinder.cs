using System;
using R3;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubject;

        public void HandleGotoMainMenuClicked()
        {
            _exitSceneSignalSubject?.OnNext(Unit.Default);
        }

        public void Bind(Subject<Unit> exitSceneSignalSubject)
        {
            _exitSceneSignalSubject = exitSceneSignalSubject;
        }
    }
}