using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;                 // Auto-found by tag if left empty

    [Header("Detection")]
    public float detectionRadius = 15f;      // How far zombie can see
    public float fieldOfView = 120f;         // Vision cone (degrees)
    public float loseSightTime = 3f;         // Time after losing sight before giving up

    [Header("Movement")]
    public float roamSpeed = 1.2f;
    public float chaseSpeed = 3.5f;
    public float roamRadius = 8f;
    public float roamPointTolerance = 0.4f;

    [Header("Attack")]
    public float attackRange = 1.6f;         // Distance to hit player
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;      // Seconds between attacks
    public string attackStateName = "Z_Attack";

    [Header("Animation (state names must match Animator)")]
    public Animator animator;
    public string idleStateName = "Z_Idle";
    public string walkStateName = "Z_Walk_InPlace";
    public string runStateName  = "Z_Run_InPlace";

    private NavMeshAgent agent;
    private Vector3 spawnPos;
    private float timeSinceSeen = 999f;
    private float nextAttackTime = 0f;
    private PlayerHealth playerHealth;

    private enum State { Roam, Chase }
    private State state = State.Roam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                Debug.LogWarning("ZombieAI: Player has no PlayerHealth component.");
        }
        else
        {
            Debug.LogError("ZombieAI: No Player found! Tag your player as 'Player'.");
            enabled = false;
            return;
        }

        spawnPos = transform.position;
        SetNewRoamPoint();
        PlayIdle();
    }

    void Update()
    {
        if (!player) return;

        bool seesPlayer = CanSeePlayer();

        // Hard reset if very far away
        float farDistance = detectionRadius * 2f;
        if (Vector3.Distance(transform.position, player.position) > farDistance)
        {
            state = State.Roam;
            timeSinceSeen = loseSightTime + 1f;
            Roam();
            UpdateAnimation();
            return;
        }

        if (seesPlayer)
        {
            timeSinceSeen = 0f;
            state = State.Chase;
        }
        else
        {
            timeSinceSeen += Time.deltaTime;
            if (timeSinceSeen > loseSightTime)
                state = State.Roam;
        }

        if (state == State.Chase)
        {
            ChasePlayer();

            // Try to attack when close enough
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange)
                TryAttack();
        }
        else
        {
            Roam();
        }

        UpdateAnimation();
    }

    // --------- MOVEMENT & DETECTION ---------

    void Roam()
    {
        agent.isStopped = false;
        agent.speed = roamSpeed;

        if (!agent.hasPath || agent.remainingDistance < roamPointTolerance)
            SetNewRoamPoint();
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void SetNewRoamPoint()
    {
        Vector3 random = spawnPos + Random.insideUnitSphere * roamRadius;

        if (NavMesh.SamplePosition(random, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        if (!player) return false;

        Vector3 toPlayer = player.position - transform.position;
        float sqrDist = toPlayer.sqrMagnitude;

        // Distance cutoff
        if (sqrDist > detectionRadius * detectionRadius)
            return false;

        // FOV check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfView * 0.5f)
            return false;

        // Line-of-sight raycast
        Vector3 eyePos = transform.position + Vector3.up * 1.6f;
        if (Physics.Raycast(eyePos, dirToPlayer, out RaycastHit hit, detectionRadius))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    // --------- ATTACK ---------

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        if (playerHealth == null) return;

        nextAttackTime = Time.time + attackCooldown;

        // Face the player
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);

        // Stop the agent briefly while attacking
        agent.isStopped = true;

        // Play attack animation
        if (animator != null)
            animator.Play(attackStateName);

        // Apply damage immediately (simple version)
        playerHealth.TakeDamage(attackDamage);

        // Resume movement after a short delay (tweak to match anim)
        Invoke(nameof(ResumeMovement), 0.6f);
    }

    void ResumeMovement()
    {
        agent.isStopped = false;
    }

    // --------- ANIMATION ---------

    void UpdateAnimation()
    {
        if (!animator) return;

        if (IsInAttackState()) return; // don't override attack mid-swing

        float speed = agent.velocity.magnitude;

        if (state == State.Chase)
        {
            PlayRun();
        }
        else
        {
            if (speed > 0.2f)
                PlayWalk();
            else
                PlayIdle();
        }
    }

    bool IsInAttackState()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(attackStateName);
    }

    void PlayIdle()
    {
        if (!animator) return;
        if (IsInAttackState()) return;
        animator.Play(idleStateName);
    }

    void PlayWalk()
    {
        if (!animator) return;
        if (IsInAttackState()) return;
        animator.Play(walkStateName);
    }

    void PlayRun()
    {
        if (!animator) return;
        if (IsInAttackState()) return;
        animator.Play(runStateName);
    }

    // --------- GIZMOS ---------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? spawnPos : transform.position;
        Gizmos.DrawWireSphere(center, roamRadius);
    }
}
