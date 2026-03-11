using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Shotgun,
        Melee,
        Bow,
        Laser,
        Slingshot
    }

    public enum FiringMode
    {
        SemiAuto,
        Automatic,
        Charged,
        Melee,
        Beam
    }

    public enum DamageType
    {
        Projectile,
        Hitscan,
        Melee,
        Beam
    }

    [CreateAssetMenu(menuName = "Game/Weapon")]
    public class WeaponData : ScriptableObject
    {
        #region General

        public string weaponName;
        public string weaponID;
        public WeaponType weaponType;
        public Sprite weaponIcon;

        #endregion

        #region Prefabs

        public GameObject weaponPrefab;
        public GameObject projectilePrefab;
        public GameObject worldDropPrefab;

        #endregion

        #region Combat

        public FiringMode firingMode;
        public DamageType damageType;
        public int damage;
        public float fireRate;

        #endregion

        #region Ammo

        public int maxAmmo;
        public float reloadTime;
        public bool infiniteAmmo;

        #endregion

        #region Projectile

        public float projectileSpeed = 30f;
        public float projectileLifetime = 3f;

        #endregion

        #region Hitscan

        public float hitscanRange = 100f;

        #endregion

        #region Charged

        public float chargeTime = 1f;
        public float chargeDamageMultiplier = 2f;

        #endregion

        #region Melee

        public float meleeRange = 2f;
        public float meleeAngle = 90f;

        #endregion

        #region Beam

        public float beamRange = 50f;
        public float beamDamagePerSecond = 10f;

        #endregion

        #region Spread

        public float spreadAngle;
        public int pelletsPerShot = 1;

        #endregion
    }
}