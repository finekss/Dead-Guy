using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class CharacterModel
    {
        public float WalkSpeed { get; } = 5f;

        public float RunSpeed { get; } = 10f;

        public Vector3 MoveDirection(Vector2 direction)
        {   
            return new Vector3(direction.x, 0, direction.y).normalized;
        }
    }
}
