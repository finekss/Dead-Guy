using System.Collections.Generic;
using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Weapon.Projectiles
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }

        [SerializeField] private int _initialPoolSize = 20;

        private readonly Dictionary<int, Queue<GameObject>> _pools = new();
        private readonly Dictionary<int, GameObject> _prefabMap = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public GameObject Get(GameObject prefab)
        {
            int id = prefab.GetInstanceID();

            if (!_pools.ContainsKey(id))
            {
                _pools[id] = new Queue<GameObject>();
                _prefabMap[id] = prefab;
                Prewarm(prefab, _initialPoolSize);
            }

            if (_pools[id].Count > 0)
            {
                var obj = _pools[id].Dequeue();
                obj.SetActive(true);
                return obj;
            }

            var newObj = Instantiate(prefab, transform);
            newObj.SetActive(true);
            return newObj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);

            foreach (var kvp in _pools)
            {
                kvp.Value.Enqueue(obj);
                return;
            }

            Destroy(obj);
        }

        private void Prewarm(GameObject prefab, int count)
        {
            int id = prefab.GetInstanceID();
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                _pools[id].Enqueue(obj);
            }
        }
    }
}
