using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0f)
            currentHealth = 0f;

        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = "Health: " + Mathf.RoundToInt(currentHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");

        if (MessageWindow.Instance != null)
            MessageWindow.Instance.ShowPickup("You died!");

        // For now: freeze the game on death
        Time.timeScale = 0f;
    }
}
