using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDirEnemy : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;
    [SerializeField]
    private float maxAliveTime = 1.0f;
    [SerializeField]
    private float currTime = 0.0f;
    [SerializeField]
    private GameObject vfxExplosion;

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
            Instantiate<GameObject>(vfxExplosion,transform.position,Quaternion.identity); ;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy Hit!");
        if (other.CompareTag("Obstacles"))
        {
            Instantiate<GameObject>(vfxExplosion, transform.position, Quaternion.identity); ;
            Destroy(this.gameObject);
        }
        else if(other.CompareTag("Player"))
        {
            Instantiate<GameObject>(vfxExplosion, transform.position, Quaternion.identity); ;
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Instantiate<GameObject>(vfxExplosion, transform.position, Quaternion.identity); ;
            Destroy(this.gameObject);
        }
    }
}
