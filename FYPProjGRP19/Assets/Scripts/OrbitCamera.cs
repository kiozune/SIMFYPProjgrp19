using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    // the orbit target (e.g., the player GameObject)
    [SerializeField] Transform target;

    // rotation sensitivity
    public float rotSpeed = 1.5f;

    private float rotY;     // horizontal rotation
    private Vector3 offset; // offset from the target

    // Start is called before the first frame update
    void Start()
    {
        // get transform component's yaw
        rotY = transform.eulerAngles.y;

        // calculate camera's offset from the target
        offset = target.position - transform.position;
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        // yaw based on horizontal mouse movement
        rotY += Input.GetAxis("Mouse X") * rotSpeed * 3;

        // create quaternion based on rotation angle
        Quaternion rotation = Quaternion.Euler(0, rotY, 0);

        // set the camera's position based on the rotated offset
        transform.position = target.position - (rotation * offset);

        // rotate camera to look at the target
        transform.LookAt(target); // my current game obj to target
    }

    void OnDrawGizmos() //you can fidn whih is front
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
