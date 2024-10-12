using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHP))]
public class PirateBossAI : MonoBehaviour
{
    [Header("Player values")]
    private Transform playerTransform;

    [Header("Enemy attributes")]
    [SerializeField] private string bossName = "Gideon Undertow the Undead Captain";
    private EnemyHP hpScript;

    [Header("Melee Attack")]
    public float attackDamage = 40f;
    private float maxHealth;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField,
        Tooltip("The collider used to attack the player (melee) - likely attached to the hand")]
        private GameObject attackCollider;

    [Header("Ranged Attack")] // damage is managed in a separate script
    [SerializeField] private GameObject[] rangedPrefabs;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float rangedCooldown = 10f;
    private float rangedTimer;

    [Header("Cannons")]
    [SerializeField] private GameObject[] cannons;

    [Header("Bool checks")]
    [SerializeField] private bool inPhase2 = false; // determine if boss is in second phase  
    private bool isAttacking;
    private bool rangedTimerStarted;
    private bool isWalking; // to prevent repeat calls for walking animation 
    private bool changingPhase;
    private bool isDead = false;
    private bool hasDroppedLoot; 

    [Header("UI")] 
    [SerializeField] private TMP_Text bossNameTxt;

    [Header("NavMesh/Movement")]
    [SerializeField] private Transform navMeshNode;
    [SerializeField, Range(0,100)] private int startRangedAttackChance = 20;
    private int rangedAttackChance;
    [SerializeField, Range(0, 100)] private int rangedAttackChanceIncrement = 20;

    private NavMeshAgent agent;
    private Animator animator;
    // private int lastNode;

    private void Start()
    {
        /*playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null) Debug.LogError("Player could not be found");*/

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator could not be found.");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent could not be found.");
        else
        {
            agent.speed = 10f;
            agent.acceleration = 10f;
        }

        hpScript = GetComponent<EnemyHP>();
        if (hpScript == null) Debug.LogError("EnemyHP script could not be found.");
        else maxHealth = hpScript.GetCurrentHealth();

        // more missing assignment handling
        if (rangedPrefabs.Length == 0) Debug.LogError("Projectile prefabs not assigned.");
        if (attackPoint == null) Debug.LogError("Attack point was not assigned.");  // not functioning
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        else attackCollider.SetActive(false); // prevent damage from player at the beginning  
        if (cannons.Length == 0) Debug.LogError("No cannons have been assigned.");
        else
        { 
            foreach (GameObject cannon in cannons)
                cannon.SetActive(false);
        }

        rangedAttackChance = startRangedAttackChance;
        rangedTimerStarted = true; // prevent the first action from being a ranged attack
        rangedTimer = 0;
        changingPhase = false;
        // lastNode = -1;

        // set UI
        bossNameTxt.SetText(bossName);
    }

    private void Update()
    {
        if (playerTransform == null) playerTransform = GameObject.FindWithTag("Player").transform;
        isDead = hpScript.IsDead();
        if (hpScript.GetCurrentHealth() <= (float)maxHealth / 2 && !inPhase2) StartCoroutine(StartPhaseTwo());
        // if (!inPhase2 && !changingPhase) StartCoroutine(StartPhaseTwo()); // uncomment this and comment above to test phase 2
        if (!isDead && !changingPhase)
        {
            if (inPhase2) PhaseTwo();
            else PhaseOne();
        }
    }

    private void PhaseOne()
    {
        if (playerTransform != null && agent != null)
        {
            // Debug.Log("Ranged attack chance: " + rangedAttackChance);
            if (rangedAttackChance >= 100 && !isAttacking && rangedTimer >= rangedCooldown) // guaranteed ranged attack
            {
                // Debug.Log("Guaranteed ranged attack");
                rangedAttackChance = startRangedAttackChance;
                StartCoroutine(RangedAttack(2));
            }
            else
            { 
                int attackCheck = Random.Range(1, 101);
                // Debug.Log("Attack check: " + attackCheck);
                if (attackCheck <= rangedAttackChance && !isAttacking && rangedTimer >= rangedCooldown)
                {
                    rangedAttackChance = startRangedAttackChance;
                    StartCoroutine(RangedAttack(1));
                } 
                else
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                
                    if (distanceToPlayer <= attackRange && !isAttacking) // close enough to attack and not already attacking
                    {
                        isWalking = false;
                        agent.isStopped = true;
                        rangedAttackChance += rangedAttackChanceIncrement;
                
                        StartCoroutine(MeleeAttack());
                    }
                    if (!isAttacking) // prevent walk cycle from interrupting attack
                    {
                        // Debug.Log(name + " is walking!");
                        agent.isStopped = false; // ensure agent can move
                        agent.SetDestination(playerTransform.position);
                
                        if (!isWalking)
                        {
                            animator.SetTrigger("Walk");
                            isWalking = true; // prevent repeat triggers
                        }
                    }
                }
            }

            if (rangedTimerStarted) 
                rangedTimer += Time.deltaTime; 
        }
    } 

    private IEnumerator StartPhaseTwo()
    {
        changingPhase = true;

        animator.SetTrigger("Walk");
        agent.SetDestination(navMeshNode.position);

        while (transform.position != agent.destination)
            yield return null;

        animator.SetTrigger("Cannon Attack");
        GetComponent<CapsuleCollider>().enabled = false; // prevent damage

        yield return new WaitForSeconds(7f); // when the boss is pointing

        foreach (GameObject cannon in cannons) 
            cannon.SetActive(true); 

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 7f);

        GetComponent<CapsuleCollider>().enabled = true;
        changingPhase = false;
        inPhase2 = true; 
    }

    private void PhaseTwo()
    {
        if (playerTransform != null && agent != null)
        {
            // Debug.Log("Ranged attack chance: " + rangedAttackChance);
            if (rangedAttackChance >= 100 && !isAttacking && rangedTimer >= rangedCooldown) // guaranteed ranged attack
            {
                // Debug.Log("Guaranteed ranged attack");
                rangedAttackChance = startRangedAttackChance;
                StartCoroutine(RangedAttack(4));
            }
            else
            {
                int attackCheck = Random.Range(1, 101);
                // Debug.Log("Attack check: " + attackCheck);
                if (attackCheck <= rangedAttackChance && !isAttacking && rangedTimer >= rangedCooldown)
                {
                    rangedAttackChance = startRangedAttackChance;
                    StartCoroutine(RangedAttack(2));
                }
                else
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                    if (distanceToPlayer <= attackRange && !isAttacking) // close enough to attack and not already attacking
                    {
                        isWalking = false;
                        agent.isStopped = true;
                        rangedAttackChance += rangedAttackChanceIncrement;

                        StartCoroutine(MeleeAttack());
                    }
                    if (!isAttacking) // prevent walk cycle from interrupting attack
                    {
                        // Debug.Log(name + " is walking!");
                        agent.isStopped = false; // ensure agent can move
                        agent.SetDestination(playerTransform.position);

                        if (!isWalking)
                        {
                            animator.SetTrigger("Walk");
                            isWalking = true; // prevent repeat triggers
                        }
                    }
                }
            }

            if (rangedTimerStarted)
                rangedTimer += Time.deltaTime;
        }
    }

    private string RandomizeAttackHand()
    {
        int attackCheck = Random.Range(0, 2);
        if (attackCheck == 0) return "L Attack";
        else return "R Attack";
    } 

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        LookAtPlayer();
        animator.SetTrigger("Push");  

        yield return new WaitForSeconds(0.04f);

        attackCollider.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        attackCollider.SetActive(false);

        // wait for the animation to end
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 0.14f);

        agent.isStopped = false;
        isAttacking = false;
    } 

    /// <summary>
    /// Ranged attack for the pirate boss - specific to its animation timings
    /// </summary>
    /// <param name="numOfAttacks">Number of attacks in a row</param>
    /// <returns></returns>
    private IEnumerator RangedAttack(int numOfAttacks)
    {
        isAttacking = true;
        agent.isStopped = true; // prevent movement

        // reset timer
        rangedTimer = 0;
        rangedTimerStarted = false;

        for (int i = 0; i < numOfAttacks; i++)
        {
            animator.SetTrigger(RandomizeAttackHand());

            for (float j = 0; j < 0.05f; j += Time.deltaTime)
            {
                LookAtPlayer();
                yield return null;
            } 

            // randomize between projectiles
            int projectileRand = Random.Range(0, rangedPrefabs.Length);
            GameObject projectile = Instantiate(rangedPrefabs[projectileRand],
                attackPoint.position, Quaternion.LookRotation(playerTransform.position));
            /*Vector3 attackOrigin = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z + 1f);
            GameObject projectile = Instantiate(rangedPrefabs[projectileRand],
                attackOrigin, Quaternion.LookRotation(playerTransform.position));*/
            ProjectileDirEnemy projectileDir = projectile.GetComponent<ProjectileDirEnemy>();

            if (projectileDir != null)
            {
                Vector3 targetPos = playerTransform.position;
                Vector3 throwDirection = CalculateThrowDirection(attackPoint.position, targetPos);

                projectileDir.SetDirection(throwDirection);
            }

            // wait for the remainder of the animation
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 0.05f);
        } 

        isAttacking = false;
        rangedTimerStarted = true;
    } 

    // Simplified throw direction calculation with an arc
    private Vector3 CalculateThrowDirection(Vector3 start, Vector3 target)
    {
        // Get the direction to the player
        Vector3 direction = (target - start).normalized;

        // Give it an upward arc
        direction.y = 0.5f;

        return direction;
    }

    private void LookAtPlayer()
    {
        Vector3 lookAtPos = playerTransform.position - transform.position;
        lookAtPos.y = 0; // prevent looking up or down
        Quaternion rotation = Quaternion.LookRotation(lookAtPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
    }
}
