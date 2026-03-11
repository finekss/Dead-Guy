using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class CharacterView: MonoBehaviour
    {
        [SerializeField] private Transform _weaponPivot;

        public Transform WeaponPivot => _weaponPivot;

        public void OnAttack()
        {
            
        }
        
        public void OnMove(Vector3 move, float speed)
        {   
            transform.Translate(move * (speed * Time.fixedDeltaTime), Space.Self);
        }
    }
}