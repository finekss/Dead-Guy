using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public class SemiAutoFiringBehavior : IFiringBehavior
    {
        private float _lastFireTime;

        public void OnAttackPressed(WeaponPresenter presenter)
        {
            if (Time.time - _lastFireTime < 1f / presenter.Model.FireRate) return;
            if (!presenter.Model.HasAmmo) return;

            _lastFireTime = Time.time;
            presenter.ExecuteShot();
        }

        public void OnAttackReleased(WeaponPresenter presenter) { }

        public void Tick(WeaponPresenter presenter) { }

        public void Reset()
        {
            _lastFireTime = 0f;
        }
    }
}
