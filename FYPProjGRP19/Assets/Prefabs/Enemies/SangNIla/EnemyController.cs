using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    
    // Animation state names
    private string standingUpAnim = "StandingUp";  // Ensure this matches your animation name in the Animator

    void Start()
    {
        // Get the Animator component attached to the enemy
        animator = GetComponent<Animator>();

        // Play the default standing up animation
        animator.Play(standingUpAnim);
    }

    void Update()
    {
        // Here you can add more conditions to change animations or behavior later
    }
}
