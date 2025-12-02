using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNeeds : MonoBehaviour
{
    [Header("Max Values")]
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    [Header("Current Values (read-only in-game)")]
    public float hunger;
    public float thirst;

    [Header("Decay Per Second")]
    public float hungerDecayPerSecond = 0.5f;
    public float thirstDecayPerSecond = 0.75f;

    [Header("UI References")]
    public Slider hungerSlider;
    public Slider thirstSlider;
    public TMP_Text hungerText;
    public TMP_Text thirstText;

    [Header("Low Warning Threshold")]
    public float lowThreshold = 20f;

    void Start()
    {
        // Start full
        hunger = maxHunger;
        thirst = maxThirst;

        if (hungerSlider != null)
        {
            hungerSlider.minValue = 0f;
            hungerSlider.maxValue = maxHunger;
        }

        if (thirstSlider != null)
        {
            thirstSlider.minValue = 0f;
            thirstSlider.maxValue = maxThirst;
        }

        UpdateUI();
    }

    void Update()
    {
        // Decay needs over time
        hunger -= hungerDecayPerSecond * Time.deltaTime;
        thirst -= thirstDecayPerSecond * Time.deltaTime;

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        thirst = Mathf.Clamp(thirst, 0f, maxThirst);

        UpdateUI();

        // TODO: later – apply damage or death when hunger/thirst == 0
        // For now, just let it hit 0 and stay there.
    }

    void UpdateUI()
    {
        if (hungerSlider != null)
            hungerSlider.value = hunger;

        if (thirstSlider != null)
            thirstSlider.value = thirst;

        if (hungerText != null)
        {
            hungerText.text = $"Hunger: {(int)hunger}";
            hungerText.color = (hunger <= lowThreshold) ? Color.red : Color.white;
        }

        if (thirstText != null)
        {
            thirstText.text = $"Thirst: {(int)thirst}";
            thirstText.color = (thirst <= lowThreshold) ? Color.red : Color.white;
        }
    }

    public void Eat(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        UpdateUI();
    }

    public void Drink(float amount)
    {
        thirst = Mathf.Clamp(thirst + amount, 0f, maxThirst);
        UpdateUI();
    }

    public bool IsStarving()
    {
        return hunger <= 0f;
    }

    public bool IsDehydrated()
    {
        return thirst <= 0f;
    }
}
