using UnityEngine;

public class RotatableHouse : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float correctZRotation = 0f;
    public float rotationStep = 90f;

    [Header("State")]
    public bool isLocked = false;
    public bool requiresUnlockSwitch = false;
    public bool isUnlocked = true;

    [Header("Optional Visuals")]
    public GameObject fixedIndicator;

    private TownOrientationManager townManager;

    private void Start()
    {
        townManager = FindFirstObjectByType<TownOrientationManager>();

        if (requiresUnlockSwitch)
        {
            isUnlocked = false;
        }

        if (fixedIndicator != null)
        {
            fixedIndicator.SetActive(false);
        }
    }

    public void RotateHouse()
    {
        if (isLocked)
            return;

        if (!isUnlocked)
        {
            Debug.Log(gameObject.name + " is locked. Brother must activate a switch first.");
            return;
        }

        transform.Rotate(0f, 0f, rotationStep);

        CheckCorrectRotation();
    }

    public void UnlockHouse()
    {
        isUnlocked = true;
        Debug.Log(gameObject.name + " is now unlocked.");
    }

    private void CheckCorrectRotation()
    {
        float currentZ = NormalizeAngle(transform.eulerAngles.z);
        float correctZ = NormalizeAngle(correctZRotation);

        if (Mathf.Abs(Mathf.DeltaAngle(currentZ, correctZ)) < 1f)
        {
            isLocked = true;

            if (fixedIndicator != null)
            {
                fixedIndicator.SetActive(true);
            }

            Debug.Log(gameObject.name + " fixed!");

            if (townManager != null)
            {
                townManager.HouseFixed();
            }
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle < 0)
            angle += 360f;

        return angle;
    }
}