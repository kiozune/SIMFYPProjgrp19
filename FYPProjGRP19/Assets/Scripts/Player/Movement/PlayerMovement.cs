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
    private float speed = 5f;
    [SerializeField]
    private float gravity = -9.8f;
    [SerializeField]
    private Transform characterModel; // The actual model to rotate
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
    private Vector3 mousePoint;


    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }
    private void Start()
    {
    }

    void Update()
    {
        HandleMovement();
        RotateModelToMouse();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        // Move the character based on global axes
        Vector3 move = new Vector3(xAxis, 0, zAxis);
        charController.Move(move * speed * Time.deltaTime);
    }

    void RotateModelToMouse()
    {
        if (Camera.main == null) return;

        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Define the ground plane at y = 0, assuming the ground is parallel to the XZ plane
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // The ground plane is at y = 0
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            // Get the point where the ray intersects with the ground plane
            Vector3 mousePoint = ray.GetPoint(rayLength);

            // Compute the direction from the character to the mouse point
            Vector3 directionToMouse = new Vector3(mousePoint.x, 0, mousePoint.z) - transform.position;

            // Check if the direction is significant
            if (directionToMouse.sqrMagnitude > Mathf.Epsilon)
            {
                // Calculate the target rotation to look at the mouse point
                Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

                // Extract the Yaw (rotation around Y) component from the target rotation
                Vector3 eulerAngles = targetRotation.eulerAngles;
                eulerAngles.x = 0; // Set X to 0 to keep the character upright
                eulerAngles.z = 0; // Set Z to 0 to avoid any tilt

                // Apply the rotation only around the Y-axis
                Quaternion newRotation = Quaternion.Euler(eulerAngles);

                // Smoothly rotate towards the target rotation
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