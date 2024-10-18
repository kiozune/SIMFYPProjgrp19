using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Stage2Boss : MonoBehaviour
{
    private Animator animator;
    public GameObject player;
    private NavMeshAgent agent;
    public GameObject flag;

    private EnemyHP bossHP;
    public float detectionRange = 15f;
    public float attackRange = 5f;
    private float chaseTimer = 0;
    private bool isRunning = false;  // To track if the boss is running
    private bool isInRange = false;
    private float cooldown = 0;
    private bool phaseChange = false;
    private bool nextPhase = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();  // Initialize NavMeshAgent
        bossHP = GetComponent<EnemyHP>();

        agent.updateRotation = false;
        
        StartCoroutine(FindPlayer());
        animator.SetTrigger("Idle");

        if (bossHP.HalfHealth() == true)
        {
            ResetAllAnimatorTriggers(animator);
            animator.SetTrigger("Phase");
        }
    }

    // Coroutine to find the player after spawn
    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return new WaitForSeconds(0.5f);  // Check every 0.5 seconds
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.stoppingDistance = attackRange - 1;  // Stops when within attack range

        //Debug.Log("Boss Range is " + detectionRange);

        //Debug.Log("CurrentHP: " + currentHP + " and PercentChange" + phaseChange);
        if (player != null)
        {
            if (bossHP.HalfHealth() == true)
            {
                Phase2();
            }
            else
            {
                Phase1();
            }
            //DetectPlayer();  // Check if player is detected
            //InRange();       // Check if player is in attack range
        } 
    }

    void Phase1()
    {
        DetectPlayer();
        InRange();
        if (isInRange)
        {
            BasicCombo();
        }
        else
        {
            animator.ResetTrigger("Attack");
        }
    }

    void PhaseTransition()
    {
        if (!phaseChange)
        {
            ResetAllAnimatorTriggers(animator);
            animator.SetTrigger("Phase");
            phaseChange = true;
        }
        else
        {
            animator.ResetTrigger("Phase");
            nextPhase = true;
        }
    }
    //2nd Phase Attack Pattern
    void Phase2()
    {
        //animator.ResetTrigger("Phase");
        PhaseTransition();

        DetectPlayer();
        InRange();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Move using the NavMeshAgent only if the walk animation is playing
        if (!stateInfo.IsName("BossBuff") && nextPhase)
        {
            Moveset();
        }
    }

        // Ensure the boss is always facing the player
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


    // Detect if the player is within detection range
    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        

        if (distanceToPlayer <= detectionRange)
        {
            detectionRange = 50f;

            animator.ResetTrigger("Idle");
            if (!isRunning)  // Only trigger walk if not already running
            {
                animator.SetTrigger("Walk");
                Debug.Log("Player Detected");

                // Check if the walking animation has started before moving
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Move using the NavMeshAgent only if the walk animation is playing
                if (stateInfo.IsName("BossWalk") && !agent.pathPending && distanceToPlayer > agent.stoppingDistance)
                {
                    chaseTimer += Time.deltaTime;

                    // After walking for 10 seconds, switch to running
                    if (chaseTimer >= 10)
                    {
                        StartCoroutine(RunTowardsPlayer());  // Start running using a while loop in a coroutine
                    }

                    // Continuously update the destination to the player's position
                    agent.SetDestination(player.transform.position);

                    FacePlayer();
                }
            }
        }
        else
        {
            // If the player is out of detection range, reset the boss to idle
            StopRunning();  // Reset running if player is lost
            animator.SetTrigger("Idle");
            Debug.Log("Player Lost");
            attackRange = 5f;  // Reset attack range to default
            agent.ResetPath();  // Stop the agent from moving
        }
    }

    // Coroutine to handle running with a while loop
    IEnumerator RunTowardsPlayer()
    {
        isRunning = true;
        animator.SetBool("isRunning", true);
        animator.ResetTrigger("Walk");
        animator.SetTrigger("Run");
        agent.speed = 12f;  // Increase speed to running
        attackRange = 3f;   // Shrink attack range
        Debug.Log("Boss is running");

        while (isRunning)  // Keep running until a condition is met
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                // Stop running when within attack range
                isRunning = false;
                animator.SetBool("isRunning", false);
                agent.ResetPath();  // Stop the agent from moving when attacking
                animator.ResetTrigger("Run");
                animator.SetTrigger("Attack");
                attackRange = 5f;   
                Debug.Log("Attacking Player");
                yield break;  // Exit the coroutine
            }

            // Continuously update the destination to the player's position
            agent.SetDestination(player.transform.position);
            FacePlayer();

            yield return null;  // Wait for the next frame
        }
    }

    // Function to stop running and reset everything if the player is lost
    void StopRunning()
    {
        if (isRunning)
        {
            StopCoroutine(RunTowardsPlayer());
            chaseTimer = 0;
            isRunning = false;
            animator.SetBool("isRunning", false);
            agent.speed = 3.5f;  // Reset speed to walk speed
            attackRange = 5f;
            animator.ResetTrigger("Run");
            Debug.Log("Boss stopped running");
        }
    }

    // Check if the player is in attack range
    void InRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {

            StopRunning();  // Stop running when attacking
            animator.ResetTrigger("Walk");
            isInRange = true;
            FacePlayer();
            if (attackRange == 3f)
            {
                attackRange = 5f;
            }
            //Debug.Log("Attacking Player");
        }
        else
        {
            isInRange = false;
        }
    }

    //1st Phase Attack Pattern
    void BasicCombo()
    {
        animator.SetTrigger("Attack");
    }

    //2nd Phase
    void Moveset()
    {
            int diceRoll = Random.Range(1, 7);  // 10-sided dice roll (1 to 10)
            if (cooldown <= 0)
            {
                if (diceRoll >= 1 && diceRoll < 4)   // roll 1-3 to summon flag
                {
                    SpecialSummonFlag();
                    cooldown = 10f;
                }
                else if (diceRoll >= 4 && diceRoll < 6)  // roll 4-5 to do Special Attack 1
                {
                    animator.SetTrigger("Special1");
                    cooldown = 5f;
                }
                else if (diceRoll == 6)  // roll 6 to do Special Attack 2
                {
                    animator.SetTrigger("Special2");
                    cooldown = 5f;
                }

            }
            else   // roll 7-10 to do normal combo
            {
                cooldown -= Time.deltaTime;
                if (isInRange)
                {
                    BasicCombo();
                }
                else
                {
                    animator.ResetTrigger("Attack");
                }
            }
    }
    void SpecialSummonFlag()
    {
        animator.SetTrigger("Flag");

        // Define the radius around the boss where the flag will be summoned
        float summonRadius = 10f;

        // Generate a random point within a circle of radius summonRadius
        Vector2 randomPoint = Random.insideUnitCircle * summonRadius;

        // Convert the 2D point to 3D by adding it to the boss's position
        Vector3 summonPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);

        // Instantiate the flag at the generated position
        Instantiate(flag, summonPosition, Quaternion.identity);

        Debug.Log("Summoned Flag at position: " + summonPosition);
    }


    public void ResetAllAnimatorTriggers(Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
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
