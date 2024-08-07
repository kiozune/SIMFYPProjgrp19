using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Camera will follow the player")]
    private Transform target; // The player
    [SerializeField]
    [Tooltip("Keep the natural offset above and behind the player")]
    private Vector3 offset = new Vector3(0, 8, -8); // Offset to keep the camera above and behind the player
    [SerializeField]
    [Tooltip("The speed with which the camera follows the player")]
    private float smoothSpeed = 0.125f; // Smoothing speed for camera movement
    [SerializeField]
    [Tooltip("The angle at which the camera looks down at the player")]
    private float tiltAngle = 45f; // Angle to tilt the camera

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.rotation = Quaternion.Euler(tiltAngle, 0, 0); // Set the tilt angle
        }
    }
}