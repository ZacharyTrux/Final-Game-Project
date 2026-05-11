using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class GhostNavMeshChase : MonoBehaviour
{
    [Header("Visual")]
    public Transform visualModel;
    public float visualRotationOffsetZ = 180f;

    [Header("Target")]
    public Transform targetPlayer;

    [Header("Chase Settings")]
    public float chaseSpeed = 2f;
    public float updateRate = 0.2f;

    [Header("Plane Lock")]
    public bool lockZ = true;

    [Header("Hit Settings")]
    public int scorePenalty = 50;
    public float damageCooldown = 1.5f;

    [Header("Stun / Fade Settings")]
    public float stunDuration = 2f;
    [Range(0.1f, 1f)]
    public float stunnedAlpha = 0.35f;

    [Header("Fallback Patrol When Stuck")]
    public float patrolDistance = 2f;
    public float patrolPointSearchRadius = 2f;
    public float stuckCheckTime = 1.0f;
    public float stuckMoveThreshold = 0.05f;
    public float fallbackPatrolDuration = 3f;

    private NavMeshAgent agent;

    private Renderer[] visualRenderers;
    private Color[] originalColors;

    private float updateTimer;
    private float damageTimer;
    private float fixedZ;

    private bool isStunned;
    private float stunTimer;

    private Vector3 lastPosition;
    private float stuckTimer;

    private bool fallbackPatrolMode;
    private float fallbackTimer;
    private Vector3 patrolCenter;
    private Vector3 patrolLeftPoint;
    private Vector3 patrolRightPoint;
    private bool movingToRightPoint = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed;
        agent.stoppingDistance = 0f;

        // NavMesh calculates path, but this script controls visible transform.
        agent.updatePosition = false;
        agent.updateRotation = false;

        if (visualModel != null)
        {
            visualRenderers = visualModel.GetComponentsInChildren<Renderer>();
        }
        else
        {
            visualRenderers = GetComponentsInChildren<Renderer>();
        }

        SaveOriginalColors();
    }

    void Start()
    {
        fixedZ = transform.position.z;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);

            Vector3 startPos = hit.position;

            if (lockZ)
            {
                startPos.z = fixedZ;
            }

            transform.position = startPos;
            agent.nextPosition = transform.position;

            patrolCenter = transform.position;
            BuildPatrolPoints();

            Debug.Log("Top-down ghost snapped to NavMesh: " + hit.position);
        }
        else
        {
            Debug.LogWarning("Top-down ghost could not find NavMesh near its position.");
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (agent == null) return;
        if (!agent.isOnNavMesh) return;

        damageTimer -= Time.deltaTime;

        if (isStunned)
        {
            UpdateStun();
            return;
        }

        if (fallbackPatrolMode)
        {
            UpdateFallbackPatrol();
        }
        else
        {
            UpdateChasePlayer();
            CheckIfStuck();
        }

        MoveGhostUsingAgent();
    }

    private void UpdateChasePlayer()
    {
        if (targetPlayer == null) return;

        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
            updateTimer = updateRate;
        }
    }

    private void CheckIfStuck()
    {
        if (targetPlayer == null) return;

        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        bool tryingToMove =
            agent.hasPath &&
            agent.remainingDistance > agent.stoppingDistance + 0.2f;

        if (tryingToMove && movedDistance < stuckMoveThreshold)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckCheckTime)
        {
            StartFallbackPatrol();
        }

        lastPosition = transform.position;
    }

    private void StartFallbackPatrol()
    {
        fallbackPatrolMode = true;
        fallbackTimer = fallbackPatrolDuration;
        stuckTimer = 0f;

        patrolCenter = transform.position;
        BuildPatrolPoints();

        movingToRightPoint = !movingToRightPoint;
        SetPatrolDestination();

        Debug.Log("Top-down ghost is stuck, switching to forward/back patrol.");
    }

    private void UpdateFallbackPatrol()
    {
        fallbackTimer -= Time.deltaTime;

        if (fallbackTimer <= 0f)
        {
            fallbackPatrolMode = false;
            updateTimer = 0f;

            Debug.Log("Top-down ghost leaving fallback patrol and chasing again.");
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.25f)
        {
            movingToRightPoint = !movingToRightPoint;
            SetPatrolDestination();
        }
    }

    private void BuildPatrolPoints()
    {
        Vector3 leftRaw = patrolCenter - transform.right * patrolDistance;
        Vector3 rightRaw = patrolCenter + transform.right * patrolDistance;

        patrolLeftPoint = GetNearestNavMeshPoint(leftRaw);
        patrolRightPoint = GetNearestNavMeshPoint(rightRaw);
    }

    private Vector3 GetNearestNavMeshPoint(Vector3 rawPoint)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(rawPoint, out hit, patrolPointSearchRadius, NavMesh.AllAreas))
        {
            Vector3 point = hit.position;

            if (lockZ)
            {
                point.z = fixedZ;
            }

            return point;
        }

        Vector3 fallback = transform.position;

        if (lockZ)
        {
            fallback.z = fixedZ;
        }

        return fallback;
    }

    private void SetPatrolDestination()
    {
        Vector3 destination = movingToRightPoint ? patrolRightPoint : patrolLeftPoint;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    private void MoveGhostUsingAgent()
    {
        Vector3 oldPosition = transform.position;
        Vector3 next = agent.nextPosition;

        if (lockZ)
        {
            next.z = fixedZ;
        }

        transform.position = next;
        agent.nextPosition = transform.position;

        Vector3 moveDirection = transform.position - oldPosition;
        RotateVisualTopDown(moveDirection);
    }

    private void RotateVisualTopDown(Vector3 moveDirection)
    {
        if (visualModel == null) return;
        if (moveDirection.sqrMagnitude < 0.0001f) return;

        Vector3 dir = new Vector3(moveDirection.x, moveDirection.y, 0f).normalized;

        Quaternion lookRotation =
            Quaternion.LookRotation(Vector3.forward, dir) *
            Quaternion.Euler(0f, 0f, visualRotationOffsetZ);

        visualModel.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTopDownPlayerTouch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTopDownPlayerTouch(other);
    }

    private void HandleTopDownPlayerTouch(Collider other)
    {
        if (!other.CompareTag("TopDownPlayer")) return;
        if (damageTimer > 0f) return;
        if (isStunned) return;

        Debug.Log("Top-down ghost hit top-down player. Score -50, ghost stunned/faded.");

        DamagePlayer();
        DecreaseScore(scorePenalty);
        StartStun();

        damageTimer = damageCooldown;
    }

    private void StartStun()
    {
        isStunned = true;
        stunTimer = stunDuration;

        fallbackPatrolMode = false;
        stuckTimer = 0f;

        agent.isStopped = true;
        agent.ResetPath();

        SetGhostAlpha(stunnedAlpha);
    }

    private void UpdateStun()
    {
        stunTimer -= Time.deltaTime;

        // Keep ghost standing still.
        agent.nextPosition = transform.position;

        if (stunTimer <= 0f)
        {
            EndStun();
        }
    }

    private void EndStun()
    {
        isStunned = false;

        SetGhostAlpha(1f);

        agent.isStopped = false;
        updateTimer = 0f;

        Debug.Log("Top-down ghost recovered. Searching player again.");
    }

    private void DamagePlayer()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.TakeDamage();
        }
        else
        {
            Debug.LogWarning("PlayerManager.Instance is missing. Cannot damage player.");
        }
    }

    private void DecreaseScore(int amount)
    {
        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.AddScore(-amount);
            Debug.Log("Score decreased by " + amount);
        }
        else
        {
            Debug.LogWarning("ScoringManager.Instance is missing. Cannot decrease score.");
        }
    }

    private void SaveOriginalColors()
    {
        if (visualRenderers == null) return;

        originalColors = new Color[visualRenderers.Length];

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            Material mat = visualRenderers[i].material;

            if (mat.HasProperty("_BaseColor"))
            {
                originalColors[i] = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color"))
            {
                originalColors[i] = mat.color;
            }
            else
            {
                originalColors[i] = Color.white;
            }
        }
    }

    private void SetGhostAlpha(float alpha)
    {
        if (visualRenderers == null || originalColors == null) return;

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            Material mat = visualRenderers[i].material;
            Color c = originalColors[i];
            c.a = alpha;

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.color = c;
            }
        }
    }
}