using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ghost2DChase : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float hoverHeight = 1.2f;
    public float stopDistance = 0.25f;

    [Header("Annoy Player")]
    public float annoyDistance = 1.2f;
    public float verticalBobAmount = 0.35f;
    public float verticalBobSpeed = 4f;

    [Header("Z Movement Test")]
    public bool allowZMovement = true;
    public float zOffsetAmount = 2f;
    public float zMoveSpeed = 2f;

    [Header("Visual")]
    [Header("Visual")]
    public Transform visualModel;
    public float faceRightYRotation = -90f;
    public float faceLeftYRotation = 90f;
    public float turnSpeed = 180f; // degrees per second, lower = turn slower

    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float damageCooldown = 1.5f;

    private Rigidbody rb;
    private float damageTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (targetPlayer == null) return;

        Vector3 targetPosition = targetPlayer.position;

        // Ghost chases slightly above the player.
        targetPosition.y += hoverHeight;

        // Let the ghost move forward/backward on Z to test depth movement.
        if (allowZMovement)
        {
            targetPosition.z += Mathf.Sin(Time.time * zMoveSpeed) * zOffsetAmount;
        }
        else
        {
            targetPosition.z = transform.position.z;
        }

        Vector3 currentPosition = rb.position;
        Vector3 toTarget = targetPosition - currentPosition;

        float distance = toTarget.magnitude;

        if (distance > stopDistance)
        {
            Vector3 direction = toTarget.normalized;

            Vector3 nextPosition =
                currentPosition + direction * chaseSpeed * Time.fixedDeltaTime;

            // When close to player, bob up/down to feel annoying.
            if (distance < annoyDistance)
            {
                nextPosition.y += Mathf.Sin(Time.time * verticalBobSpeed)
                                * verticalBobAmount
                                * Time.fixedDeltaTime;
            }

            rb.MovePosition(nextPosition);
            RotateVisual(direction);
        }

        damageTimer -= Time.fixedDeltaTime;
    }

    private void RotateVisual(Vector3 direction)
    {
        if (visualModel == null) return;

        Quaternion targetRotation;

        if (direction.x > 0.05f)
        {
            // Ghost moving right
            targetRotation = Quaternion.Euler(0f, faceRightYRotation, 0f);
        }
        else if (direction.x < -0.05f)
        {
            // Ghost moving left
            targetRotation = Quaternion.Euler(0f, faceLeftYRotation, 0f);
        }
        else
        {
            return;
        }

        visualModel.localRotation = Quaternion.RotateTowards(
            visualModel.localRotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (damageTimer > 0f) return;

        if (other.CompareTag("Player") ||
            other.CompareTag("2DPlayer") ||
            other.CompareTag("TopDownPlayer"))
        {
            Debug.Log("Ghost touched player.");

            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.TakeDamage();
            }

            damageTimer = damageCooldown;
        }
    }
}