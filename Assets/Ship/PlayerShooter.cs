using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    Shooter shooter;
    InputAction fireAction;

    private void Awake()
    {
        shooter = GetComponent<Shooter>();
        fireAction = InputSystem.actions.FindAction("Fire");
    }

    void FireShooter()
    {
        shooter.isFiring = fireAction.IsPressed();
    } 
}
