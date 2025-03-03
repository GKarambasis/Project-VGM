using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class HealthPotion : MonoBehaviourPunCallbacks
{

    AdventurerDeath AdventurerDeath;
    public int healAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<AdventurerDeath>().Heal(healAmount);
            PhotonNetwork.Destroy(gameObject);   
        }
    }

    
}
