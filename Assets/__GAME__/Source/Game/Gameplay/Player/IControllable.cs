using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public interface IControllable
    {
        void Move(Vector2 direction, bool isFast = false);
        void StartAttack();
        void StopAttack();
        void Interact();
        void Reload();
        void SwitchWeapon();
    }
}