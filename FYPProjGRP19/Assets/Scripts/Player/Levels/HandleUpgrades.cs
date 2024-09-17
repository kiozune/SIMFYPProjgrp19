using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleUpgrades : MonoBehaviour
{
    [Header("Player attributes Value")]
    [SerializeField]
    private float damageIncrease = 5f;
    [SerializeField]
    private float rangeIncrease = 0.5f;
    [SerializeField]
    private float aoeIncrease = 1f;

    [Header("Player's Game Object")]
    [SerializeField]
    private GameObject player;

    [Header("Player's Upgrade UI")]
    [SerializeField]
    private GameObject upgradeScreen;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void handlePlayerDamageIncrease()
    {
        //Range damage increase
        player.GetComponent<WeaponAttack>().addDamage(damageIncrease);

        //Melee damage increase
        player.GetComponentInChildren<WeaponDamageScript>().upgradeDamage(damageIncrease);
        Time.timeScale = 1f;
        upgradeScreen.SetActive(false);
    }
    public void handlePlayerRangeIncrease()
    {
        player.GetComponent<WeaponDamageScript>().IncreaseMeleeRange(rangeIncrease);
        Time.timeScale = 1f;
        upgradeScreen.SetActive(false);
        player.GetComponent<WeaponAttack>().setAOEBool();
    }

}
