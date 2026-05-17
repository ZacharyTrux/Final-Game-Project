using UnityEngine;

public class MovingFloatingStair : MonoBehaviour
{
    [Header("Movement Around Starting Position")]
    [SerializeField] private float horizontalRangeX = 5f;
    [SerializeField] private float horizontalRangeZ = 0f;

    [Header("Speed")]
    [SerializeField] private float moveSpeedX = 1f;
    [SerializeField] private float moveSpeedZ = 0.7f;

    [Header("Floating Up/Down")]
    [SerializeField] private float verticalRange = 0.7f;
    [SerializeField] private float verticalSpeed = 1f;

    [Header("Random Offset")]
    [SerializeField] private float startOffset = 0f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        if (startOffset == 0f)
        {
            startOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float xOffset = Mathf.Sin((Time.time + startOffset) * moveSpeedX) * horizontalRangeX;
        float zOffset = Mathf.Cos((Time.time + startOffset) * moveSpeedZ) * horizontalRangeZ;
        float yOffset = Mathf.Sin((Time.time + startOffset) * verticalSpeed) * verticalRange;

        transform.position = startPosition + new Vector3(xOffset, yOffset, zOffset);
    }
}