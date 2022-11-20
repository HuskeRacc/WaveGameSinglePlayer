using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class EnemyController : MonoBehaviour
{
    [Header("Ragdoll")]
    [SerializeField] List<Collider> ragdollParts = new();
    [SerializeField] Rigidbody[] rbs;
    [SerializeField] Rigidbody mainRb;
    [SerializeField] Animator anim;

    [Header("Navigation/AI")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] GameObject playerGO;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

    [Header("Walkpoints")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    [SerializeField] float walkPointRange;

    [Header("Attacking")]
    [SerializeField] float attackDamage = 25f;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttacked;
    [SerializeField] PlayerHealth playerHealth;

    [Header("Ranges")]
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSightRange;
    [SerializeField] bool playerInAttackRange;

    [Header("Enemy Stats")]
    [SerializeField] float health;
    [SerializeField] GunScript gunScript;
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] WaveSpawner waveSpawner;
    [SerializeField] EnemiesAlive enemiesAlive;

    [Header("Optimization")]
    [SerializeField] float ragdollDeletionTime = 3f;
    [SerializeField] bool AiDisabled = false;

    #region init
    private void Awake()
    {
        SetRagdollParts();
        Reference();
        mainRb.isKinematic = true;
        foreach(Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
    }

    void Reference()
    {
        playerGO = GameObject.Find("Player");
        playerHealth = playerGO.GetComponent<PlayerHealth>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        gunScript = player.GetComponentInChildren<GunScript>();
        anim = gameObject.GetComponent<Animator>();
        rbs = gameObject.GetComponents<Rigidbody>();
        rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        mainRb = gameObject.GetComponent<Rigidbody>();
        waveSpawner = GameObject.Find("WaveSpawner").GetComponent<WaveSpawner>();
        
    }

    #endregion


    private void Update()
    {
        //Check sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!AiDisabled)
        {
            if (!playerInSightRange && !playerInAttackRange) Patrol();

            if (playerInSightRange && !playerInAttackRange) Chase();

            if (playerInSightRange && playerInAttackRange) Attack();
        }
    }

    #region AI

    void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(walkPoint);
            anim.SetFloat("Forward", 1);
            anim.SetBool("Running", false);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 5f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        anim.SetFloat("Forward", 1);
        anim.SetBool("Running", true);

    }

    void ForceChase()
    {
        sightRange = 100f;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        anim.SetFloat("Forward", 1);
        anim.SetBool("Running", true);

    }
    void Attack()
    {
        //stop moving
        agent.speed = 1;
        agent.SetDestination(transform.position);
        anim.SetFloat("Forward", 0);
        anim.SetBool("Running", false);

        transform.LookAt(player,Vector3.up);

        if (!alreadyAttacked)
        {
            playerHealth.health -= attackDamage;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        ForceChase();

        if(health <= 0)
        {
            agent.enabled = false;
            AiDisabled = true;
            TurnOnRagdoll();
            Invoke(nameof(DestroyRagdoll), ragdollDeletionTime);
        }
    }

    void DestroyRagdoll()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);

    }

    #endregion

    #region Ragdoll

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider c in colliders)
        {
            if( c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                ragdollParts.Add(c);
            }
        }
    }

    public  void TurnOnRagdoll()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
        this.GetComponent<CapsuleCollider>().enabled = false;
        anim.enabled = false;

        foreach(Collider c in ragdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
            waveSpawner.enemiesKilled++;
    }

    #endregion
}
