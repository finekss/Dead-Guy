using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class CharacterView: MonoBehaviour
    {
        public void OnAttack()
        {
            
        }
        
        public void OnMove(Vector3 move, float speed)
        {   
            transform.Translate(move * (speed * Time.fixedDeltaTime), Space.Self);
        }
    }
}