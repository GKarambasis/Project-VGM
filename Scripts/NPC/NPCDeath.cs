using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class NPCDeath : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public bool invulnerable = false;
    public int health = 100;

    [Header("SFX")]
    public AudioClip deathSFX;
    public AudioClip hitSFX;

    [Header("VFX")]
    public GameObject deathVFX;
    public GameObject hitVFX;

    AudioSource audioSource;
    Animator animator;
    Rigidbody rb;
    CapsuleCollider Collider;
    SphereCollider sphereCollider;
    Rigidbody[] ragdollBodies;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
    }

    public void Hit(int damage)
    {
        if (invulnerable)
        {
            return;
        }

        if(health > damage)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("Hit");
            }
            
            //update the new health
            health -= damage;
            photonView.RPC("InvokeHit", RpcTarget.Others, health);


        }
        else
        {
            Die();
        }
    }
    public void Hit(int damage, Vector3 forceDirection)
    {
        if (invulnerable)
        {
            return;
        }

        if (health > damage)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("Hit");
            }

            //VFX
            if (hitVFX) { Instantiate(hitVFX, transform); }
            //SFX
            audioSource.clip = hitSFX;
            audioSource.Play();
            //update the new health
            health -= damage;
            photonView.RPC("InvokeHit", RpcTarget.Others, health);


        }
        else
        {
            Die();
            photonView.RPC("ApplyRagdollForce", RpcTarget.All, forceDirection, (float)damage / 1.5f);

        }
    }

    [PunRPC]
    void ApplyRagdollForce(Vector3 collisionPoint, float forceIntensity)
    {
        Vector3 forceDirection = (transform.position - collisionPoint).normalized;

        foreach (Rigidbody rb in ragdollBodies)
        {

            rb.AddForce(forceDirection * forceIntensity, ForceMode.Impulse); // Adjust force value as needed
        }
    }

    [PunRPC]
    void InvokeHit(int newHealth)
    {
        health = newHealth;
        animator.SetTrigger("Hit");

        //VFX
        if (hitVFX) { Instantiate(hitVFX, transform); }

        audioSource.clip = hitSFX;
        audioSource.Play();
    }

    public void Die()
    {
        //VFX
        if (deathVFX) { Instantiate(deathVFX, transform); }

        //SFX
        audioSource.clip = deathSFX;
        audioSource.Play();

        photonView.RPC("InvokeDie", RpcTarget.Others);
        DisableAllActions();

        //Freeze rigidbody XYZ axis to prevent body from falling from the world
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Collider.enabled = false;
        sphereCollider.enabled = false;
        animator.enabled = false;

    }


    [PunRPC]
    void InvokeDie()
    {
        //VFX
        if (deathVFX) {Instantiate(deathVFX, transform);}

        //SFX
        audioSource.clip = deathSFX;
        audioSource.Play();
        
        DisableAllActions();

        //Freeze rigidbody XYZ axis to prevent body from falling from the world
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Collider.enabled = false;
        sphereCollider.enabled = false;
        animator.enabled = false;
    }

    void DisableAllActions()
    {
        if (GetComponent<NPCAttack>() != null)
        {
            gameObject.GetComponent<NPCAttack>().enabled = false;
        }
        gameObject.GetComponent<NPCController>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
    }
}
