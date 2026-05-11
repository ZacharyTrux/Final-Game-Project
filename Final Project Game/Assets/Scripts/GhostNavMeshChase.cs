using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class GhostTopDownNavMeshChase : MonoBehaviour
{
    [Header("Target")]
    public Transform topDownPlayer;
    public string topDownPlayerTag = "TopDownPlayer";
    public string player2DTag = "2DPlayer";

    [Header("Chase")]
    public float chaseSpeed = 3.5f;
    public float stoppingDistance = 1.2f;
    public float updatePathRate = 0.2f;

    [Header("Damage TopDown Player Only")]
    public int scorePenalty = 50;
    public float damageCooldown = 1.5f;

    [Header("2D Player Can Kill Ghost")]
    public bool canBeKilledBy2DPlayer = true;
    public float stompHeightOffset = 0.05f;

    [Header("Keep Original Rotation")]
    public bool keepStartingRotation = true;

    private NavMeshAgent agent;
    private Collider ghostCollider;
    private Quaternion startingRotation;

    private float pathTimer;
    private float damageTimer;
    private bool isDead = false;
    private DamageFlash damageFlash;

    private void Awake()
    {   
        agent = GetComponent<NavMeshAgent>();
        ghostCollider = GetComponent<Collider>();
        damageFlash = GetComponentInChildren<DamageFlash>();

        startingRotation = transform.rotation;

        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;

        // Keeps your ghost from rotating weirdly while chasing.
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        if (topDownPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(topDownPlayerTag);

            if (playerObj != null)
            {
                topDownPlayer = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Ghost cannot find TopDownPlayer. Check the player tag.");
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        damageTimer -= Time.deltaTime;

        if (keepStartingRotation)
        {
            transform.rotation = startingRotation;
        }

        if (topDownPlayer == null) return;
        if (!agent.isOnNavMesh) return;

        pathTimer -= Time.deltaTime;

        if (pathTimer <= 0f)
        {
            agent.SetDestination(topDownPlayer.position);
            pathTimer = updatePathRate;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTouch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTouch(other);
    }

    private void HandleTouch(Collider other)
    {
        if (isDead) return;

        // TopDown player touches ghost: lose score.
        if (other.CompareTag(topDownPlayerTag))
        {
            DamageTopDownPlayer();
            return;
        }

        // 2D player touches ghost: ONLY check if 2D player jumped on it.
        // If it is side touch, do nothing.
        if (other.CompareTag(player2DTag))
        {
            if (canBeKilledBy2DPlayer && Is2DPlayerStompingGhost(other))
            {
                KillGhost();
            }

            return;
        }
    }

    private bool Is2DPlayerStompingGhost(Collider playerCollider)
    {
        if (ghostCollider == null) return false;

        Bounds playerBounds = playerCollider.bounds;
        Bounds ghostBounds = ghostCollider.bounds;

        bool playerIsAboveGhost = playerBounds.center.y > ghostBounds.center.y;

        bool playerFeetAreHighEnough =
            playerBounds.min.y >= ghostBounds.center.y + stompHeightOffset;

        bool playerMovingDown = true;

        Rigidbody playerRb = playerCollider.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerMovingDown = playerRb.linearVelocity.y <= 0.2f;
        }

        return playerIsAboveGhost && playerFeetAreHighEnough && playerMovingDown;
    }

    private void DamageTopDownPlayer()
    {
        if (damageTimer > 0f) return;

        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.AddScore(-scorePenalty);
            Debug.Log("TopDown player hit by ghost. Score -" + scorePenalty);
        }
        else
        {
            Debug.LogWarning("ScoringManager.Instance not found.");
        }

        if (damageFlash != null)
        {
            Debug.Log("Calling ghost damage flash.");
            damageFlash.CallDamageFlash();
        }
        else
        {
            Debug.LogWarning("DamageFlash script not found on ghost.");
        }
        
        damageTimer = damageCooldown;

        
    }

    private void KillGhost()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("2D Player jumped on top-down ghost. Ghost killed.");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (ghostCollider != null)
        {
            ghostCollider.enabled = false;
        }

        Destroy(gameObject);
    }
}