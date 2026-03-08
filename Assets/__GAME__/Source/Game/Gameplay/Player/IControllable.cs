using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public interface IControllable
    {
        void Move(Vector2 direction, bool isFast = false);
        void Attack();
    }
}