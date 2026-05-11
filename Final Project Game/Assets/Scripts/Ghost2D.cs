using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ghost2DChase : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;

    [Header("Movement - Left / Right Only")]
    public float chaseSpeed = 3f;
    public float stopDistanceX = 0.25f;

    [Header("Damage Settings")]
    public int scorePenalty = 50;
    public float damageCooldown = 1.5f;

    [Header("Stomp Kill Settings")]
    public bool canBeStomped = true;
    public float stompHeightThreshold = 0.15f;
    public bool destroyOnStomp = true;

    [Header("Stun After Side Hit")]
    public float stunDuration = 2f;

    private Rigidbody rb;
    private Collider ghostCollider;

    private float damageTimer;
    private bool isStunned;
    private float stunTimer;

    private float fixedY;
    private float fixedZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ghostCollider = GetComponent<Collider>();

        rb.useGravity = false;
        rb.freezeRotation = true;

        // Lock physics movement so it cannot fly upward/downward or move in Z.
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void FixedUpdate()
    {
        damageTimer -= Time.fixedDeltaTime;

        KeepGhostOnLine();

        if (isStunned)
        {
            UpdateStun();
            return;
        }

        if (targetPlayer == null) return;

        ChasePlayerLeftRightOnly();
    }

    private void ChasePlayerLeftRightOnly()
    {
        float distanceX = targetPlayer.position.x - rb.position.x;

        if (Mathf.Abs(distanceX) <= stopDistanceX)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float directionX = Mathf.Sign(distanceX);

        Vector3 nextPosition = rb.position;
        nextPosition.x += directionX * chaseSpeed * Time.fixedDeltaTime;
        nextPosition.y = fixedY;
        nextPosition.z = fixedZ;

        rb.MovePosition(nextPosition);
    }

    private void KeepGhostOnLine()
    {
        Vector3 pos = rb.position;
        pos.y = fixedY;
        pos.z = fixedZ;
        rb.position = pos;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void UpdateStun()
    {
        stunTimer -= Time.fixedDeltaTime;

        rb.linearVelocity = Vector3.zero;

        if (stunTimer <= 0f)
        {
            isStunned = false;
            Debug.Log("2D Ghost finished stun. Chasing again.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandlePlayerTouch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandlePlayerTouch(other);
    }

    private void HandlePlayerTouch(Collider other)
    {
        if (!other.CompareTag("2DPlayer")) return;

        // Check stomp FIRST.
        // If player is above ghost, kill ghost immediately.
        if (canBeStomped && IsPlayerStompingGhost(other))
        {
            KillGhost();
            return;
        }

        // If not stomp, then it is side/body hit.
        DamagePlayerAndStunGhost();
    }

    private bool IsPlayerStompingGhost(Collider playerCollider)
    {
        if (ghostCollider == null) return false;

        Bounds playerBounds = playerCollider.bounds;
        Bounds ghostBounds = ghostCollider.bounds;

        // Player must be above the ghost center.
        bool playerIsAboveGhost = playerBounds.center.y > ghostBounds.center.y;

        // Player feet should be near the top half/top area of ghost.
        bool playerFeetNearGhostTop =
            playerBounds.min.y >= ghostBounds.center.y;

        // Optional downward check.
        // If player has Rigidbody and is falling, this helps detect stomp.
        bool playerMovingDown = true;

        Rigidbody playerRb = playerCollider.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerMovingDown = playerRb.linearVelocity.y <= 0.2f;
        }

        return playerIsAboveGhost && playerFeetNearGhostTop && playerMovingDown;
    }

    private void DamagePlayerAndStunGhost()
    {
        if (damageTimer > 0f) return;

        Debug.Log("2D Player hit ghost from side. Score -50. Ghost stunned.");

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.TakeDamage();
        }

        DecreaseScore(scorePenalty);
        StunGhost();

        damageTimer = damageCooldown;
    }

    private void StunGhost()
    {
        isStunned = true;
        stunTimer = stunDuration;
        rb.linearVelocity = Vector3.zero;
    }

    private void KillGhost()
    {
        Debug.Log("2D Player jumped on ghost. Ghost killed.");

        // Stop ghost from triggering score loss again before it disappears.
        if (ghostCollider != null)
        {
            ghostCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject);
    }

    private void DecreaseScore(int amount)
    {
        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.AddScore(-amount);
        }
        else
        {
            Debug.LogWarning("Could not find GameObject named ScoringSystem.");
        }
    }
}