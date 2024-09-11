using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public float maxHP = 100f;           // Enemy health points
    private float currentHP = 100f;
    public float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit

    private NavMeshAgent agent;       // Reference to the NavMeshAgent component
    private Animator animator;        // Reference to the Animator component
    private Collider parentCollider;  // Reference to the Collider of the parent object
    private SliderBar sliderBar;

    [Header("Enemy EXP")]
    [SerializeField]
    private int experiencePoints = 50;

    void Start()
    {
        // Get the NavMeshAgent component attached to this enemy
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to the enemy.");
        }

        // Get the Animator component attached to this enemy
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is not attached to the enemy.");
        }

        // Get the Collider from the parent object (it should be attached to the same GameObject as this script)
        parentCollider = GetComponent<Collider>();
        if (parentCollider == null)
        {
            Debug.LogError("Parent object collider not found.");
        }

        // Check if the player is assigned
        if (player == null)
        {
            Debug.LogError("Player is not assigned in the EnemyAI script.");
        }

        // Initialize sliderBar if it's attached to this object or elsewhere
        sliderBar = GetComponentInChildren<SliderBar>(); // Or find it in another way depending on its location
        if (sliderBar == null)
        {
            Debug.LogError("SliderBar is not assigned or found.");
        }

        currentHP = maxHP;
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
        }

        if (agent != null && animator != null)
        {
            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;  // Reduce the enemy's HP by the damage amount

        sliderBar.UpdateBar(currentHP, maxHP);

        Debug.Log("Enemy HP: " + currentHP);
    }

    public bool checkHealth()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public int awardEXP()
    {
        return experiencePoints;
    } 
}
