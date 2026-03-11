using UnityEngine;
using __GAME__.Source.Game.Gameplay.Weapon;
using __GAME__.Source.Game.Gameplay.Weapon.Pickup;

namespace __GAME__.Source.Game.Gameplay.Player
{
    public class CharacterPresenter: IControllable
    {
        private readonly CharacterView _view;
        private readonly CharacterModel _model;
        private readonly WeaponInventory _weaponInventory;
        private readonly WeaponPickupSystem _pickupSystem;

        public CharacterPresenter(
            CharacterView characterView,
            CharacterModel characterModel,
            WeaponInventory weaponInventory,
            WeaponPickupSystem pickupSystem)
        {
            _view = characterView;
            _model = characterModel;
            _weaponInventory = weaponInventory;
            _pickupSystem = pickupSystem;
        }

        public void Move(Vector2 direction, bool isFast = false)
        {
            float runSpeed = _model.RunSpeed;
            float walkSpeed = _model.WalkSpeed;
            
            Vector3 move = _model.MoveDirection(direction);
            float speed = isFast ? runSpeed : walkSpeed;
            
            _view.OnMove(move, speed);
        }

        public void StartAttack()
        {
            if (_weaponInventory.HasWeapon())
            {
                _weaponInventory.ActiveWeapon.StartAttack();
            }

            _view.OnAttack();
        }

        public void StopAttack()
        {
            if (_weaponInventory.HasWeapon())
            {
                _weaponInventory.ActiveWeapon.StopAttack();
            }
        }

        public void Interact()
        {
            _pickupSystem.TryPickup();
        }

        public void Reload()
        {
            if (_weaponInventory.HasWeapon())
            {
                _weaponInventory.ActiveWeapon.Reload();
            }
        }

        public void SwitchWeapon()
        {
            _weaponInventory.SwitchWeapon();
        }
    }
}