using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public class ChargedFiringBehavior : IFiringBehavior
    {
        private float _chargeStartTime;
        private bool _isCharging;

        public void OnAttackPressed(WeaponPresenter presenter)
        {
            if (!presenter.Model.HasAmmo) return;

            _isCharging = true;
            _chargeStartTime = Time.time;
            presenter.View.OnChargeStarted();
        }

        public void OnAttackReleased(WeaponPresenter presenter)
        {
            if (!_isCharging) return;

            _isCharging = false;
            float chargeTime = presenter.Model.Data.chargeTime;
            float elapsed = Time.time - _chargeStartTime;
            float chargePercent = Mathf.Clamp01(elapsed / chargeTime);

            presenter.ExecuteChargedShot(chargePercent);
            presenter.View.OnChargeReleased();
        }

        public void Tick(WeaponPresenter presenter)
        {
            if (!_isCharging) return;

            float chargeTime = presenter.Model.Data.chargeTime;
            float elapsed = Time.time - _chargeStartTime;
            float chargePercent = Mathf.Clamp01(elapsed / chargeTime);

            presenter.View.OnChargeUpdate(chargePercent);
        }

        public void Reset()
        {
            _isCharging = false;
            _chargeStartTime = 0f;
        }
    }
}
