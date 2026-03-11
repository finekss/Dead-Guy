using UnityEngine;
using __GAME__.Source.Game.Gameplay.Weapon;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] private Transform _weaponPivot;
        [SerializeField] private LayerMask _hitMask;

        public Transform WeaponPivot => _weaponPivot;
        public LayerMask HitMask => _hitMask;
    }
}