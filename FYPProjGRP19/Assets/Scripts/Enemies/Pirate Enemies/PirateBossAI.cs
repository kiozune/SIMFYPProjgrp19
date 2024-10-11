using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

[RequireComponent(typeof(NavMeshAgent))]
public class PirateBossAI : MonoBehaviour
{
    [Header("Player values")]
    private Transform playerTransform;

    [Header("Enemy attributes")]
    [SerializeField] private string bossName = "Gideon Undertow the Undead Captain";
    [SerializeField] private float maxHP = 1000f;
    private float currentHP;

    [Header("Melee Attack")]
    public float attackDamage = 40f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField,
        Tooltip("The collider used to attack the player (melee) - likely attached to the hand")]
        private GameObject attackCollider;

    [Header("Ranged Attack")] // damage is managed in a separate script
    [SerializeField] private GameObject[] rangedPrefabs;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float rangedCooldown = 10f;
    private float rangedTimer;

    [Header("Bool checks")]
    [SerializeField] private bool inPhase2 = false; // determine if boss is in second phase
    [SerializeField] private bool cannonsActivated = false; // determine if cannons have been activated
    private bool isAttacking;
    private bool rangedTimerStarted;
    private bool isWalking; // to prevent repeat calls for walking animation
    private bool soundPlayed;
    private bool isDead = false;
    private bool hasDroppedLoot;


    [Header("UI")]
    [SerializeField] private SliderBar bossHPBar;
    [SerializeField] private TMP_Text bossNameTxt;

    [Header("VFX/SFX")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private AudioClip hitSoundClip;
    [SerializeField] private AudioSource audioSource;

    [Header("NavMesh/Movement")]
    /*[SerializeField,
        Tooltip("Ensure each node is located at a valid spot on the NavMeshSurface")] private Transform[] movementNodes;*/
    [SerializeField, Range(0,100)] private int startRangedAttackChance = 20;
    private int rangedAttackChance;
    [SerializeField, Range(0, 100)] private int rangedAttackChanceIncrement = 20;

    private NavMeshAgent agent;
    private Animator animator;
    // private int lastNode;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null) Debug.LogError("Player could not be found");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator could not be found.");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent could not be found.");
        else
        {
            agent.speed = 10f;
            agent.acceleration = 10f;
        }

        // more missing assignment handling
        if (rangedPrefabs.Length == 0) Debug.LogError("Projectile prefabs not assigned.");
        if (attackPoint == null) Debug.LogError("Attack point was not assigned."); 
        if (bossHPBar == null) Debug.LogError("HP bar was not assigned.");
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        else attackCollider.SetActive(false); // prevent damage from player at the beginning
        // if (movementNodes.Length == 0) Debug.LogWarning("No NavMesh movement nodes were assigned.");

        currentHP = maxHP;
        rangedAttackChance = startRangedAttackChance;
        rangedTimerStarted = true; // prevent the first action from being a ranged attack
        rangedTimer = 0;
        // lastNode = -1;

        // set UI
        bossNameTxt.SetText(bossName);
    }

    private void Update()
    {
        if (!isDead)
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

    private void PhaseTwo()
    {

    }

    private string RandomizeAttackHand()
    {
        int attackCheck = Random.Range(0, 2);
        if (attackCheck == 0) return "L Attack";
        else return "R Attack";
    }

    /*private GameObject RandomizeNode()
    {
        int node;
        while (true) // ensure next destination is not the same as the last
        {
            node = Random.Range(0, rangedPrefabs.Length);
            if (node != lastNode || lastNode == -1) break; // -1 will indicate this is the first time this function was called
        }
        lastNode = node;
        return rangedPrefabs[node];
    }*/

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
            /* I DONT KNOW WHY ATTACK POINT ISNT WORKING
            GameObject projectile = Instantiate(rangedPrefabs[projectileRand],
                attackPoint.position, Quaternion.LookRotation(playerTransform.position));
             */
            Vector3 attackOrigin = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z + 1f);
            GameObject projectile = Instantiate(rangedPrefabs[projectileRand],
                attackOrigin, Quaternion.LookRotation(playerTransform.position));
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
