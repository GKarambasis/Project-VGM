using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerWeapon : MonoBehaviour
{
    BoxCollider myCollider;
    [SerializeField] GameObject shield;

    [Header("Weapon Settings")]
    public int damage = 25;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        
        if(other is CapsuleCollider)
        {
            if (other.gameObject.GetComponent<NPCDeath>() != null)
            {
                //Disable the collider if it impacts so it doesn't hit the enemy twice during the attack
                myCollider.enabled = false;
                other.gameObject.GetComponent<NPCDeath>().Hit(damage, other.ClosestPoint(transform.position));
            }
        }
    }

    public void ShowShield(bool state)
    {
        if (shield != null) { shield.SetActive(state); }
    }
}
