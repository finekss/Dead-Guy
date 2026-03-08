using __GAME__.Source.Game.Gameplay.Systems;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class UnityCharacter : MonoBehaviour, IControllable
    {
        private UnityHealth _health;
        
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 7f;

        public void Move(Vector2 direction, bool isFast = false)
        {   
            Vector3 move = new Vector3(direction.x, 0, direction.y).normalized;
            float speed = isFast ? runSpeed : walkSpeed;
        
            transform.Translate(move * (speed * Time.fixedDeltaTime), Space.Self);
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }
    }
}
