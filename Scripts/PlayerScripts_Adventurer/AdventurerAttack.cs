using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AdventurerAttack : MonoBehaviourPunCallbacks
{

    Animator animator;
    AdventurerWeapon[] weaponScripts;

    [Header("Stamina Settings")]
    public float stamina;
    public float maxStamina;
    public float staminaRefillDelay;
    public float staminaRefillRate;
    public float staminaCost;
    private Slider staminaBar;
    

    [Header("Animation Settings")]
    public float comboTimeout = 1.0f; // Time within which the player can perform the next attack

    public float weaponActivationDelay;
    public float weaponDeactivationDelay;
    
    private int comboStep = 0;
    private float lastAttackTime;
    bool isAttacking = false;


    private void Start()
    {
        weaponScripts = GetComponentsInChildren<AdventurerWeapon>(true);
        animator = GetComponentInChildren(typeof(Animator)) as Animator;

        if (photonView.IsMine)
        {
            staminaBar = FindObjectOfType<StaminaBar>(true).GetComponent<Slider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                HandleAttackInput();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                //Block(); Removed Feature
            }

            RefillStaminaBar();
            UpdateStaminaBar(stamina);
        }
    }
    

    void HandleAttackInput()
    {
        if (Time.time - lastAttackTime > comboTimeout)
        {
            // Reset combo if too much time has passed
            comboStep = 0;
        }

        if (!isAttacking)
        {
            if(stamina - staminaCost >= 0)
            {
                // Start the combo attack
                StartCoroutine(PerformAttack());
            }
        }
        else if (comboStep == 1)
        {
            // Allow for second attack during combo
            comboStep = 2;
        }   
    }

    private IEnumerator PerformAttack()
    {
        stamina -= staminaCost;
        
        Collider weaponCollider = FindWeaponCollider();
        isAttacking = true;

        if (comboStep == 0)
        {
            animator.SetTrigger("Attack1");
            photonView.RPC("Attack1", RpcTarget.Others);
            comboStep = 1;  // Set to expect next combo
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("Attack2");
            photonView.RPC("Attack2", RpcTarget.Others);
            comboStep = 0;  // Reset combo after second attack
        }
        
        yield return new WaitForSeconds(weaponActivationDelay);
        weaponCollider.enabled = true;

        // Simulate the time duration of the attack animations
        yield return new WaitForSeconds(weaponDeactivationDelay);

        isAttacking = false;
        weaponCollider.enabled = false;
        lastAttackTime = Time.time;
    }

    [PunRPC]
    void Attack1()
    {
        animator.SetTrigger("Attack1");
    }
    [PunRPC]
    void Attack2()
    {
        animator.SetTrigger("Attack2");

    }

    //Finds the weapon collider of the currently active 
    Collider FindWeaponCollider()
    {
        foreach (AdventurerWeapon weaponScript in weaponScripts)
        {
            if (weaponScript.gameObject.activeInHierarchy)
            {
                return weaponScript.GetComponent<Collider>();
            }
           
        }

        Debug.LogWarning("No Acive Weapon Found, Cannot Attack");
        
        return null;
    }

    void RefillStaminaBar()
    {
        if (Time.time - lastAttackTime > staminaRefillDelay)
        {
            if(stamina < maxStamina)
            {
                stamina += staminaRefillRate;
            }
            else if (stamina > maxStamina) 
            {
                stamina = maxStamina;
            }
        }
    }

    void UpdateStaminaBar(float Stamina)
    {
        staminaBar.value = Stamina;
    }
}
