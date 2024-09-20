using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Sensitivity for mouse movement
    public Transform playerBody;           // Player body reference for horizontal rotation

    private float xRotation = 0f;          // To store vertical rotation of the camera

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Hide the cursor
    }

    void Update()
    {
        // Get the mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body horizontally (left/right movement)
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (up/down movement)
        xRotation -= mouseY;                      // Invert Y-axis input for natural control
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Clamp rotation to avoid flipping

        // Apply the vertical rotation to the camera (the script should be attached to the camera)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
