using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject ship;
    [SerializeField] CinemachineCamera cm;

    public static GameManager Instance;

    GameObject playerPrefab;

    private void Awake()
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

        playerPrefab = Instantiate(ship);

    }

    private void Start()
    {
        cm.Target.TrackingTarget = playerPrefab.transform;
        cm.Target.CustomLookAtTarget = false; 
    }
}
