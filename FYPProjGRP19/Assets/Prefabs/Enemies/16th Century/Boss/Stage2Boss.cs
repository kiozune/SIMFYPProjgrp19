using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.MeshOperations;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class Stage2Boss : MonoBehaviour
{
    private Animator animator;
    public GameObject player;
    private NavMeshAgent agent;
    private EnemyHP bossHP;
    public GameObject flag;

    public float detectionRange = 15f;
    public float attackRange = 5f;
    private float chaseTimer = 0f;
    private float transition = 0f;

    private float distanceToPlayer;
    private bool inRange = false;
    private bool isAttacking = false;
    private bool isPhase = false;
    private bool isChasing = false;
    private string currentAnimation = "";
    private int currentAttack = 0;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();    //Initialize Animator
        agent = GetComponent<NavMeshAgent>();  // Initialize NavMeshAgent
        bossHP = GetComponent<EnemyHP>();       //Get HP elements

        agent.updateRotation = false;


        StartCoroutine(FindPlayer());
        StartCoroutine(ChangeAttack());

        ChangeAnimation("Idle");

        IEnumerator ChangeAttack()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Get current state of layer 0

            while (true)
            {
                yield return new WaitForSeconds(stateInfo.length);
                ++currentAttack;
                if (currentAttack >= 5)
                {
                    currentAttack = 0;
                }
            }
        }
    }

    // Coroutine to find the player after spawn
    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            //variables requiring constant frame update
            

            yield return new WaitForSeconds(0.5f);  // Check every 0.5 seconds
        }
    }

    //put conditionals in here
    private void Update()
    {
        //debug log
        //Debug.Log("isChasing is " + isChasing);
        //Debug.Log("chase timer is" + chaseTimer);

        //check every frame
        DetectPlayer();
        CheckAnimation();

        if (bossHP.GetCurrentHealth() <= 0)
        {
            HandleDeath();
        }
        if (isPhase && isAttacking && !isChasing)
        {
            ChangeAnimation("BossSpecial2");
        }
        else if (!isPhase && isAttacking && !isChasing)
        {
            ChangeAnimation("BossAttack");
        }

        //if boss is at half health or lower, play midphase animation
        if (bossHP.HalfHealth())
        {
            isPhase = true;
            return;
        }

        //check chase
        if (isChasing && inRange)
        {
            chaseTimer += Time.deltaTime;
        }
        else
        {
            chaseTimer = 0f;
        }

        //if chase and in range to attack
        if (distanceToPlayer > attackRange)
        {
            isChasing = true;
            isAttacking = false;
            return;
        }
        else if (distanceToPlayer <= attackRange)
        {
            chaseTimer = 0f;
            isChasing = false;
            isAttacking = true;
            return;
        }
        /*
        //variables requiring constant frame update
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //agent.stoppingDistance = attackRange - 1;  // Stops when within attack range

        //start looking for Player
        DetectPlayer(); //includes walk and run anim bools

        //if the player is within the boss' attack range
        if (distanceToPlayer <= attackRange)
        {
            //set mode
            isAttacking = true;
            //stop walking
            animator.SetBool("Walk", false);
            //stop walking while attacking
            agent.ResetPath();
            //play attack animation
            animator.SetBool("Attack", true);
        }
        else if (distanceToPlayer > attackRange) //if player is outside boss' range
        {
            //set mode
            isAttacking = false;
            //stop attack animation
            animator.SetBool("Attack", false);
            //resume tracking player
            DetectPlayer();
        }

        if (bossHP.HalfHealth())
        {
            isPhase = true;
            transition = 5f;
        }

        if (isPhase)
        {
            agent.ResetPath();
            animator.SetBool("Phase", true);
            transition -= Time.deltaTime;
        }

        if (isPhase && transition <= 0)
        {
            animator.SetBool("Phase", false);
        }
        */
    }
    //check animation played
    private void CheckAnimation()
    {
        if (isDead)
        {
            ChangeAnimation("BossDefeat");
            return;
        }
        else if (chaseTimer >= 10)
        {
            ChangeAnimation("BossRun");
            agent.SetDestination(player.transform.position);
            FacePlayer();
            //run at increased speed
            agent.speed = 12f;
            attackRange = 3f;
        }
        else if (isPhase && isAttacking)
        {
            agent.SetDestination(player.transform.position);
            FacePlayer();
            agent.ResetPath();
            return;
        }
        /*
        else if (isPhase)
        {
            ChangeAnimation("BossBuff");
            return;
        }
        */
        else if (isAttacking)
        {
            agent.SetDestination(player.transform.position);
            FacePlayer();
            agent.ResetPath();
        }
        //chase player
        else if (inRange)
        {
            ChangeAnimation("BossWalk");
            agent.SetDestination(player.transform.position);
            FacePlayer();
            agent.speed = 3.5f;
            attackRange = 5f;
        }
        else
        {
            ChangeAnimation("BossIdle");
        }


        /*
        void CheckAttack()
        {
            void IsInRange()
            {
                // Check if the player is within attack range
                if (distanceToPlayer > attackRange)
                {
                    isAttacking = false;
                    CheckAnimation();
                    // If the player is outside the attack range, exit the function
                    return;
                }
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Get current state of layer 0

            if (currentAttack == 0 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSpecial1");
                agent.SetDestination(player.transform.position);
                FacePlayer();
                IsInRange();
                return;
            }
            else if (currentAttack == 1 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSpecial2");
                agent.SetDestination(player.transform.position);
                FacePlayer();
                IsInRange();
                return;
            }
            else if (currentAttack == 2 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSummonFlag");
                if (stateInfo.IsName("BossSummonFlag") && stateInfo.normalizedTime >= 1.0f)
                {
                    SpecialSummonFlag();
                }
                IsInRange();
                return;
            }
            else if (currentAttack == 3 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSpecial1");
                agent.SetDestination(player.transform.position);
                FacePlayer();
                IsInRange();
                return;
            }
            else if (currentAttack == 4 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSpecial2");
                agent.SetDestination(player.transform.position);
                FacePlayer();
                IsInRange();
                return;
            }
            else if (currentAttack == 5 && distanceToPlayer <= attackRange)
            {
                ChangeAnimation("BossSummonFlag");
                if (stateInfo.IsName("BossSummonFlag") && stateInfo.normalizedTime >= 1.0f)
                {
                    SpecialSummonFlag();
                }
                IsInRange();
                return;
            }
        }
        */

    }
    public void HandleDeath()
    {
        // Stop enemy movement and handle death logic
        Debug.Log("Death Triggered");
        agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        inRange = false;
        isAttacking = false;
        isPhase = false;
        isChasing = false;
        isDead = true;
    }

    //changes animation
    public void ChangeAnimation(string animation, float crossfade = 0.2f, float time = 0)
    {
        if (time > 0)
        {
            StartCoroutine(Wait());
        }
        else
        {
            Validate();
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
            Validate();
        }

        void Validate()
        {
            if (currentAnimation != animation)
            {
                currentAnimation = animation;

                if (currentAnimation == "")
                {
                    CheckAnimation();
                }
                else
                {
                    animator.CrossFade(animation, crossfade);
                }
            }
        }
    }

    void DetectPlayer()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //if the player is within the initial detection range
        if (distanceToPlayer <= detectionRange)
        {
            //increase detection range to arena range
            detectionRange = 50f;

            inRange = true;
        }
        else //if player is outside of attack range
        {
            inRange = false;
        }
    }
    /*
    //method to detect the player
    void DetectPlayer()
    {
        //if the player is within the initial detection range
        if (distanceToPlayer <= detectionRange) 
        {
            //increase detection range to arena range
            detectionRange = 50f;

            //start walking towards player
            animator.SetBool("Walk", true);
            agent.SetDestination(player.transform.position);
            FacePlayer();

            //if player is outside of attack range
            if (distanceToPlayer > attackRange && !isAttacking)
            {
                //start timer to chase
                chaseTimer += Time.deltaTime;

                //if chase timer is over 15 seconds
                if (chaseTimer > 15 && !isAttacking)
                {
                    //run at increased speed
                    agent.speed = 12f;
                    attackRange = 3f;
                    animator.SetBool("Run", true);
                }
            }
            else if (distanceToPlayer <= attackRange) //if player is within attack range
            {
                //reset chase timer
                //reset boss settings and go back to walking
                agent.speed = 3.5f;
                attackRange = 5f;
                animator.SetBool("Run", false);
                chaseTimer = 0f;
            }
        }
        else //if player moves outside of detection range
        {
            //stop moving and play idle animation
            animator.SetBool("Walk", false);
            agent.ResetPath();
        }
    }
    */

    void FacePlayer()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;  // Ensure the boss only rotates on the y-axis (no tilting up/down)

            // Immediately rotate the boss to face the player without smoothing to prevent drift
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;  // Directly apply the rotation to avoid drift
        }
    }

    void SpecialSummonFlag()
    {
        // Define the radius around the boss where the flag will be summoned
        float summonRadius = 10f;

        // Generate a random point within a circle of radius summonRadius
        Vector2 randomPoint = Random.insideUnitCircle * summonRadius;

        // Convert the 2D point to 3D by adding it to the boss's position
        Vector3 summonPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);

        // Instantiate the flag at the generated position
        Instantiate(flag, summonPosition, Quaternion.identity);

        //Debug.Log("Summoned Flag at position: " + summonPosition);

        return;
    }

    IEnumerator SummonOneFlag()
    {
        SpecialSummonFlag();
        yield return new WaitForEndOfFrame();
    }
    void ResetPhase()
    {
        isPhase = false;
        animator.SetBool("Phase", false);
    }

    // This function will draw the detection range and attack range as gizmos in the scene view
    private void OnDrawGizmosSelected()
    {
        // Set the color for the detection range (yellow)
        Gizmos.color = Color.yellow;
        // Draw a wire sphere to show the detection range
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Set the color for the attack range (red)
        Gizmos.color = Color.red;
        // Draw a wire sphere to show the attack range
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}