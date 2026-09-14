using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public UnityEvent<float, float> OnHealthChanged; // current, max
    public UnityEvent OnPlayerDeath;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void InstantKill()
    {
        currentHealth = 0f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        OnPlayerDeath?.Invoke();
    }
}