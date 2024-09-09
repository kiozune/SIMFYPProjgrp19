using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        RotateModelToMouse();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(xAxis, 0, zAxis);
        charController.Move(move * speed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > Mathf.Epsilon;
        playerAnimController.SetBool("isRunning", isMoving);
    }

    void RotateModelToMouse()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 mousePoint = ray.GetPoint(rayLength);
            Vector3 directionToMouse = new Vector3(mousePoint.x, 0, mousePoint.z) - transform.position;

            if (directionToMouse.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
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