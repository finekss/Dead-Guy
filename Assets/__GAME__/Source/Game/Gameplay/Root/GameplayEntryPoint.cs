using __GAME__.Source.Game.Gameplay.Player;
using __GAME__.Source.Game.Gameplay.Root.View;
using __GAME__.Source.Game.Gameplay.Weapon;
using __GAME__.Source.Game.Gameplay.Weapon.Pickup;
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
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private GameObject _inputSystemPrefab;
        [SerializeField] private Camera _playerCamera;
        
        
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

            InitPlayer(gameplayContainer);
            
            return exitToMainMenuSceneSignal;
        }

        private void InitPlayer(DIContainer gameplayContainer)
        {
            gameplayContainer.RegisterFactory(c => new CharacterModel()).AsSingle();

            var characterObject = Instantiate(_characterPrefab, Vector3.zero, Quaternion.identity);
            var characterView = characterObject.GetComponent<CharacterView>();
            gameplayContainer.RegisterInstance(characterView);

            var weaponSlot = characterObject.GetComponent<WeaponSlot>();
            if (weaponSlot == null)
                weaponSlot = characterObject.AddComponent<WeaponSlot>();

            Transform weaponPivot = characterView.WeaponPivot;
            if (weaponPivot == null)
            {
                var pivotObject = new GameObject("WeaponPivot");
                pivotObject.transform.SetParent(characterObject.transform);
                pivotObject.transform.localPosition = new Vector3(0.5f, 1f, 0.5f);
                weaponPivot = pivotObject.transform;
            }

            var weaponInventory = new WeaponInventory(weaponPivot, weaponSlot.HitMask);
            gameplayContainer.RegisterInstance(weaponInventory);

            var pickupSystem = characterObject.GetComponent<WeaponPickupSystem>();
            if (pickupSystem == null)
                pickupSystem = characterObject.AddComponent<WeaponPickupSystem>();

            Camera cam = _playerCamera != null ? _playerCamera : Camera.main;
            pickupSystem.Init(weaponInventory, cam);
            gameplayContainer.RegisterInstance(pickupSystem);

            gameplayContainer.RegisterFactory(c =>
                new CharacterPresenter(
                    c.Resolve<CharacterView>(),
                    c.Resolve<CharacterModel>(),
                    c.Resolve<WeaponInventory>(),
                    c.Resolve<WeaponPickupSystem>()
                )).AsSingle();

            var characterPresenter = gameplayContainer.Resolve<CharacterPresenter>();
            
            var inputObject = Instantiate(_inputSystemPrefab, Vector3.zero, Quaternion.identity);
            var inputSystem = inputObject.GetComponent<UnityInputSystem>();
            inputSystem.Init(characterPresenter);
        }
    }
}