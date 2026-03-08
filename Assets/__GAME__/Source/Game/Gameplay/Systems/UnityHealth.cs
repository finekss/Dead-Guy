using UnityEngine;

namespace __GAME__.Source.Game.Gameplay.Systems
{
    public class UnityHealth : MonoBehaviour
    {
        [SerializeField] private GameObject _entity;
        [SerializeField] private int maxHealth = 10;

        private int currentHealth;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public event System.Action<int, int> OnHealthChanged;
        public event System.Action OnDead;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0) Dead();
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Dead()
        {
            currentHealth = 0;
            OnDead?.Invoke();

            if (_entity != null)
                _entity.SetActive(false);
        }
    }
}