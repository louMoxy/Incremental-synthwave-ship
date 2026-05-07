using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [SerializeField] Slider HealthSlider;
    [SerializeField] Health playerHealth;

    public void UpdateHealthBar(int health)
    {
        HealthSlider.value = health;
    }

    private void Start()
    {
        HealthSlider.maxValue = playerHealth.GetComponent<Health>().GetTotalHealth();
        UpdateHealthBar(playerHealth.GetComponent<Health>().GetCurHealth());
    }
}
