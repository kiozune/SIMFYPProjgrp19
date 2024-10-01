using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField]
    private Animator playerAnimator;

    [Header("Attachment/Fire Points")]
    [SerializeField]
    private GameObject meleeAttachment;
    [SerializeField]
    private GameObject bowPrefab;
    [SerializeField]
    private GameObject firePoint;

    [Header("Class Check")]
    [SerializeField]
    private bool meleeWeapon;
    [SerializeField]
    private bool rangeWeapon;

    [Header("Animation Timer")]
    [SerializeField]
    private float attackDuration = 0.5f; // The full duration of the attack
    [SerializeField]
    private float rangedAttackDuration = 0.17f;

    [Header("Ranged controls")]
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float currRate = 0.0f;
    [SerializeField]
    private float bonusAOEExplosionRange = 0.5f;
    [SerializeField]
    private GameObject projectiles;

    [Header("Melee Weapon")]
    [SerializeField]
    private GameObject meleeWeaponObject;
    [SerializeField]
    private float damage = 55f;
    [SerializeField]
    private WeaponDamageScript wepDamageScript;
    [Header("Melee Cooldown")]
    [SerializeField]
    private float meleeCooldown = 1.0f; // Cooldown duration for melee attacks
    private bool meleeOnCooldown = false; // Track cooldown state

    // This will be accessed by PlayerMovement to check if the player is attacking
    public bool isAttacking { get; private set; }

    [SerializeField]
    private bool rangeAttacking = false;

    [SerializeField]
    private bool enableAOE = false;

    private PlayerInputAction playerInputActions;

    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
        meleeWeapon = PlayerPrefs.GetInt("Meleewep") == 1;
        rangeWeapon = PlayerPrefs.GetInt("Rangewep") == 1;

        playerInputActions = new PlayerInputAction();
        playerInputActions.Enable(); // Enable input actions

        // Use the right trigger for attack
        playerInputActions.Player.Attack.performed += ctx => HandleAttack();
    }

    void OnDisable()
    {
        playerInputActions.Player.Attack.performed -= ctx => HandleAttack();
        playerInputActions.Disable(); // Disable input actions
    }

    void Update()
    {
        if (wepDamageScript == null)
        {
            wepDamageScript = gameObject.GetComponent<WeaponDamageScript>();
        }
        if (meleeWeapon)
        {
            playerAnimator.SetTrigger("melee");
            meleeAttachment.SetActive(true);
            firePoint.SetActive(false);
            bowPrefab.SetActive(false);
        }

        if (rangeWeapon)
        {
            playerAnimator.SetTrigger("ranged");
            meleeAttachment.SetActive(false);
            firePoint.SetActive(true);
            bowPrefab.SetActive(true);
        }
    }

    private void HandleAttack()
    {
        // Check if melee or ranged weapon is equipped
        if (meleeWeapon && !isAttacking && !meleeOnCooldown)
        {
            StartCoroutine(PerformMeleeAttack());
        }
        else if (rangeWeapon && !rangeAttacking && Time.time > currRate)
        {
            SoundManager.Instance.PlayShootSound();
            StartCoroutine(PerformRangedAttack());
        }
    }


    private IEnumerator PerformMeleeAttack()
    {
        isAttacking = true; // Prevent further attacks until this one is done
        meleeOnCooldown = true; // Start cooldown for melee attack

        playerAnimator.SetBool("swordAttack", true);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        wepDamageScript.setBoolHit(true);

        // Wait for the full duration of the attack
        yield return new WaitForSeconds(attackDuration);

        // Reset the attack animation and allow movement again
        playerAnimator.SetBool("swordAttack", false);
        isAttacking = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        wepDamageScript.setBoolHit(false);

        // Wait for the melee cooldown duration
        yield return new WaitForSeconds(meleeCooldown);
        meleeOnCooldown = false; // Reset cooldown state
    }

    private void HandleRangedAttack()
    {
        meleeAttachment.SetActive(false);
        firePoint.SetActive(true);
        bowPrefab.SetActive(true);

        SoundManager.Instance.PlayShootSound();
        StartCoroutine(PerformRangedAttack());
    }

    private IEnumerator PerformRangedAttack()
    {
        rangeAttacking = true;

        // Set isAttacking to true and start the ranged attack animation
        playerAnimator.SetBool("bowShoot", true);

        // Fire the projectile only once at the start of the animation
        FireProjectile();

        // Wait for the duration of the ranged attack
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
        projectileDamage projectileDamage = projectileInstance.GetComponent<projectileDamage>();
        projectileDamage.setDamage(damage);
        projectileDamage.setAOEEnabled(enableAOE, bonusAOEExplosionRange);

        if (projectileDir != null)
        {
            projectileDir.SetDirection(shootDirection);
        }
    }

    // Getters and setters for damage
    public float returnDamageValue()
    {
        return damage;
    }
    public void addDamage(float damageValue)
    {
        damage += damageValue;
    }
    public void setAOEBool()
    {
        enableAOE = true;
    }
}