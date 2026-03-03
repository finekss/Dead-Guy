using __GAME__.Source.Systems;
using UnityEngine;

namespace __GAME__.Source.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(UnityHealth))]

    public class UnityPlayer : MonoBehaviour
    {
        public static UnityPlayer Player;

        private Rigidbody _rb;
        private PlayerInputHandler _inputHandler;
        private UnityHealth _unityHealth;
        
        [SerializeField] private float moveSpeed = 5f;

        private void Awake()
        {
            Player = this;

            _rb = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<PlayerInputHandler>();
            _unityHealth = GetComponent<UnityHealth>();

        }

        private void OnEnable()
        {
            _inputHandler.input.Enable();
        }

        private void OnDisable()
        {
            _inputHandler.input.Disable();
        }

        private void Update()
        {
            _inputHandler.ReadInput();
            CheackState();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            Vector3 moveDir = _inputHandler.moveDirection;
            
            Vector3 velocity = moveDir.normalized * moveSpeed;
            _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
        }

        private void Dead()
        {
            gameObject.SetActive(false);
        }

        private void CheackState()
        {
            if (_unityHealth.IsDead) Dead();
        }
    }
}
