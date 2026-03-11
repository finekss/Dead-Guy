using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Pickup
{
    [RequireComponent(typeof(Collider))]
    public class WorldWeapon : MonoBehaviour
    {
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private float _rotationSpeed = 50f;
        [SerializeField] private float _bobSpeed = 1f;
        [SerializeField] private float _bobHeight = 0.15f;

        private Vector3 _startPosition;
        private bool _isPickedUp;

        public WeaponData WeaponData => _weaponData;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (_isPickedUp) return;

            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

            float newY = _startPosition.y + Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        public WeaponData PickUp()
        {
            _isPickedUp = true;
            gameObject.SetActive(false);
            return _weaponData;
        }

        public void SetWeaponData(WeaponData data)
        {
            _weaponData = data;
        }

        public static WorldWeapon SpawnInWorld(WeaponData data, Vector3 position)
        {
            if (data.worldDropPrefab == null) return null;

            var dropObject = Instantiate(data.worldDropPrefab, position, Quaternion.identity);
            var worldWeapon = dropObject.GetComponent<WorldWeapon>();

            if (worldWeapon == null)
            {
                worldWeapon = dropObject.AddComponent<WorldWeapon>();
            }

            worldWeapon.SetWeaponData(data);
            return worldWeapon;
        }
    }
}
