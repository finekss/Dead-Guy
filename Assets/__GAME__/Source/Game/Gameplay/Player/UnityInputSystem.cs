using System;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class UnityInputSystem : MonoBehaviour
    { 
        public GameObject _player;
        
        private IControllable _controllable;
        private InputSystem _input;
        
        private Vector2 direction;


        private void Awake()
        {
            _input = new InputSystem();
        }
        private void Start()
        {
            if(_player == null) throw new NullReferenceException("Player is null");
            _controllable = _player.GetComponent<IControllable>();
            if(_controllable == null) throw new NullReferenceException("IControllable is null");
        }

        private void OnEnable()
        {
            _input.Enable();
        }
        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            Movement();
        }
        
        private void Movement()
        {
            direction = _input.Player.Move.ReadValue<Vector2>();
            if (_input.Player.Sprint.IsPressed()) _controllable.Move(direction, true);
            else _controllable.Move(direction, false);
        }
    }
}