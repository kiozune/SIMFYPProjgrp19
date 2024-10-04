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
        if (Camera.main == null) return; // Ensure there's a main camera

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 mousePoint = ray.GetPoint(rayLength);
            Vector3 directionToLook = new Vector3(mousePoint.x, 0, mousePoint.z) - transform.position;

            if (directionToLook.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
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