using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public class WeaponModel
    {
        #region Properties

        public WeaponData Data { get; private set; }
        public int CurrentAmmo { get; private set; }
        public bool IsReloading { get; private set; }
        public float ReloadEndTime { get; private set; }
        public float FireRate => Data.fireRate;
        public bool HasAmmo => Data.infiniteAmmo || CurrentAmmo > 0;

        #endregion

        public WeaponModel(WeaponData data)
        {
            Data = data;
            CurrentAmmo = data.maxAmmo;
        }

        #region Ammo

        public void ConsumeAmmo(int amount = 1)
        {
            if (Data.infiniteAmmo) return;
            CurrentAmmo = Mathf.Max(0, CurrentAmmo - amount);
        }

        public void StartReload()
        {
            if (IsReloading) return;
            if (CurrentAmmo >= Data.maxAmmo) return;

            IsReloading = true;
            ReloadEndTime = Time.time + Data.reloadTime;
        }

        public bool TryFinishReload()
        {
            if (!IsReloading) return false;
            if (Time.time < ReloadEndTime) return false;

            CurrentAmmo = Data.maxAmmo;
            IsReloading = false;
            return true;
        }

        public void CancelReload()
        {
            IsReloading = false;
        }

        #endregion

        #region Damage

        public int CalculateDamage(float chargePercent = 1f)
        {
            float multiplier = Data.firingMode == FiringMode.Charged
                ? Mathf.Lerp(1f, Data.chargeDamageMultiplier, chargePercent)
                : 1f;

            return Mathf.RoundToInt(Data.damage * multiplier);
        }

        #endregion
    }
}
