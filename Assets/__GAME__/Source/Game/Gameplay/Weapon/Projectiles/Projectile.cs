using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        private int _damage;
        private float _speed;
        private Vector3 _direction;
        private float _deathTime;
        private Rigidbody _rigidbody;
        private bool _initialized;

        public void Init(int damage, float speed, float lifetime, Vector3 direction)
        {
            _damage = damage;
            _speed = speed;
            _direction = direction;
            _deathTime = Time.time + lifetime;
            _initialized = true;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = _direction * _speed;
        }

        private void Update()
        {
            if (!_initialized) return;

            if (Time.time >= _deathTime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_initialized) return;

            var damageable = other.GetComponent<IDamageable>();
            damageable?.TakeDamage(_damage);

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _initialized = false;

            if (ProjectilePool.Instance != null)
            {
                ProjectilePool.Instance.Return(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
