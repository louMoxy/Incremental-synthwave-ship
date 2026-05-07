using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerEvents : MonoBehaviour
{
    public MMF_Player DamageFeedback;

    public void TakeDamage()
    {
        FindFirstObjectByType<UIUpdater>().UpdateHealthBar(GetComponent<Health>().GetCurHealth());

        DamageFeedback.PlayFeedbacks();
    }
}
