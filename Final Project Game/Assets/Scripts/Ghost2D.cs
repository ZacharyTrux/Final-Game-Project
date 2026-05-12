using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ghost2DChase : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float patrolSpeed = 1.8f;
    public float stopDistanceX = 0.25f;

    [Header("Patrol Waypoints")]
    public Transform patrolLeftPoint;   // ← Assign in Inspector
    public Transform patrolRightPoint;  // ← Assign in Inspector

    [Header("Aggro Range")]
    public float aggroRangeX = 5f;      // Player must be within this X distance to trigger chase

    [Header("Damage Settings")]
    public int scorePenalty = 50;
    public float damageCooldown = 1.5f;

    [Header("Stomp Kill Settings")]
    public bool canBeStomped = true;
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

    private enum GhostState { Patrol, Chase }
    private GhostState currentState = GhostState.Patrol;

    private Transform currentPatrolTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ghostCollider = GetComponent<Collider>();

        rb.useGravity = false;
        rb.freezeRotation = true;

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

        // Start patrolling toward the right point first
        currentPatrolTarget = patrolRightPoint != null ? patrolRightPoint : patrolLeftPoint;
    }

    void FixedUpdate()
    {
        damageTimer -= Time.fixedDeltaTime;

        EnforceGhostLine();

        if (isStunned)
        {
            UpdateStun();
            return;
        }

        if (targetPlayer == null) return;

        DecideState();

        if (currentState == GhostState.Chase)
            ChasePlayer();
        else
            Patrol();
    }

    // ── State Decision ─────────────────────────────────────────────────────

    private void DecideState()
    {
        float distX = Mathf.Abs(targetPlayer.position.x - rb.position.x);

        if (distX <= aggroRangeX)
        {
            if (currentState != GhostState.Chase)
                Debug.Log("Ghost spotted player — switching to Chase.");

            currentState = GhostState.Chase;
        }
        else
        {
            if (currentState != GhostState.Patrol)
                Debug.Log("Player out of range — switching to Patrol.");

            currentState = GhostState.Patrol;
        }
    }

    // ── Chase ──────────────────────────────────────────────────────────────

    private void ChasePlayer()
    {
        float distanceX = targetPlayer.position.x - rb.position.x;

        if (Mathf.Abs(distanceX) <= stopDistanceX)
            return;

        MoveOnX(Mathf.Sign(distanceX), chaseSpeed);
    }

    // ── Patrol ─────────────────────────────────────────────────────────────

    private void Patrol()
    {
        if (currentPatrolTarget == null) return;

        float distanceX = currentPatrolTarget.position.x - rb.position.x;

        // Reached current waypoint — flip to the other one
        if (Mathf.Abs(distanceX) <= 0.15f)
        {
            FlipPatrolTarget();
            return;
        }

        MoveOnX(Mathf.Sign(distanceX), patrolSpeed);
    }

    private void FlipPatrolTarget()
    {
        if (currentPatrolTarget == patrolRightPoint)
            currentPatrolTarget = patrolLeftPoint;
        else
            currentPatrolTarget = patrolRightPoint;
    }

    // ── Movement ───────────────────────────────────────────────────────────

    private void MoveOnX(float direction, float speed)
    {
        Vector3 next = rb.position;
        next.x += direction * speed * Time.fixedDeltaTime;
        next.y = fixedY;
        next.z = fixedZ;
        rb.MovePosition(next);
    }

    private void EnforceGhostLine()
    {
        Vector3 pos = rb.position;
        pos.y = fixedY;
        pos.z = fixedZ;
        rb.position = pos;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // ── Stun ───────────────────────────────────────────────────────────────

    private void UpdateStun()
    {
        stunTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = Vector3.zero;

        if (stunTimer <= 0f)
        {
            isStunned = false;
            Debug.Log("Ghost stun finished. Resuming.");
        }
    }

    // ── Collision ──────────────────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)   { HandlePlayerTouch(other); }
    private void OnTriggerStay(Collider other)    { HandlePlayerTouch(other); }

    private void HandlePlayerTouch(Collider other)
    {
        if (!other.CompareTag("2DPlayer")) return;

        if (canBeStomped && IsPlayerStompingGhost(other))
        {
            KillGhost();
            return;
        }

        DamagePlayerAndStunGhost();
    }

    private bool IsPlayerStompingGhost(Collider playerCollider)
    {
        if (ghostCollider == null) return false;

        Bounds pb = playerCollider.bounds;
        Bounds gb = ghostCollider.bounds;

        bool above = pb.center.y > gb.center.y;
        bool feetHigh = pb.min.y >= gb.center.y;
        bool movingDown = true;

        Rigidbody playerRb = playerCollider.GetComponent<Rigidbody>();
        if (playerRb != null)
            movingDown = playerRb.linearVelocity.y <= 0.2f;

        return above && feetHigh && movingDown;
    }

    private void DamagePlayerAndStunGhost()
    {
        if (damageTimer > 0f) return;

        Debug.Log("2D Player hit ghost from side. Damage + Ghost stunned.");

        PlayerManager.Instance?.TakeDamage();
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
        Debug.Log("2D Player stomped ghost. Ghost killed.");

        if (ghostCollider != null) ghostCollider.enabled = false;
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.isKinematic = true; }

        Destroy(gameObject);
    }

    private void DecreaseScore(int amount)
    {
        if (ScoringManager.Instance != null)
            ScoringManager.Instance.AddScore(-amount);
        else
            Debug.LogWarning("ScoringManager not found.");
    }
}