using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class UnityInputSystem : MonoBehaviour
    { 
        private IControllable _controllable;
        private InputSystem _input;
        
        private Vector2 _direction;
        private bool _isSprinting;

        private void Awake()
        {
            _input = new InputSystem();
        }

        public void Init(IControllable controllable)
        {   
            _controllable = controllable;
            _input.Enable();
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

            if (_input.Player.Attack.IsPressed())
                _controllable.Attack();
        }

        private void FixedUpdate()
        {
            if (_controllable == null) return;

            _controllable.Move(_direction, _isSprinting);
        }
    }
}