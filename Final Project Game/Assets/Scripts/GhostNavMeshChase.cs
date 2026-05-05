using UnityEngine;
using UnityEngine.AI;

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

    [Header("2D Plane Lock")]
    public bool lockZ = true;

    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float damageCooldown = 1.5f;

    private NavMeshAgent agent;
    private float updateTimer;
    private float damageTimer;
    private float fixedZ;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed;
        agent.stoppingDistance = 0f;

        // Let NavMesh calculate path, but do not let it move/rotate the Transform directly.
        agent.updatePosition = false;
        agent.updateRotation = false;
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

            Debug.Log("Ghost snapped to NavMesh: " + hit.position);
        }
        else
        {
            Debug.LogWarning("Ghost could not find NavMesh near its position.");
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;
        if (agent == null) return;
        if (!agent.isOnNavMesh) return;

        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            agent.SetDestination(targetPlayer.position);
            updateTimer = updateRate;
        }

        Vector3 oldPosition = transform.position;
        Vector3 next = agent.nextPosition;

        if (lockZ)
        {
            next.z = fixedZ;
        }

        transform.position = next;

        // Keep the NavMesh simulation synced with the visible ghost.
        agent.nextPosition = transform.position;

        Vector3 moveDirection = transform.position - oldPosition;
        RotateVisual2D(moveDirection);

        damageTimer -= Time.deltaTime;
    }

    private void RotateVisual2D(Vector3 moveDirection)
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
            Debug.Log("Ghost touched player. Damage player here.");

            PlayerManager.Instance.TakeDamage();

            damageTimer = damageCooldown;
        }
    }
}