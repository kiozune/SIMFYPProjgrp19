using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FutureBoss : MonoBehaviour
{
    private Animator animator;
    public GameObject player;
    private NavMeshAgent agent;
    private EnemyHP bossHP;

    public float detectionRange = 15f;
    public float attackRange = 5f;

    private float distanceToPlayer;
    private string currentAnimation = "";

    private bool inRange = false;
    private bool isAttacking = false;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    //Initialize Animator
        agent = GetComponent<NavMeshAgent>();  // Initialize NavMeshAgent
        bossHP = GetComponent<EnemyHP>();       //Get HP elements

        agent.updateRotation = false;

        StartCoroutine(FindPlayer());

        ChangeAnimation("Idle");
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

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
        CheckAnimation();

        if (bossHP.GetCurrentHealth() <= 0)
        {
            HandleDeath();
        }
        if (isAttacking && inRange)
        {
            ChangeAnimation("Attack");
        }
    }

    //check animation played
    private void CheckAnimation()
    {
        if (isDead)
        {
            ChangeAnimation("Defeat");
            return;
        }
        else if (isAttacking)
        {
            agent.SetDestination(player.transform.position);
            FacePlayer();
            agent.ResetPath();
        }
        else if (inRange)
        {
            ChangeAnimation("Walk");
            agent.SetDestination(player.transform.position);
            FacePlayer();
            agent.speed = 3.5f;
            attackRange = 5f;
        }
        else
        {
            ChangeAnimation("Idle");
        }
    }

    public void HandleDeath()
    {
        // Stop enemy movement and handle death logic
        Debug.Log("Death Triggered");
        agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        /*
        inRange = false;
        isAttacking = false;
        isPhase = false;
        isChasing = false;
        isDead = true;
        */
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
