using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] // enforces dependency on character controller
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] Transform target;  // camera

    // sensitivities
    public float moveSpeed = 6.0f;
    public float rotSpeed = 15.0f;

    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -20.0f;
    private float vertSpeed;

    public float pushForce = 3.0f;

    private CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        // get character controller component
        charController = GetComponent<CharacterController>();

        
    }

    // Update is called once per frame
    void Update()
    {
        // start with zero and add movement components progressively
        Vector3 movement = Vector3.zero;

        // changes based on horizontal and vertical input
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        // if input is not zero
        if (horInput != 0 || vertInput != 0)
        {
            // x z movement transformed relative to target
            Vector3 right = target.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);

            // calculate movement
            movement = (right * horInput) + (forward * vertInput);

            // scale by movement speed
            movement *= moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            // face movement direction
            //transform.rotation = Quaternion.LookRotation(movement);
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                    direction, rotSpeed * Time.deltaTime);
        }

        //movement *= Time.deltaTime;
        //charController.Move(movement);

        if (Input.GetButtonDown("Jump") && charController.isGrounded)
        { Debug.Log("test");
            vertSpeed = jumpSpeed;
        }
        else if (!charController.isGrounded)
        {
            vertSpeed += gravity * 5 * Time.deltaTime;
            if (vertSpeed < terminalVelocity)
            {
                vertSpeed = terminalVelocity;
            }
        }

        movement.y = vertSpeed;
        movement *= Time.deltaTime;
        charController.Move(movement);

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            body.velocity = hit.moveDirection * pushForce;
        }
    }
}
