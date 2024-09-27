using UnityEngine;

public class BasicMovementWithAnimation : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;  // Reference to the Animator component

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Update()
    {
        // Check if the character is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input from the WASD keys
        float x = Input.GetAxis("Horizontal");  // A/D keys
        float z = Input.GetAxis("Vertical");    // W/S keys

        // Calculate movement direction based on player's forward direction (which is controlled by the mouse)
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        controller.Move(move * speed * Time.deltaTime);

        // Set animation based on input
        if (z != 0 || x != 0)  // Moving in any direction
        {
            animator.SetBool("isRunning", true);  // Trigger run animation
        }
        else
        {
            animator.SetBool("isRunning", false); // Stop run animation
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
