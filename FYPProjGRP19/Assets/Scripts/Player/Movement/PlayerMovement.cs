using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
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
    private GameObject groundFloor;
    [SerializeField]
    private float groundCheckDistance = 0.4f;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private bool isGrounded;

    private Vector3 velocity;
    [SerializeField]
    private WeaponAttack weaponAttack; // Reference to WeaponAttack script

    [SerializeField]
    private PlayerInputAction playerInputActions;
    private Vector2 lookInputMouse;
    private Vector2 movementInputKeyboard;


    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        weaponAttack = GetComponent<WeaponAttack>(); // Get the WeaponAttack component
        groundFloor = GameObject.FindGameObjectWithTag("Ground");
        groundCheck = groundFloor.transform;

        playerInputActions = new PlayerInputAction();

        // Mouse input for rotation
        playerInputActions.Player.RotateMouse.performed += ctx => lookInputMouse = ctx.ReadValue<Vector2>();
        playerInputActions.Player.RotateMouse.canceled += ctx => lookInputMouse = Vector2.zero;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.Player.MoveKeyboard.performed += OnMoveKeyboard;
        playerInputActions.Player.MoveKeyboard.canceled += OnMoveKeyboard; // To stop movement
    }

    private void OnMoveKeyboard(InputAction.CallbackContext context)
    {
        movementInputKeyboard = context.ReadValue<Vector2>();
    }
    private void OnDisable()
    {
        playerInputActions.Player.MoveKeyboard.performed -= OnMoveKeyboard;
        playerInputActions.Player.MoveKeyboard.canceled -= OnMoveKeyboard;
        playerInputActions.Disable();
    }


    void Update()
    {
        if (!weaponAttack.isAttacking) // Only allow movement if not attacking
        {
            HandleKeyboardMovement();
        }

        RotateModelWithMouse();
        ApplyGravity();
    }

    // Handle Keyboard Movement
    void HandleKeyboardMovement()
    {
        if (movementInputKeyboard.sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 move = new Vector3(movementInputKeyboard.x, 0, movementInputKeyboard.y);
            charController.Move(move * speed * Time.deltaTime);
            playerAnimController.SetBool("isRunning", true);
        }
        else
        {
            playerAnimController.SetBool("isRunning", false);
        }
    }
    // Rotate model using mouse input
    void RotateModelWithMouse()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
            return;
        }

        // Cast a ray from the camera to the mouse position on the screen
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Horizontal plane (Y-up)
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            // Get the point where the ray intersects the ground plane
            Vector3 mousePoint = ray.GetPoint(rayLength);

            // Calculate the direction to look, keeping the Y-axis (height) zero
            Vector3 directionToLook = new Vector3(mousePoint.x, transform.position.y, mousePoint.z) - transform.position;

            // Ensure the direction is non-zero before rotating
            if (directionToLook.sqrMagnitude > Mathf.Epsilon)
            {
                // Only rotate on the Y-axis (horizontal rotation)
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToLook.x, 0, directionToLook.z));
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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