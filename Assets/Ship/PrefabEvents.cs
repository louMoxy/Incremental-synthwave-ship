using UnityEngine;
using MoreMountains.Feedbacks;

public class PrefabEvents : MonoBehaviour
{
    public MMF_Player DamageFeedback;
    public MMF_Player DeathFeedback;
    [SerializeField] bool isPlayer = false;

    public void TakeDamage()
    {
        if (isPlayer)
        {
            FindFirstObjectByType<UIUpdater>().UpdateHealthBar(GetComponent<Health>().GetCurHealth());
        }
        DamageFeedback?.PlayFeedbacks();
    }

    public void OnDeath(Vector3 pos)
    {
        GetComponent<AestroidMaterials>()?.GenerateMaterials(pos);
        DeathFeedback?.PlayFeedbacks();
    }
}
