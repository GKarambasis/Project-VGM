using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeapon : MonoBehaviour
{
    BoxCollider myCollider;
    [Header("Weapon Settings")]
    public int damage = 25;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    //When the weapon collider detects an object with the player tag, get its AdventurerDeath script and do damage to them
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<AdventurerDeath>() != null)
            {
                other.gameObject.GetComponent<AdventurerDeath>().Hit(damage);
                myCollider.enabled = false;
            }
        }
    }
}
