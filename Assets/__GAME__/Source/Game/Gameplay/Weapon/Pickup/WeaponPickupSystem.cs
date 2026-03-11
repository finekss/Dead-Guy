using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Pickup
{
    public class WeaponPickupSystem : MonoBehaviour
    {
        [SerializeField] private float _pickupRange = 3f;
        [SerializeField] private LayerMask _pickupMask;
        [SerializeField] private Camera _playerCamera;

        private WeaponInventory _inventory;
        private WorldWeapon _currentTarget;

        public WorldWeapon CurrentTarget => _currentTarget;

        public void Init(WeaponInventory inventory, Camera playerCamera)
        {
            _inventory = inventory;
            _playerCamera = playerCamera;
        }

        private void Update()
        {
            UpdateAimTarget();
        }

        public void TryPickup()
        {
            if (_currentTarget == null) return;

            WeaponData pickedUpData = _currentTarget.PickUp();
            WeaponData droppedData = _inventory.PickUpWeapon(pickedUpData);

            if (droppedData != null)
            {
                WorldWeapon.SpawnInWorld(droppedData, _currentTarget.transform.position);
            }

            Destroy(_currentTarget.gameObject);
            _currentTarget = null;
        }

        private void UpdateAimTarget()
        {
            if (_playerCamera == null) return;

            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _pickupRange, _pickupMask))
            {
                _currentTarget = hit.collider.GetComponent<WorldWeapon>();
            }
            else
            {
                _currentTarget = null;
            }
        }
    }
}
