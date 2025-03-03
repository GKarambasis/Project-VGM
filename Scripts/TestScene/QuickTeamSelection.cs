using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuickTeamSelection : MonoBehaviourPunCallbacks
{
    //Quick Connection Scripts are for Testing Purposes
    [Header("Controller: True for VGM, False for Adventurer")]
    public bool isVGM;
    GameObject myController;

    [Header("Player Prefabs")]
    [SerializeField] GameObject adventurerPrefab;
    [SerializeField] GameObject vgmPrefab;

    [Header("Player HUDs")]
    [SerializeField] GameObject adventurerHUD;
    [SerializeField] GameObject vgmHUD;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        StartCoroutine(AssignController());

        vgmHUD?.SetActive(false);
        adventurerHUD?.SetActive(false);
    }

    IEnumerator AssignController()
    {
        yield return new WaitForSeconds(3);

        if (isVGM)
        {
            //Instantiate the VGM prefab
            myController = PhotonNetwork.Instantiate(vgmPrefab.name, transform.position, Quaternion.identity); //instantiate the VGM
            yield return new WaitForSeconds(0.1f);
            SetUpVGM();
        }
        else
        {
            //Instantiate the Adventurer Prefab
            myController = PhotonNetwork.Instantiate(adventurerPrefab.name, transform.position, Quaternion.identity); //instantiate the Adventurer
            yield return new WaitForSeconds(0.1f);
            SetUpAdventurer();
        }
    }
    private void SetUpAdventurer()
    {
        //Enable the player movement script
        myController.GetComponent<AdventurerMovement>().enabled = true;
        //Enable the player camera
        myController.GetComponentInChildren<Camera>().enabled = true;
        //Enable the correct HUD
        adventurerHUD?.SetActive(true);
    }

    private void SetUpVGM()
    {
        //Enable the VGM input script
        myController.GetComponent<VGMInputController>().enabled = true;
        //Enable the VGM movement script
        myController.GetComponent<VGMCameraController>().enabled = true;

        //Disable all other cameras that are not the main one
        Camera[] cameras = FindObjectsOfType<Camera>();

        foreach (var item in cameras)
        {
            if(item != Camera.main)
            {
                item.enabled = false;
            }
        }
        
        //Enable the vgm camera
        myController.GetComponent<Camera>().enabled = true;

        //Enable the VGM HUD
        vgmHUD?.SetActive(true);
    }
}
