using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
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
    private WeaponAttackController weaponAttack; // Reference to WeaponAttack script

    // New input actions reference
    [SerializeField]
    private PlayerInputAction playerInputActions;
    [SerializeField]
    private Vector2 movementInput;
    [SerializeField]
    private Vector2 lookInput;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        weaponAttack = GetComponent<WeaponAttackController>(); // Get the WeaponAttack component
        playerInputActions = new PlayerInputAction();

        playerInputActions.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Rotation.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        playerInputActions.Player.Movement.canceled += ctx => movementInput = Vector2.zero;
        playerInputActions.Player.Rotation.canceled += ctx => lookInput = Vector2.zero;

    }

    private void OnEnable()
    {
        // Enable input actions
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        // Disable input actions
        playerInputActions.Player.Disable();
    }


    void Update()
    {
        if (weaponAttack.isAttacking)
        {
            return;
        }

        if (!weaponAttack.isAttacking) // Only allow movement if not attacking
        {
            HandleMovement();
        }

        RotateModelWithRightStick();
        ApplyGravity();
    }

    void HandleMovement()
    {
        // Convert movement input to world space
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        charController.Move(move * speed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > Mathf.Epsilon;
        playerAnimController.SetBool("isRunning", isMoving);
    }

    void RotateModelWithRightStick()
    {
        // Use right stick for rotation
        if (lookInput.sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 directionToLook = new Vector3(lookInput.x, 0, lookInput.y);

            if (directionToLook.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                Vector3 eulerAngles = targetRotation.eulerAngles;
                eulerAngles.x = 0;
                eulerAngles.z = 0;
                Quaternion newRotation = Quaternion.Euler(eulerAngles);
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, newRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

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