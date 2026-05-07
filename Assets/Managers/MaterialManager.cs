using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MaterialType
{
    Iron,
    Coal,
    Gold
}

public class MaterialManager : MonoBehaviour
{
    Dictionary<MaterialType, int> materials = new Dictionary<MaterialType, int>();
    public static MaterialManager Instance;
    [SerializeField] private TextMeshProUGUI coalText;
    [SerializeField] private TextMeshProUGUI ironText;

    ThreatManager threatManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize all materials to 0
        foreach (MaterialType type in System.Enum.GetValues(typeof(MaterialType)))
        {
            materials[type] = 0;
        }
    }

    private void Start()
    {
        UpdateUI();
        threatManager = FindFirstObjectByType<ThreatManager>();
    }

    public int GetAmount(MaterialType type)
    {
        return materials[type];
    }

    public void Add(MaterialType type, int amount)
    {
        materials[type] += amount;
        UpdateUI();
        threatManager.OnMaterialCollected(type, amount);
    }


    public bool Remove(MaterialType type, int amount)
    {
        if (materials[type] < amount)
        {
            return false;
        }

        materials[type] -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        coalText.SetText(materials[MaterialType.Coal].ToString());
        ironText.SetText(materials[MaterialType.Iron].ToString());
    }
}