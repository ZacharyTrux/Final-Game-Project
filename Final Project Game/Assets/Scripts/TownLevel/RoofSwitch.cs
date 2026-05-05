using UnityEngine;
using UnityEngine.InputSystem;

public class RoofSwitch : MonoBehaviour
{
    public RotatableHouse houseToUnlock;

    private bool playerInRange = false;
    private bool used = false;

    private void Update()
    {
        if (used)
            return;

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (houseToUnlock != null)
            {
                houseToUnlock.UnlockHouse();
                used = true;
                Debug.Log("Roof switch activated.");
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