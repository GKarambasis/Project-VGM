using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventurerDeath : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public bool invulnerable = false;
    public int health = 100;
    public int maxHealth = 100;

    [Header("SFX")]
    [SerializeField] AudioClip hitSFX;

    [Header("VFX")]
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject deathVFX;

    Slider healthBar;

    Animator animator;
    Rigidbody rb;
    CapsuleCollider Collider;
    AudioSource audioSource;
    AdventurerMovement adventurerMovement;
    AdventurerAttack adventurerAttack;

    // Start is called before the first frame update
    void Start()
    {
        adventurerMovement = GetComponent<AdventurerMovement>();
        adventurerAttack = GetComponent<AdventurerAttack>();
        audioSource = GetComponent<AudioSource>();
        
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        if (photonView.IsMine)
        {
            healthBar = FindObjectOfType<HPBar>(true).GetComponent<Slider>();
        }
    }

    public void Hit(int damage)
    {
        if (!invulnerable && photonView.IsMine)
        {

            if (health > damage)
            {
                //Health Update
                health -= damage;
                
                photonView.RPC("InvokeHit", RpcTarget.Others, health);
                
                //SFX
                audioSource.clip = hitSFX;
                audioSource.Play();

                //VFX
                if (hitVFX) { Instantiate(hitVFX, transform); }

                //Animation
                animator.SetTrigger("Hit");
              

                if (photonView.IsMine)
                {
                    healthBar.value = health;
                }

            }
            else if (health <= damage)
            {
                photonView.RPC("Die", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void InvokeHit(int newHealth)
    {
        //Health Update
        health = newHealth;

        //SFX
        audioSource.clip = hitSFX;
        audioSource.Play();

        //VFX 
        if (hitVFX) { Instantiate(hitVFX, transform); }
        
        //Animation
        animator.SetTrigger("Hit");
    }

    [PunRPC]
    public void Die()
    {
        //SFX
        audioSource.clip = hitSFX;
        audioSource.Play();

        //VFX 
        if (deathVFX) { Instantiate(deathVFX, transform); }

        if (photonView.IsMine)
        {         
            photonView.RPC("ToggleInvulnerability", RpcTarget.All, true);
            healthBar.value = 0;
            StartCoroutine(Respawn());
            Debug.LogWarning("You are Dead");
        }       
    }


    IEnumerator Respawn()
    {
        invulnerable = true;
        
        healthBar.value = 0;
        //Enable Ragdoll
        photonView.RPC("EnableAllActions", RpcTarget.All, false);        
        
        yield return new WaitForSeconds(5);
        transform.position = new Vector3(30, 4, 69);
        
        
        health = 100;
        healthBar.value = health;

        photonView.RPC("InvokeHit", RpcTarget.Others, health);

        photonView.RPC("EnableAllActions", RpcTarget.All, true);



        yield return new WaitForSeconds(1);
        //Disable Ragdoll
        photonView.RPC("ToggleInvulnerability", RpcTarget.All, false);
    }
    [PunRPC]
    void EnableAllActions(bool state)
    {
        //Disables animator controllers and colliders and allows the player to ragdoll
        if (photonView.IsMine)
        {
            adventurerAttack.enabled = state;
            adventurerMovement.enabled = state;
        }
        animator.enabled = state;
    }

    [PunRPC]
    void ToggleInvulnerability(bool state)
    {
        invulnerable = state;
    }

    public void Heal(int amount)
    {
        photonView.RPC("InvokeHeal", RpcTarget.All, amount);
    }
    [PunRPC]
    void InvokeHeal(int amount)
    {
        health += amount;

        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if (photonView.IsMine)
        {
            healthBar.value = health;
        }

    }

}
