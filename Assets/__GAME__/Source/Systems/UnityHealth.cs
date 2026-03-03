using UnityEngine;

namespace __GAME__.Source.Systems
{
    public class UnityHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private int currentHealth;
        [SerializeField] private bool isDead = false;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0) isDead = true;
        }

        public void AddHealth(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        public void AddMaxHealth(int amount)
        {
            currentHealth += amount;
        }

        public int CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
    }
}
