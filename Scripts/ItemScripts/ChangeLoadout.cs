using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChangeLoadout : MonoBehaviourPunCallbacks
{
    [SerializeField] RuntimeAnimatorController controllerSword;
    [SerializeField] RuntimeAnimatorController controllerShield;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().IsMine)
            {
                int photonViewID = other.gameObject.GetPhotonView().ViewID;
                //photonView.RPC("Swap", RpcTarget.All, other.gameObject);
                photonView.RPC("Swap", RpcTarget.All, photonViewID);
            }
        }
    }
    [PunRPC]
    void Swap(int viewID)
    {
        GameObject player = PhotonNetwork.GetPhotonView(viewID).gameObject;

        Animator animator = player.GetComponentInChildren<Animator>();

        if (animator.runtimeAnimatorController == controllerShield)
        {
            EnableWeapon(player);
            animator.runtimeAnimatorController = controllerSword;
            Debug.Log("Changing to " + controllerSword.name);

            if (photonView.IsMine) { PhotonNetwork.Destroy(gameObject); }
        }


        else if (animator.runtimeAnimatorController == controllerSword)
        {
            EnableWeapon(player);
            animator.runtimeAnimatorController = controllerShield;
            Debug.Log("Changing to " + controllerShield.name);

            if (photonView.IsMine) { PhotonNetwork.Destroy(gameObject); }
        }

        else
        {
            Debug.LogError("Unidentified Animator Controller found");
            if (photonView.IsMine) { PhotonNetwork.Destroy(gameObject); }
        }
    }



    private void EnableWeapon(GameObject player)
    {
        AdventurerWeapon[] weaponScripts = player.GetComponentsInChildren<AdventurerWeapon>(true);

        foreach (AdventurerWeapon weapon in weaponScripts)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                weapon.ShowShield(false);
                weapon.gameObject.SetActive(false);
            }
            else
            {
                weapon.gameObject.SetActive(true);
                weapon.ShowShield(true);
            }
        }
    }
    
}
