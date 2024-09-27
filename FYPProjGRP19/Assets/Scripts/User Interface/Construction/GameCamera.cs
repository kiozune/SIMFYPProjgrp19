using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("First player or object to follow")]
    private Transform target1;  // First target (e.g., player 1)

    [SerializeField]
    [Tooltip("Second player or object to follow")]
    private Transform target2;  // Second target (e.g., player 2)

    [SerializeField]
    [Tooltip("Keep the natural offset above and behind the midpoint between the players")]
    private Vector3 offset = new Vector3(0, 8, -8);  // Offset from the midpoint between players

    [SerializeField]
    [Tooltip("The speed with which the camera follows the players")]
    private float smoothSpeed = 0.125f;  // Smoothing speed for camera movement

    [SerializeField]
    [Tooltip("Minimum field of view (zoomed in)")]
    private float minFOV = 40f;  // Minimum field of view

    [SerializeField]
    [Tooltip("Maximum field of view (zoomed out)")]
    private float maxFOV = 60f;  // Maximum field of view

    [SerializeField]
    [Tooltip("How much the FOV will adjust based on player distance")]
    private float zoomLimiter = 10f;  // Zoom sensitivity based on player distance

    [SerializeField]
    [Tooltip("Layer mask for obstacles")]
    private LayerMask obstacleMask;  // Layer mask for obstacles

    private List<Renderer> currentObstacles = new List<Renderer>();  // List to track current obstacles
    private Camera cam;  // Reference to the Camera component

    private void Start()
    {
        cam = GetComponent<Camera>();  // Get the camera component
    }

    private void LateUpdate()
    {
        if (target1 != null && target2 != null)
        {
            // Calculate the midpoint between the two players
            Vector3 midpoint = (target1.position + target2.position) / 2f;
            Vector3 desiredPosition = midpoint + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Adjust camera's field of view based on the distance between the players
            float distance = Vector3.Distance(target1.position, target2.position);
            float newFOV = Mathf.Lerp(minFOV, maxFOV, distance / zoomLimiter);
            cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);

            // Keep the camera tilt angle
            transform.rotation = Quaternion.Euler(45f, 0, 0);

            // Check for obstacles between the camera and the midpoint
            HandleObstacles(midpoint);
        }
    }

    void HandleObstacles(Vector3 midpoint)
    {
        // Clear the list of obstacles that were transparent in the previous frame
        foreach (Renderer renderer in currentObstacles)
        {
            SetTransparency(renderer, 1f);  // Restore full opacity
        }
        currentObstacles.Clear();

        // Perform raycast to detect obstacles between the camera and the midpoint
        Vector3 direction = midpoint - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, obstacleMask);

        // Make detected obstacles transparent
        foreach (RaycastHit hit in hits)
        {
            Renderer[] renderers = hit.collider.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                SetTransparency(renderer, 0.3f);  // Set transparency level for all renderers in the object
                currentObstacles.Add(renderer);
            }
        }
    }

    void SetTransparency(Renderer renderer, float alpha)
    {
        // Loop through all materials on the object
        foreach (Material material in renderer.materials)
        {
            Color color = material.color;
            color.a = alpha;  // Set alpha transparency
            material.color = color;

            // Ensure the material's rendering mode supports transparency
            if (alpha < 1f)  // If alpha is less than 1, make the material transparent
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else  // Restore material to opaque if alpha is 1 (fully visible)
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
}