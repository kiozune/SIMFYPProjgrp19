using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CannonAI : MonoBehaviour
{
    private Transform playerTransform;
    private Animation anim;

    [SerializeField] private float cooldown = 10f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform attackPoint;

    private bool hasRisen;
    private bool isAttacking;

    void Start()
    {
        /*playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null) Debug.LogError("Player could not be found");*/

        anim = GetComponent<Animation>();
        if (anim == null) Debug.LogError("Animator could not be found.");

        if (projectilePrefab == null) Debug.LogError("Projectile prefab could not be found.");
         
        hasRisen = false; 
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) playerTransform = GameObject.FindWithTag("Player").transform;

        if (!isAttacking)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        for (float i = 0; i <= cooldown; i += Time.deltaTime)
        { 
            LookAtPlayer();
            yield return null;
        }

        GameObject projectile = Instantiate(projectilePrefab,
            attackPoint.position, Quaternion.LookRotation(playerTransform.position));
        ProjectileDirEnemy projectileDir = projectile.GetComponent<ProjectileDirEnemy>();

        if (projectileDir != null)
        {
            Vector3 targetPos = playerTransform.position;
            Vector3 throwDirection = CalculateThrowDirection(attackPoint.position, targetPos);

            projectileDir.SetDirection(throwDirection);
        }

        isAttacking = false;
    }

    private void LookAtPlayer()
    {
        Vector3 lookAtPos = playerTransform.position - transform.position;
        lookAtPos.y = 0; // prevent looking up or down
        Quaternion rotation = Quaternion.LookRotation(lookAtPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
    }

    // Simplified throw direction calculation with an arc
    private Vector3 CalculateThrowDirection(Vector3 start, Vector3 target)
    {
        // Get the direction to the player
        Vector3 direction = (target - start).normalized;

        // Give it an upward arc
        direction.y = 0.5f;

        return direction;
    }
}
