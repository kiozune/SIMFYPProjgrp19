using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

[RequireComponent(typeof(NavMeshAgent))]
public class PirateBossAI : MonoBehaviour
{
    [Header("Player values")]
    private GameObject[] players; // array of players - account for multiplayer
    private Transform playerTransform; // target player Transform
    private int targetPlayerIdx = 0;

    [Header("Enemy attributes")]
    [SerializeField] private string bossName = "Gideon Undertow the Undead Captain";
    private BossHealth hpScript;

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

    [Header("Cannon Array")]
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
    [SerializeField] private Image bossHPSliderImg;

    [Header("NavMesh/Movement")]
    [SerializeField] private Transform navMeshNode;
    [SerializeField] private Transform lookAtNode;
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

        hpScript = GetComponent<BossHealth>();
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
        if (lookAtNode == null) Debug.LogError("No Look At Node has been assigned");
        if (bossHPSliderImg == null) Debug.LogError("No Boss HP Slider Image has been assigned");

        rangedAttackChance = startRangedAttackChance;
        rangedTimerStarted = true; // prevent the first action from being a ranged attack
        rangedTimer = 0;
        changingPhase = false;
        targetPlayerIdx = 0;
        // lastNode = -1;

        // set UI
        bossNameTxt.SetText(bossName);
    }

    private void Update()
    {
        isDead = (hpScript.currentHealth == 0);
        if (playerTransform == null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            playerTransform = players[targetPlayerIdx].transform; // set target to first player
        }

        if (hpScript.GetCurrentHealth() <= (float)maxHealth / 2 && !inPhase2 && !changingPhase)
            StartCoroutine(StartPhaseTwo());
        if (!isDead && !changingPhase)
        { 
            if (inPhase2) PhaseTwo();
            else PhaseOne();
        }
    }

    private void ChangeTarget()
    {
        int randInt = Random.Range(0, 2); // coin flip on switching targets
        int isMultiplayer = PlayerPrefs.GetInt("Multiplayer", 0); // Default to singleplayer if not set
        if (randInt == 0 && isMultiplayer != 0)
        {
            if (targetPlayerIdx == 0) // Player 1 
                targetPlayerIdx = 1;
            else // Player 2
                targetPlayerIdx = 0;
            playerTransform = players[targetPlayerIdx].transform;
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
                }
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

            if (rangedTimerStarted) 
                rangedTimer += Time.deltaTime; 
        }
    } 

    private IEnumerator StartPhaseTwo()
    {
        changingPhase = true;
        Color32 hpColor = bossHPSliderImg.color;
        bossHPSliderImg.color = new Color32(108, 177, 185, 255); // a blue-ish grey color
        GetComponent<CapsuleCollider>().enabled = false; // prevent damage

        animator.SetTrigger("Walk");
        agent.SetDestination(navMeshNode.position);

        while (transform.position != agent.destination)
            yield return null; 

        // look towards the bow of the ship
        Vector3 lookAtPos = lookAtNode.position - transform.position;
        lookAtPos.y = 0; // prevent looking up or down
        Quaternion rotation = Quaternion.LookRotation(lookAtPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);

        animator.SetTrigger("Cannon Attack");

        yield return new WaitForSeconds(7f); // when the boss is pointing

        foreach (GameObject cannon in cannons) 
            cannon.SetActive(true); 

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 7f);

        bossHPSliderImg.color = hpColor; // return to original color
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
                }
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
        ChangeTarget();

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
            /* // in the chance attackPoint stops working
            Vector3 attackOrigin = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z + 1f);
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
