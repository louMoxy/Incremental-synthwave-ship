using UnityEngine;

public class ThreatManager : MonoBehaviour
{
    [Header("Threat")]
    [SerializeField] float maxThreat = 100f;
    [SerializeField] float passiveTickRate = 0.5f; // per second

    [Header("Material Threat Values")]
    [SerializeField] float coalThreat = 2f;
    [SerializeField] float ironThreat = 4f;
    [SerializeField] float goldThreat = 8f;

    float threatLevel;

    public float ThreatLevel => threatLevel;
    public float ThreatNormalised => threatLevel / maxThreat; // 0–1, for UI and Music

    void Update()
    {
        threatLevel += passiveTickRate * Time.deltaTime;
        threatLevel = Mathf.Clamp(threatLevel, 0f, maxThreat);
    }

    public void OnMaterialCollected(MaterialType material, int amount)
    {
        float spike = material switch
        {
            MaterialType.Coal => coalThreat * amount,
            MaterialType.Iron => ironThreat * amount,
            MaterialType.Gold => goldThreat * amount,
            _ => 0f
        };

        threatLevel = Mathf.Clamp(threatLevel + spike, 0f, maxThreat);
    }

    public int ThreatBand
    {
        get
        {
            if (threatLevel < 25f) return 0;
            if (threatLevel < 50f) return 1;
            if (threatLevel < 75f) return 2;
            return 3;
        }
    }

    public void ResetThreat()
    {
        threatLevel = 0f;
    }
}