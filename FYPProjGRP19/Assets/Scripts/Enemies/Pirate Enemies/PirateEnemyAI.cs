using System.Collections; 
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

// enforce other components
[RequireComponent(typeof(LootDrops))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHP))]
public class PirateEnemyAI : MonoBehaviour
{
    [Header("Player values")]
    private Transform playerTransform; 

    [Header("Enemy attributes")]
    // standard attributes for all pirate enemies 
    [SerializeField] public float attackDamage = 20f; 
    [SerializeField] private float eliteDamage = 40f;   
    [SerializeField] private float nextHealthThreshold;
    private EnemyHP hpScript;
    // attack ranges
    private float attackRange;
    [SerializeField] private float dashDistance = 15f; // distance of the dash attack 
    [SerializeField] private float rangeDistance = 10f;
    [SerializeField] private float meleeDistance = 2.5f;
    // attack hitbox - for elite and melee
    [SerializeField] private GameObject attackCollider;
    [SerializeField] private GameObject blockCollider; // for melee skeleton
    // ranged attack
    [SerializeField] private GameObject projectilePrefab;
    // [SerializeField] private Transform attackPoint; // idk why this isn't working


    [Header("Bool checks")]
    // determine attack type 
    [SerializeField] private bool isRanged;
    [SerializeField] private bool isElite; // dash enemy
    // other bools
    private bool isAttacking; // used to prevent repeat attacks when unintended
    private bool isWalking; // to prevent repeat animation triggers  
    private bool hasDroppedLoot;
    private bool isDead;

    [Header("UI")]
    [SerializeField] private Sprite meleeSprite;
    [SerializeField] private Sprite rangedSprite;
    [SerializeField] private Sprite eliteSprite;
    [SerializeField] private Image mobIcon; 
     
    private NavMeshAgent agent;
    private Animator animator;   

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null) Debug.LogError("Player could not be found");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator could not be found.");  

        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent could not be found.");

        hpScript = GetComponent<EnemyHP>();
        if (hpScript == null) Debug.LogError("EnemyHP script could not be found.");

        // change HP bar icon and adjust attack range
        if (isRanged)
        {
            mobIcon.sprite = rangedSprite;
            attackRange = rangeDistance;

            if (projectilePrefab == null) Debug.LogError("[Ranged Enemy] Projectile prefab has not been assigned");
            // if (attackPoint = null) Debug.LogError("[Ranged Enemy] Attack origin point has not been assigned");
        } else if (isElite)
        {
            mobIcon.sprite = eliteSprite;
            attackRange = dashDistance; 
            attackDamage = eliteDamage;

            if (attackCollider == null) Debug.LogError("[Elite] Attack collider has not been assigned");
            else attackCollider.SetActive(false);
        } else
        {
            mobIcon.sprite = meleeSprite;
            attackRange = meleeDistance; 

            if (attackCollider == null) Debug.LogError("[Melee Enemy] Attack collider has not been assigned");
            else attackCollider.SetActive(false);
            if (blockCollider == null) Debug.LogError("[Melee Enemy] Block collider has not been assigned");
            else blockCollider.SetActive(false);
        } 

        isAttacking = false; 
    }

    private void Update()
    {
        isDead = hpScript.IsDead();
        if (!isDead)
        {
            if (playerTransform != null && agent != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    isWalking = false;
                    agent.isStopped = true; 

                    // attack
                    if (isRanged) StartCoroutine(RangedAttack());
                    else if (isElite) StartCoroutine(DashWindUp());
                    else StartCoroutine(MeleeAttack());  
                }
                if (!isAttacking) // prevent walk cycle starting while attacking
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);

                    if (!isWalking)
                    {
                        animator.SetTrigger("Walk");
                        isWalking = true; // to prevent repeat triggers
                    }
                }
            }
        }
    }

    private IEnumerator DashWindUp()
    { 
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Wind Up") && !isAttacking)
        {
            isAttacking = true;
            // wind up
            animator.SetTrigger("Wind Up"); 
            for (float i = 0; i < animator.GetCurrentAnimatorClipInfo(0).Length; i += Time.deltaTime)
            {
                // keep enemy looking at player
                LookAtPlayer();
                yield return null;
            }

            // begin dash attack
            StartCoroutine(DashAttack());
        }  
    }

    private IEnumerator DashAttack()
    {  
        Vector3 targetPos = playerTransform.position;
        bool cooldownStarted = false;

        // increase acceleration and speed
        float startAcceleration = agent.acceleration;
        float startSpd = agent.speed;
        agent.acceleration = 10f;
        agent.speed = 10f;

        animator.SetTrigger("Dash");
        agent.isStopped = false;
        agent.SetDestination(targetPos);
        attackCollider.SetActive(true);

        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if (distanceToTarget <= 0.5f) break;
            else if (distanceToTarget <= 8f && !cooldownStarted)
            {
                cooldownStarted = true;
                animator.SetTrigger("Cooldown");
            }
            yield return null; // wait for next frame
        }
         
        agent.speed = startSpd;
        agent.acceleration = startAcceleration; 
        attackCollider.SetActive(false);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

        agent.isStopped = false; // re-enable movement
        isAttacking = false; // no longer attacking 
    }

    private IEnumerator RangedAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Aim");

        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            LookAtPlayer();
            yield return null;
        }

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds((float)animator.GetCurrentAnimatorClipInfo(0).Length / 2);

        // shoot projectile
        // ensure the instantiated prefab has a trigger and ProjectileDirEnemy 
        // I DONT KNOW WHY ATTACK POINT ISNT WORKING
        Vector3 attackOrigin = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z + 1f);
        GameObject projectile = Instantiate(projectilePrefab, attackOrigin, Quaternion.LookRotation(transform.forward));
        ProjectileDirEnemy projectileDir = projectile.GetComponent<ProjectileDirEnemy>();

        if (projectileDir != null)
        {
            Vector3 targetPos = playerTransform.position;
            Vector3 throwDirection = targetPos - transform.position;

            // Set the direction of the projectile
            projectileDir.SetDirection(throwDirection);
        }

        yield return new WaitForSeconds((float)animator.GetCurrentAnimatorClipInfo(0).Length / 2);

        isAttacking = false;
    } 

    private IEnumerator MeleeAttack()
    {
        agent.isStopped = true;
        isAttacking = true;
        blockCollider.SetActive(true);
        animator.SetTrigger("Shield Up");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 0.5f);

        // block attacks from the front
        animator.SetTrigger("Guard");
        for (float i = 0; i < 1.5f; i += Time.deltaTime) // defend for 1.5s before attacking
        {
            LookAtPlayer(); // keep looking at player 
            yield return null; // wait for next frame
        }

        // attack
        blockCollider.SetActive(false); // stop blocking
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.11f); // wait for the beginning of the swing
        attackCollider.SetActive(true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 0.8f);

        agent.isStopped = false;
        isAttacking = false;
        attackCollider.SetActive(false);
    }

    private void LookAtPlayer()
    {
        Vector3 lookAtPos = playerTransform.position - transform.position;
        lookAtPos.y = 0; // prevent looking up or down
        Quaternion rotation = Quaternion.LookRotation(lookAtPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
    } 

    private void HandleDeath()
    {
        agent.isStopped = true;
        gameObject.GetComponent<Collider>().enabled = false;

        isDead = true; // prevent repeated deaths
        SoundManager.Instance.PlayDeathSound();

        animator.SetTrigger("Defeat");
        StartCoroutine(HandleDeathAfterAnimation());
    }

    private IEnumerator HandleDeathAfterAnimation()
    { 
        // wait for animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

        if (!hasDroppedLoot)
        {
            LootDrops lootDrop = GetComponent<LootDrops>();
            if (lootDrop != null) 
                lootDrop.DropLoot(); 

            hasDroppedLoot = true;
        }

        // Destroy the enemy game object after loot drop
        Destroy(gameObject);
    } 
}
