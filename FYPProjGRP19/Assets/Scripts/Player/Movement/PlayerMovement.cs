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
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Assuming the ground is parallel to the XZ plane
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            // Get the point where the ray intersects with the ground plane
            Vector3 mousePoint = ray.GetPoint(rayLength);

            // Compute the direction from the character to the mouse point
            Vector3 directionToMouse = new Vector3(mousePoint.x, transform.position.y, mousePoint.z) - transform.position;

            // If the direction is very small, no need to rotate
            if (directionToMouse.sqrMagnitude > Mathf.Epsilon)
            {
                // Calculate the rotation needed to look at the mouse point
                Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

                // Apply the rotation to the character model
                characterModel.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
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

    //private void ondrawgizmos()
    //{
    //    if (camera.main == null) return;

    //    gizmos.color = color.red;
    //    ray ray = camera.main.screenpointtoray(input.mouseposition);
    //    gizmos.drawray(ray.origin, ray.direction * 100); // draw the raycast for visualization

    //    if (mousepoint != vector3.zero)
    //    {
    //        gizmos.color = color.blue;
    //        gizmos.drawsphere(mousepoint, 0.2f); // draw a sphere at the mouse point for visualization
    //    }
    //}
}