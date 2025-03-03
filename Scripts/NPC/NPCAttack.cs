using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
[RequireComponent(typeof(NavMeshAgent))]

public class NPCAttack : MonoBehaviourPunCallbacks
{
    //This Class controls the movement and attack pattern of the NPC
    
    [Header("Active")]
    public bool isActive;

    [Header("Attack Settings")]
    public float scanRadius = 10f;          // Radius to scan for players
    public float attackRange = 2f;          // Range within which the NPC attacks
    public float attackCooldown = 2f;       // Cooldown time between attacks
    public float rotationSpeed = 5f;

    private Transform targetPlayer;         // Current player target
    private NavMeshAgent agent;             // NavMeshAgent component for movement
    private float lastAttackTime = 0f;      // Time since the last attack

    private Collider weaponCollider;

    private Animator animator;
    private NPCController npcController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        npcController = GetComponent<NPCController>();

        if (GetComponentInChildren<NPCWeapon>() != null)
        {
            weaponCollider = GetComponentInChildren<NPCWeapon>().gameObject.GetComponent<Collider>();
        }

    }

    void Update()
    {
        if (isActive)
        {

            ScanForPlayers();

            if (targetPlayer != null)
            {
                MoveToTarget();

                if (Vector3.Distance(transform.position, targetPlayer.position) <= attackRange)
                {
                    AttackPlayer();
                }
            }

            UpdateAnimations();
        }

    }

    // Scans for players in the area based on Player tag
    void ScanForPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Find all game objects with Player tag
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity; // Start with a very large distance

        foreach (GameObject player in players)
        {
            if (!player.GetComponent<AdventurerDeath>().invulnerable)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= scanRadius && distanceToPlayer < closestDistance)
                {
                    closestPlayer = player.transform;
                    closestDistance = distanceToPlayer;
                }
            }
        }

        targetPlayer = closestPlayer; // Assign closest player, or null if none found
    }

    // Move towards the closest player
    void MoveToTarget()
    {
        if (targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.position);
        }
    }

    // Attack the player if within range
    void AttackPlayer()
    {
        //Look at enemy
        Vector3 directionToTarget = (targetPlayer.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Attack logic here, like reducing player's health
            //Debug.Log("Attacking the player!");
            animator.SetTrigger("Attack1");

            StartCoroutine(PerformAttack());

            lastAttackTime = Time.time; // Update last attack time

        }

    }

    // Visualize the scan radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }

    void UpdateAnimations()
    {
        //animator.SetFloat("HorizontalSpeed", agent.velocity.magnitude);
        animator.SetFloat("VerticalSpeed", agent.velocity.magnitude);
    }

    public void AttackState(bool state)
    {
        isActive = state;
        photonView.RPC("InvokeAttackState", RpcTarget.Others, state);
    }
    [PunRPC]
    void InvokeAttackState(bool state)
    {
        isActive = state;
    }

    IEnumerator PerformAttack()
    {
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(1);

        weaponCollider.enabled = false;
    }
}
