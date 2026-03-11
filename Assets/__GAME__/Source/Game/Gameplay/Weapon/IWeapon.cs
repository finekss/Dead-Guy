using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public interface IWeapon
    {
        WeaponData Data { get; }
        bool IsEquipped { get; }
        bool CanAttack { get; }
        int CurrentAmmo { get; }

        void Equip(Transform weaponPivot);
        void Unequip();
        void StartAttack();
        void StopAttack();
        void Reload();
        void Tick();
    }
}