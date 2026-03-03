using UnityEngine;

namespace __GAME__.Source.Weapon
{
    public enum WeaponCategory
    {
        Melee,
        Ranged
    }

    public enum TriggerType
    {
        OnPress,      // при нажатии
        OnRelease,    // при отпускании
        Held          // пока удерживается
    }

    public enum AttackMode
    {
        Single,       // один удар/выстрел
        Continuous,   // авто-огонь
        Charged       // зарядка с удержанием
    }

    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("General")]
        public string weaponName;
        public WeaponCategory category;

        [Header("Trigger Behaviour")]
        public TriggerType triggerType;
        public AttackMode attackMode;

        [Header("Damage")]
        public float damage = 10f;
        public float range = 2f;             // melee радиус или дальность hitscan
        public float attackRate = 1f;        // атак в секунду

        [Header("Charge Settings")]
        public bool canCharge;
        public float maxChargeTime = 2f;
        public AnimationCurve chargeMultiplier = AnimationCurve.Linear(0, 1, 1, 2);

        [Header("Ammo (для ranged)")]
        public bool usesAmmo;
        public int magazineSize = 30;
        public float reloadTime = 2f;

        [Header("Projectile (опционально)")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 50f;
    }
}