using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2 : MonoBehaviour
{
    [Header("General Values")]
    [SerializeField]
    private CharacterController charController;
    [SerializeField]
    private Animator playerAnimController;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float gravity = -9.8f;
    [SerializeField]
    private Transform characterModel;
    [Tooltip("Control Value of rotation of the character")]
    public float rotationSpeed = 5f;
    public float mouseSensitivity = 100f; // Mouse sensitivity for look

    [Header("Character Groundcheck")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckDistance = 0.4f;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private bool isGrounded;

    private Vector3 velocity;
    [SerializeField]
    private WeaponAttack weaponAttack; // Reference to WeaponAttack script

    private float xRotation = 0f; // For vertical look rotation

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        weaponAttack = GetComponent<WeaponAttack>(); // Get the WeaponAttack component
    }

    void Update()
    {
        if (!weaponAttack.isAttacking) // Only allow movement if not attacking
        {
            HandleMovement();
        }

        ApplyGravity();
        HandleMouseLook(); // Add mouse look rotation logic
    }

    // Handles WASD movement
    void HandleMovement()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xAxis + transform.forward * zAxis; // Move relative to forward direction
        charController.Move(move * speed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > Mathf.Epsilon;
        playerAnimController.SetBool("isRunning", isMoving);
    }

    // Handles mouse movement (Look left/right)
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body based on mouse X movement
        characterModel.Rotate(Vector3.up * mouseX);

        // Handle vertical look rotation (camera tilt)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical look to avoid flipping

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Handles gravity
    void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }
}
