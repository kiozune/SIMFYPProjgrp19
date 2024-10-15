using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stage2Boss : MonoBehaviour
{
    private Animator animator;
    public GameObject player;
    private NavMeshAgent agent;

    public float detectionRange = 15f;
    public float attackRange = 5f;
    private float chaseTimer = 0;
    private bool isRunning = false;  // To track if the boss is running

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();  // Initialize NavMeshAgent

        // Set the stopping distance for the agent
        agent.stoppingDistance = attackRange;  // Stops when within attack range

        StartCoroutine(FindPlayer());
        animator.SetTrigger("Idle");

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
        if (player != null)
        {
            DetectPlayer();  // Check if player is detected
            InRange();       // Check if player is in attack range
        }


        float speedCheck = agent.speed;
        Debug.Log("Boss Speed is" + speedCheck);
    }

    // Ensure the boss is always facing the player
    void FacePlayer()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;  // Ensure the boss only rotates on the y-axis (no tilting up/down)

            // Calculate the rotation required to face the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Apply the rotation smoothly
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Detect if the player is within detection range
    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isRunning)  // Only trigger walk if not already running
            {
                animator.ResetTrigger("Idle");
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
        agent.speed = 10f;  // Increase speed to running
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
            agent.speed = 3f;  // Reset speed to walk speed
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
            animator.SetTrigger("Attack");

            FacePlayer();
            Debug.Log("Attacking Player");
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
