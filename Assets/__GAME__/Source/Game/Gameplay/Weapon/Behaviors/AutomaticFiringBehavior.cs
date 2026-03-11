using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public class AutomaticFiringBehavior : IFiringBehavior
    {
        private float _lastFireTime;
        private bool _isFiring;

        public void OnAttackPressed(WeaponPresenter presenter)
        {
            _isFiring = true;
        }

        public void OnAttackReleased(WeaponPresenter presenter)
        {
            _isFiring = false;
        }

        public void Tick(WeaponPresenter presenter)
        {
            if (!_isFiring) return;
            if (!presenter.Model.HasAmmo) return;
            if (Time.time - _lastFireTime < 1f / presenter.Model.FireRate) return;

            _lastFireTime = Time.time;
            presenter.ExecuteShot();
        }

        public void Reset()
        {
            _lastFireTime = 0f;
            _isFiring = false;
        }
    }
}
