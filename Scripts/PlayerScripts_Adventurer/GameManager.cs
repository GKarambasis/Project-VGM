using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] GameObject adventurerPrefab;
    [SerializeField] GameObject vgmPrefab;

    [Header("Controller References")]
    [SerializeField] GameObject myController;
    [SerializeField] GameObject gameMaster;
    [SerializeField] List<GameObject> adventurers;
    
    [Header("HUDs")]
    [SerializeField] GameObject adventurerHUD;
    [SerializeField] GameObject vgmHUD;
    [SerializeField] GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            loadingScreen.SetActive(true);
            vgmHUD?.SetActive(false);
            adventurerHUD?.SetActive(false);
            StartCoroutine(SpawnPlayerCo());
        }
    }



    IEnumerator SpawnPlayerCo()
    {
        yield return new WaitForSeconds(1);
        
        //Retrieve the Local Player's Team
        int playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["TEAM"];

        //Spawn the Adventurer Controller 
        if(playerTeam == 0)
        {
            myController = PhotonNetwork.Instantiate(adventurerPrefab.name, transform.position, Quaternion.identity); //instantiate the adventurer
            yield return new WaitForSeconds(0.1f);
            SetUpAdventurer();
            loadingScreen.SetActive(false);

            if (PhotonNetwork.IsMasterClient)
            {
                TransferOwnershipToVGM();
            }
        }

        //Spawn the VGM Controller
        else if(playerTeam == 1) 
        {
            myController = PhotonNetwork.Instantiate(vgmPrefab.name, transform.position, Quaternion.identity); //instantiate the VGM
            yield return new WaitForSeconds(0.1f);
            SetUpVGM();
            loadingScreen.SetActive(false);
        }
        
        else { Debug.LogError("Invalid Player Team Number: " + playerTeam.ToString()); }

        yield return new WaitForSeconds(2);

        //adventurers = findobjectsoftype
    }

    private void SetUpAdventurer()
    {
        myController.GetComponent<AdventurerMovement>().enabled = true;

        myController.GetComponentInChildren<Camera>().enabled = true;

        adventurerHUD?.SetActive(true);
    }
    
    private void SetUpVGM()
    {
        myController.GetComponent<VGMInputController>().enabled = true;
        myController.GetComponent<VGMCameraController>().enabled = true;

        Camera[] cameras = FindObjectsOfType<Camera>();

        foreach (var item in cameras)
        {
            if (item != Camera.main)
            {
                item.enabled = false;
            }
        }

        myController.GetComponent<Camera>().enabled = true;

        vgmHUD?.SetActive(true);
    }

    private void TransferOwnershipToVGM()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((int)player.CustomProperties["TEAM"] == 1)
            {
                PhotonNetwork.SetMasterClient(player);
                Debug.LogWarning("Transferring Master Client Status to VGM Player: " + player.NickName);
            }
        }
    }
}
