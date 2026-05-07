using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    [SerializeField] float maxFuel = 100f;
    [SerializeField] float drainPerSecondAtFullThrottle = 8f;
    [SerializeField] Slider fuelSlider;

    public float CurrentFuel { get; private set; }
    public float MaxFuel => maxFuel;

    void Awake()
    {
        CurrentFuel = maxFuel;

        if (fuelSlider)
        {
            fuelSlider.minValue = 0f;
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = CurrentFuel;
        }
    }

    public bool HasFuel => CurrentFuel > 0.001f;

    /// <summary>
    /// Drains fuel based on throttle (0..1) over deltaTime.
    /// Returns true if fuel was consumed (i.e. you still had fuel).
    /// </summary>
    public bool Consume(float throttle01, float deltaTime)
    {
        if (throttle01 <= 0f) return HasFuel;

        if (CurrentFuel <= 0f)
        {
            CurrentFuel = 0f;
            UpdateUI();
            return false;
        }

        float amount = drainPerSecondAtFullThrottle * throttle01 * deltaTime;
        CurrentFuel = Mathf.Max(0f, CurrentFuel - amount);

        UpdateUI();
        return CurrentFuel > 0f;
    }

    public void Refuel(float amount)
    {
        CurrentFuel = Mathf.Min(maxFuel, CurrentFuel + amount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (fuelSlider) fuelSlider.value = CurrentFuel;
    }
}