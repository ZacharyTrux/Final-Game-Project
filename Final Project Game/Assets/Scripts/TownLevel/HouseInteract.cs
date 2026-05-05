using UnityEngine;
using UnityEngine.InputSystem;

public class HouseInteract : MonoBehaviour
{
    public RotatableHouse targetHouse;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (targetHouse != null)
            {
                targetHouse.RotateHouse();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}