using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PirateBossAI : MonoBehaviour
{
    [Header("Player values")]
    private Transform playerTransform;

    [Header("Enemy attributes")]
    [SerializeField] private string bossName = "Gideon Undertow the Undead Captain";
    [SerializeField] private float maxHP = 1000f;
    private float currentHP;

    [Header("Melee Attack")]
    public float attackDamage = 40f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private GameObject attackCollider;

    [Header("Ranged Attack")] // damage is managed in a separate script
    [SerializeField] private GameObject rangedPrefab;
    [SerializeField] private Transform attackPoint;

    [Header("Bool checks")]
    [SerializeField] private bool inPhase2 = false; // determine if boss is in second phase
    private bool isAttacking; // prevent 
    private bool isWalking; // to prevent repeat calls for walking animation
    private bool soundPlayed;
    private bool isDead = false;
    private bool hasDroppedLoot;

    [Header("UI")]
    [SerializeField] private SliderBar bossHPBar;
    [SerializeField] private TMP_Text bossNameTxt;

    [Header("VFX/SFX")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private AudioClip hitSoundClip;
    [SerializeField] private AudioSource audioSource;

    [Header("NavMesh/Movement")]
    [SerializeField] private Transform[] movementNodes;
    [SerializeField, Range(0,100)] private float rangedAttackChance = 20f;

    private NavMeshAgent agent;
    private Animator animator; 

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null) Debug.LogError("Player could not be found");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator could not be found.");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent could not be found.");

        // more missing assignment handling
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        if (bossHPBar == null) Debug.LogError("HP bar was not assigned.");
        if (attackCollider == null) Debug.LogError("Attack collider was not assigned.");
        if (movementNodes.Length == 0) Debug.LogWarning("No NavMesh movement nodes were assigned.");

        currentHP = maxHP;
    }

    private void Update()
    {
        if (!isDead)
        {
            if (inPhase2) // behavior in phase 2
            {

            }
            else
            {

            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        yield return null;
    }

    private IEnumerator RangedAttack()
    {
        yield return null;
    }
}
