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
    [SerializeField]
    private bool magicWeapon;

    [Header("Animation Timer")]
    [SerializeField]
    private float currTime = 0;
    [SerializeField]
    private float maxTime = 0.25f;

    [Header("Ranged controls")]
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float currRate = 0.0f;
    [SerializeField]
    private GameObject projectiles;



    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
        //taking in player prefs from main menu
        meleeWeapon = PlayerPrefs.GetInt("Meleewep") == 1;
        rangeWeapon = PlayerPrefs.GetInt("Rangewep") == 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (meleeWeapon)
        {
            meleeAttachment.SetActive(true);
            firePoint.SetActive(false);
            currTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.SetTrigger("meleeAttack");
            }
            if (currTime > maxTime)
            {
                playerAnimator.SetTrigger("resetToIdle");
                currTime = 0;

            }
        }
        if (rangeWeapon)
        {
            meleeAttachment.SetActive(false);
            firePoint.SetActive(true);
            Vector3 shootDirection = firePoint.transform.forward;

            if (Input.GetMouseButton(0) && Time.time > currRate)
            {
                currRate = Time.time + fireRate;

                GameObject projectileInstance = Instantiate(projectiles, firePoint.transform.position, Quaternion.LookRotation(shootDirection));
                ProjectileDir projectileDir = projectileInstance.GetComponent<ProjectileDir>();

                if (projectileDir != null)
                {
                    projectileDir.SetDirection(shootDirection);
                    Debug.Log(shootDirection);
                }
            }
        }
    }
}