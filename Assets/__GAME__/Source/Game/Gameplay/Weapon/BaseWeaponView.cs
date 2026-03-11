using System;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Transform _muzzlePoint;
        [SerializeField] private Transform _meleeHitPoint;
        [SerializeField] private LineRenderer _beamRenderer;
        [SerializeField] private ParticleSystem _muzzleFlash;

        public Transform MuzzlePoint => _muzzlePoint;
        public event Action OnTickRequested;

        #region Lifecycle

        private void Update()
        {
            OnTickRequested?.Invoke();
        }

        #endregion

        #region Equip

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        #endregion

        #region Projectile

        public void SpawnProjectile(GameObject prefab, int damage, float speed, float lifetime, float spreadAngle)
        {
            if (_muzzlePoint == null) return;

            Vector3 direction = _muzzlePoint.forward;
            if (spreadAngle > 0f)
            {
                direction = ApplySpread(direction, spreadAngle);
            }

            var projectileObject = Instantiate(prefab, _muzzlePoint.position, Quaternion.LookRotation(direction));
            var projectile = projectileObject.GetComponent<Projectiles.Projectile>();
            if (projectile != null)
            {
                projectile.Init(damage, speed, lifetime, direction);
            }

            PlayMuzzleFlash();
        }

        public void SpawnProjectilesSpread(GameObject prefab, int damage, float speed, float lifetime, float spreadAngle, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnProjectile(prefab, damage, speed, lifetime, spreadAngle);
            }
        }

        #endregion

        #region Hitscan

        public void PerformHitscan(float range, int damage, LayerMask hitMask)
        {
            if (_muzzlePoint == null) return;

            PlayMuzzleFlash();

            if (Physics.Raycast(_muzzlePoint.position, _muzzlePoint.forward, out RaycastHit hit, range, hitMask))
            {
                var damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }

        #endregion

        #region Melee

        public void PerformMeleeOverlap(float range, float angle, int damage, LayerMask hitMask)
        {
            Transform origin = _meleeHitPoint != null ? _meleeHitPoint : transform;
            Collider[] hits = Physics.OverlapSphere(origin.position, range, hitMask);

            foreach (var hit in hits)
            {
                Vector3 dirToTarget = (hit.transform.position - origin.position).normalized;
                float angleToTarget = Vector3.Angle(origin.forward, dirToTarget);

                if (angleToTarget <= angle * 0.5f)
                {
                    var damageable = hit.GetComponent<IDamageable>();
                    damageable?.TakeDamage(damage);
                }
            }
        }

        #endregion

        #region Beam

        public void ToggleBeam(bool active)
        {
            if (_beamRenderer == null) return;
            _beamRenderer.enabled = active;
        }

        public void UpdateBeam(float range)
        {
            if (_beamRenderer == null || _muzzlePoint == null) return;

            Vector3 endPoint = _muzzlePoint.position + _muzzlePoint.forward * range;

            if (Physics.Raycast(_muzzlePoint.position, _muzzlePoint.forward, out RaycastHit hit, range))
            {
                endPoint = hit.point;
            }

            _beamRenderer.SetPosition(0, _muzzlePoint.position);
            _beamRenderer.SetPosition(1, endPoint);
        }

        public void PerformBeamDamage(float range, int damage, LayerMask hitMask)
        {
            if (_muzzlePoint == null) return;

            if (Physics.Raycast(_muzzlePoint.position, _muzzlePoint.forward, out RaycastHit hit, range, hitMask))
            {
                var damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }

        #endregion

        #region Charge

        public void OnChargeStarted() { }

        public void OnChargeUpdate(float percent) { }

        public void OnChargeReleased() { }

        #endregion

        #region Effects

        private void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
        }

        private Vector3 ApplySpread(Vector3 direction, float angle)
        {
            float halfAngle = angle * 0.5f;
            Vector3 spread = UnityEngine.Random.insideUnitSphere * Mathf.Tan(halfAngle * Mathf.Deg2Rad);
            return (direction + spread).normalized;
        }

        #endregion
    }
}
