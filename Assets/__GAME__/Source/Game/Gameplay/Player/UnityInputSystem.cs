using UnityEngine;
using UnityEngine.InputSystem;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class UnityInputSystem : MonoBehaviour
    { 
        private IControllable _controllable;
        private InputSystem _input;
        
        private Vector2 _direction;
        private bool _isSprinting;
        private bool _wasAttacking;

        private InputAction _interactAction;
        private InputAction _reloadAction;
        private InputAction _switchWeaponAction;

        private void Awake()
        {
            _input = new InputSystem();
        }

        public void Init(IControllable controllable)
        {   
            _controllable = controllable;
            _input.Enable();

            _interactAction = _input.FindAction("Player/Interact");
            _reloadAction = _input.FindAction("Player/Reload");
            _switchWeaponAction = _input.FindAction("Player/SwitchWeapon");
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            if (_controllable == null) return;

            _direction = _input.Player.Move.ReadValue<Vector2>();
            _isSprinting = _input.Player.Sprint.IsPressed();

            bool isAttacking = _input.Player.Attack.IsPressed();
            if (isAttacking && !_wasAttacking)
                _controllable.StartAttack();
            else if (!isAttacking && _wasAttacking)
                _controllable.StopAttack();
            _wasAttacking = isAttacking;

            if (_interactAction != null && _interactAction.WasPressedThisFrame())
                _controllable.Interact();

            if (_reloadAction != null && _reloadAction.WasPressedThisFrame())
                _controllable.Reload();

            if (_switchWeaponAction != null && _switchWeaponAction.WasPressedThisFrame())
                _controllable.SwitchWeapon();
        }

        private void FixedUpdate()
        {
            if (_controllable == null) return;

            _controllable.Move(_direction, _isSprinting);
        }
    }
}