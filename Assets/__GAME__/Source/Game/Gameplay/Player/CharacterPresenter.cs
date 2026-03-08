using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class CharacterPresenter: IControllable
    {
        private CharacterView _view;
        private CharacterModel _model;

        public CharacterPresenter(CharacterView characterView, CharacterModel characterModel)
        {
            _view = characterView;
            _model = characterModel;
        }
        public void Move(Vector2 direction, bool isFast = false)
        {
            float runSpeed = _model.RunSpeed;
            float walkSpeed = _model.WalkSpeed;
            
            Vector3 move = _model.MoveDirection(direction);
            float speed = isFast ? runSpeed : walkSpeed;
            
            _view.OnMove(move,speed);
        }

        public void Attack()
        {
            _model.Attack();
            _view.OnAttack();
        }
    }
}