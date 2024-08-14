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

    [SerializeField]
    [Tooltip("Layer mask for obstacles")]
    private LayerMask obstacleMask; // Layer mask for obstacles

    private List<Renderer> currentObstacles = new List<Renderer>(); // List to track current obstacles

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.rotation = Quaternion.Euler(tiltAngle, 0, 0); // Set the tilt angle

            // Check for obstacles between the camera and the target
            HandleObstacles();
        }
    }

    void HandleObstacles()
    {
        // Clear the list of obstacles that were transparent in the previous frame
        foreach (Renderer renderer in currentObstacles)
        {
            SetTransparency(renderer, 1f); // Restore full opacity
        }
        currentObstacles.Clear();

        // Perform raycast to detect obstacles within the specified layers
        RaycastHit[] hits;
        Vector3 direction = target.position - transform.position;
        hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, obstacleMask);

        // Make detected obstacles transparent
        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetTransparency(renderer, 0.3f); // Set transparency level
                currentObstacles.Add(renderer);
            }
        }
    }

    void SetTransparency(Renderer renderer, float alpha)
    {
        Material material = renderer.material;
        Color color = material.color;
        color.a = alpha;
        material.color = color;

        // Ensure the material's rendering mode supports transparency
        if (alpha < 1f)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }
}