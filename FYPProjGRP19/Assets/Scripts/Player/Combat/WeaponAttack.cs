using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WeaponAttack : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField]
    private Animator playerAnimator;

    [Header("Attachment/Fire Points")]
    [SerializeField]
    private GameObject meleeAttachment;
    [SerializeField]
    private GameObject firePoint;

    [Header("Class Check")]
    [SerializeField]
    private bool meleeWeapon;
    [SerializeField]
    private bool rangeWeapon;

    [Header("Animation Timer")]
    [SerializeField]
    private float attackDuration = 1.10f; // The full duration of the attack
    [SerializeField]
    private float rangedAttackDuration = 0.17f;

    [Header("Ranged controls")]
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float currRate = 0.0f;
    [SerializeField]
    private GameObject projectiles;

    // This will be accessed by PlayerMovement to check if the player is attacking
    public bool isAttacking { get; private set; }

    [SerializeField]
    private bool rangeAttacking = false;

    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
        meleeWeapon = PlayerPrefs.GetInt("Meleewep") == 1;
        rangeWeapon = PlayerPrefs.GetInt("Rangewep") == 1;
    }

    void Update()
    {
        if (meleeWeapon)
        {
            playerAnimator.SetTrigger("melee");
            HandleMeleeAttack();
        }

        if (rangeWeapon)
        {
            playerAnimator.SetTrigger("ranged");
            HandleRangedAttack();
        }
    }

    private void HandleMeleeAttack()
    {
        meleeAttachment.SetActive(true);
        firePoint.SetActive(false);

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformMeleeAttack());
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        // Set isAttacking to true and start the attack animation
        isAttacking = true;
        playerAnimator.SetBool("swordAttack", true);

        // Wait for the full duration of the attack (1.10 seconds)
        yield return new WaitForSeconds(attackDuration);

        // Reset the attack animation and allow movement again
        playerAnimator.SetBool("swordAttack", false);
        isAttacking = false;
    }

    private void HandleRangedAttack()
    {
        meleeAttachment.SetActive(false);
        firePoint.SetActive(true);

        if (Input.GetMouseButton(0) && Time.time > currRate && !rangeAttacking)
        {
            StartCoroutine(PerformRangedAttack());
        }
    }

    private IEnumerator PerformRangedAttack()
    {
        rangeAttacking = true;
        // Set isAttacking to true and start the ranged attack animation
        playerAnimator.SetBool("bowShoot", true);

        // Fire the projectile only once at the start of the animation
        FireProjectile();

        // Wait for the duration of the ranged attack (0.5 seconds)
        yield return new WaitForSeconds(rangedAttackDuration);

        // Reset the ranged attack animation and allow movement again
        playerAnimator.SetBool("bowShoot", false);
        rangeAttacking = false;
        // Set the fire rate timer
        currRate = Time.time + fireRate;

    }

    private void FireProjectile()
    {
        Vector3 shootDirection = firePoint.transform.forward;
        GameObject projectileInstance = Instantiate(projectiles, firePoint.transform.position, Quaternion.LookRotation(shootDirection));
        ProjectileDir projectileDir = projectileInstance.GetComponent<ProjectileDir>();

        if (projectileDir != null)
        {
            projectileDir.SetDirection(shootDirection);
        }
    }
}