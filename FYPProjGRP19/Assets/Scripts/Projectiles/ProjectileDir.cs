using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDir : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;
    [SerializeField]
    private float maxAliveTime = 1.0f;
    [SerializeField]
    private float currTime = 0.0f;

    // This function is called to set the direction of the projectile
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        // Move the projectile forward in the specified direction
        transform.position += direction * speed * Time.deltaTime;
        currTime += Time.deltaTime;
         if (currTime > maxAliveTime)
        {
            Destroy(this.gameObject);
        }
    }
}
