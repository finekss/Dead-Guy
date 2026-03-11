using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public class BeamFiringBehavior : IFiringBehavior
    {
        private bool _isFiring;
        private float _lastDamageTick;

        public void OnAttackPressed(WeaponPresenter presenter)
        {
            if (!presenter.Model.HasAmmo) return;

            _isFiring = true;
            presenter.View.ToggleBeam(true);
        }

        public void OnAttackReleased(WeaponPresenter presenter)
        {
            _isFiring = false;
            presenter.View.ToggleBeam(false);
        }

        public void Tick(WeaponPresenter presenter)
        {
            if (!_isFiring) return;
            if (!presenter.Model.HasAmmo)
            {
                _isFiring = false;
                presenter.View.ToggleBeam(false);
                return;
            }

            presenter.View.UpdateBeam(presenter.Model.Data.beamRange);

            if (Time.time - _lastDamageTick >= 0.1f)
            {
                _lastDamageTick = Time.time;
                presenter.ExecuteBeamDamage();
            }
        }

        public void Reset()
        {
            _isFiring = false;
            _lastDamageTick = 0f;
        }
    }
}
