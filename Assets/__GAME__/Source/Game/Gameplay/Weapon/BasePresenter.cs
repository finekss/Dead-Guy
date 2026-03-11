using UnityEngine;
using __GAME__.Source.Game.Gameplay.Weapon.Behaviors;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public class WeaponPresenter : IWeapon
    {
        private readonly IFiringBehavior _firingBehavior;
        private readonly LayerMask _hitMask;

        public WeaponModel Model { get; }
        public WeaponView View { get; }
        public WeaponData Data => Model.Data;
        public bool IsEquipped { get; private set; }
        public bool CanAttack => Model.HasAmmo && !Model.IsReloading;
        public int CurrentAmmo => Model.CurrentAmmo;

        public WeaponPresenter(WeaponModel model, WeaponView view, LayerMask hitMask)
        {
            Model = model;
            View = view;
            _hitMask = hitMask;
            _firingBehavior = FiringBehaviorFactory.Create(model.Data.firingMode);

            View.OnTickRequested += Tick;
        }

        #region IWeapon

        public void Equip(Transform weaponPivot)
        {
            View.SetParent(weaponPivot);
            View.gameObject.SetActive(true);
            IsEquipped = true;
            _firingBehavior.Reset();
        }

        public void Unequip()
        {
            View.gameObject.SetActive(false);
            IsEquipped = false;
            _firingBehavior.Reset();
        }

        public void StartAttack()
        {
            if (Model.IsReloading) return;
            _firingBehavior.OnAttackPressed(this);
        }

        public void StopAttack()
        {
            _firingBehavior.OnAttackReleased(this);
        }

        public void Reload()
        {
            Model.StartReload();
        }

        public void Tick()
        {
            if (Model.TryFinishReload()) return;
            _firingBehavior.Tick(this);
        }

        #endregion

        #region Shot Execution

        public void ExecuteShot()
        {
            Model.ConsumeAmmo();

            switch (Model.Data.damageType)
            {
                case DamageType.Projectile:
                    int pellets = Model.Data.pelletsPerShot;
                    if (pellets > 1)
                    {
                        View.SpawnProjectilesSpread(
                            Model.Data.projectilePrefab,
                            Model.CalculateDamage(),
                            Model.Data.projectileSpeed,
                            Model.Data.projectileLifetime,
                            Model.Data.spreadAngle,
                            pellets);
                    }
                    else
                    {
                        View.SpawnProjectile(
                            Model.Data.projectilePrefab,
                            Model.CalculateDamage(),
                            Model.Data.projectileSpeed,
                            Model.Data.projectileLifetime,
                            Model.Data.spreadAngle);
                    }
                    break;

                case DamageType.Hitscan:
                    View.PerformHitscan(Model.Data.hitscanRange, Model.CalculateDamage(), _hitMask);
                    break;
            }
        }

        public void ExecuteChargedShot(float chargePercent)
        {
            Model.ConsumeAmmo();

            int damage = Model.CalculateDamage(chargePercent);

            switch (Model.Data.damageType)
            {
                case DamageType.Projectile:
                    View.SpawnProjectile(
                        Model.Data.projectilePrefab,
                        damage,
                        Model.Data.projectileSpeed * (1f + chargePercent),
                        Model.Data.projectileLifetime,
                        Model.Data.spreadAngle * (1f - chargePercent * 0.5f));
                    break;

                case DamageType.Hitscan:
                    View.PerformHitscan(Model.Data.hitscanRange, damage, _hitMask);
                    break;
            }
        }

        public void ExecuteMeleeAttack()
        {
            View.PerformMeleeOverlap(
                Model.Data.meleeRange,
                Model.Data.meleeAngle,
                Model.CalculateDamage(),
                _hitMask);
        }

        public void ExecuteBeamDamage()
        {
            float dps = Model.Data.beamDamagePerSecond;
            int tickDamage = Mathf.RoundToInt(dps * 0.1f);

            View.PerformBeamDamage(Model.Data.beamRange, tickDamage, _hitMask);
        }

        #endregion

        public void Dispose()
        {
            View.OnTickRequested -= Tick;
            _firingBehavior.Reset();

            if (View != null && View.gameObject != null)
            {
                Object.Destroy(View.gameObject);
            }
        }
    }
}
