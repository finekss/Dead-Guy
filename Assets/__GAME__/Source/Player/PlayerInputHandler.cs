using UnityEngine;


namespace __GAME__.Source.Player
{
    public class PlayerInputHandler: MonoBehaviour
    {
        public InputSystem_Actions input;

        private Vector2 _direction;
        public Vector3 moveDirection { get; private set; }

        private void Awake()
        {
            input = new InputSystem_Actions();
        }

        public void ReadInput()
        {
            #region Movement
            _direction = input.Player.Move.ReadValue<Vector2>();
            moveDirection =
                transform.right * _direction.x +
                transform.forward * _direction.y;
            #endregion
        }
    }
}