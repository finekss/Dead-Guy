using System;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public class WeaponInventory
    {
        private const int MaxSlots = 2;

        private readonly WeaponPresenter[] _weapons = new WeaponPresenter[MaxSlots];
        private readonly Transform _weaponPivot;
        private readonly LayerMask _hitMask;

        private int _activeIndex;

        public IWeapon ActiveWeapon => _weapons[_activeIndex];
        public int ActiveIndex => _activeIndex;
        public event Action<IWeapon> OnWeaponChanged;

        public WeaponInventory(Transform weaponPivot, LayerMask hitMask)
        {
            _weaponPivot = weaponPivot;
            _hitMask = hitMask;
            _activeIndex = 0;
        }

        #region Pickup / Drop

        public WeaponData PickUpWeapon(WeaponData data)
        {
            WeaponData droppedData = null;

            int emptySlot = FindEmptySlot();
            if (emptySlot >= 0)
            {
                CreateWeaponInSlot(emptySlot, data);
                SwitchToSlot(emptySlot);
            }
            else
            {
                droppedData = _weapons[_activeIndex].Data;
                DestroyWeaponInSlot(_activeIndex);
                CreateWeaponInSlot(_activeIndex, data);
                EquipSlot(_activeIndex);
            }

            return droppedData;
        }

        public WeaponData DropActiveWeapon()
        {
            if (_weapons[_activeIndex] == null) return null;

            WeaponData data = _weapons[_activeIndex].Data;
            DestroyWeaponInSlot(_activeIndex);

            int otherSlot = (_activeIndex + 1) % MaxSlots;
            if (_weapons[otherSlot] != null)
            {
                SwitchToSlot(otherSlot);
            }
            else
            {
                OnWeaponChanged?.Invoke(null);
            }

            return data;
        }

        #endregion

        #region Switch

        public void SwitchWeapon()
        {
            int nextIndex = (_activeIndex + 1) % MaxSlots;
            if (_weapons[nextIndex] == null) return;

            SwitchToSlot(nextIndex);
        }

        private void SwitchToSlot(int index)
        {
            if (_weapons[_activeIndex] != null)
                _weapons[_activeIndex].Unequip();

            _activeIndex = index;
            EquipSlot(index);
        }

        private void EquipSlot(int index)
        {
            if (_weapons[index] == null) return;

            _weapons[index].Equip(_weaponPivot);
            OnWeaponChanged?.Invoke(_weapons[index]);
        }

        #endregion

        #region Internal

        private void CreateWeaponInSlot(int index, WeaponData data)
        {
            var weaponObject = UnityEngine.Object.Instantiate(data.weaponPrefab, _weaponPivot);
            var view = weaponObject.GetComponent<WeaponView>();

            if (view == null)
            {
                view = weaponObject.AddComponent<WeaponView>();
            }

            var model = new WeaponModel(data);
            var presenter = new WeaponPresenter(model, view, _hitMask);

            _weapons[index] = presenter;
        }

        private void DestroyWeaponInSlot(int index)
        {
            if (_weapons[index] == null) return;

            _weapons[index].Dispose();
            _weapons[index] = null;
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_weapons[i] == null) return i;
            }
            return -1;
        }

        #endregion

        public bool HasWeapon()
        {
            return _weapons[_activeIndex] != null;
        }

        public WeaponData GetWeaponDataAt(int index)
        {
            if (index < 0 || index >= MaxSlots) return null;
            return _weapons[index]?.Data;
        }
    }
}
