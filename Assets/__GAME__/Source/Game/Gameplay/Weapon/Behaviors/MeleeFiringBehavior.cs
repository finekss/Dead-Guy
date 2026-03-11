using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public class MeleeFiringBehavior : IFiringBehavior
    {
        private float _lastSwingTime;

        public void OnAttackPressed(WeaponPresenter presenter)
        {
            if (Time.time - _lastSwingTime < 1f / presenter.Model.FireRate) return;

            _lastSwingTime = Time.time;
            presenter.ExecuteMeleeAttack();
        }

        public void OnAttackReleased(WeaponPresenter presenter) { }

        public void Tick(WeaponPresenter presenter) { }

        public void Reset()
        {
            _lastSwingTime = 0f;
        }
    }
}
