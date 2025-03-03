using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Game Version")]
    [SerializeField]
    static string gameVersion = "0.9";

    [Header("Panels")]
    [SerializeField] GameObject loadingPanel; //Loading Screen GameObject to enable when Loading Data
    [SerializeField] GameObject errorPanel;

    [SerializeField]
    GameObject menuPanel; //Main Menu Canvas to Enable when player is in Main Menu
    [SerializeField]
    GameObject roomPanel; //Different Canvas for When Player joins a lobby
    public TextMeshProUGUI roomNameText;

    [Header("Username")]
    [SerializeField] Button submitButton;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] int nickNameInputLenght;

    private MainMenuScript mainMenuScript;
    private PlayerTeamManager playerTeamManager;

    private void Awake()
    {
        //sets scene to automatic
        PhotonNetwork.AutomaticallySyncScene = true;
        mainMenuScript = FindObjectOfType<MainMenuScript>(true);
        playerTeamManager = FindObjectOfType<PlayerTeamManager>(true);
    }

    public void VerifyInputs()
    {
        //If the inputs are above a certain character length, allow submission
        submitButton.interactable = (nickNameField.text.Length >= nickNameInputLenght);
    }

    public void OnClick_PlayButton()
    {
        if (!PlayerData.LoggedIn)
        {
            mainMenuScript.MenuButton(1);
        }
        else
        {
            mainMenuScript.MenuButton(0);
        }
    }
    
    //Called Once the Player OnClick_PlaceObject their Nickname; Connects them to the Photon Server with the chosen Nickname
    public void OnClick_ConnectToLobby()
    {
        if (!PlayerData.LoggedIn)
        {
            PlayerData.username = nickNameField.text;
            PhotonConnect();
            loadingPanel.SetActive(true);
        }
    }
    //Button To Leave Room
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public static void PhotonConnect()
    {
        //Are we connected?
        if (!PhotonNetwork.IsConnected)
        {
            //Track Game Version
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
            //Establish Connection
            PhotonNetwork.ConnectUsingSettings();
            //Give Nickname
            PhotonNetwork.NickName = PlayerData.username;
            Debug.Log("Logging in as: " +  PhotonNetwork.NickName);
        }
        else
        {
            Debug.LogWarning("Already Connected as" + PhotonNetwork.NickName);
        }
    }
    public static void PhotonDisconnect()
    {
        //Are we connected?
        if (PhotonNetwork.IsConnected)
        {
            //Disconnect
            PhotonNetwork.Disconnect();
            Debug.Log("Successful Disconnection");
        }
        else
        {
            Debug.Log("Not Connected");
        }
    }


    //disconnect and provide server info
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + "Server Address: " + PhotonNetwork.ServerAddress);
        
    }
    
    //say that we connected and say what region + join the lobby
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log("Connection made to " + PhotonNetwork.CloudRegion + " Server.");
        Debug.Log("My NIckname is " + PhotonNetwork.LocalPlayer.NickName, this);

        if (loadingPanel.activeInHierarchy)
        {
            loadingPanel.SetActive(false); //Deactivates Loading Screen when Connection to Master Server is accomplished
        }
        
        if (!PhotonNetwork.InLobby)
        {
            //after connecting to server join lobby
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        mainMenuScript.menuPanels[0].SetActive(true);
    }
    //When A player joins a Room, We want to switch between canvases
    public override void OnJoinedRoom()
    {
        menuPanel.SetActive(false); 
        roomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }


    //When leaving a room Switch Canvases Again
    public override void OnLeftRoom()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
        if (roomPanel != null)
        {
            roomPanel.SetActive(false);

        }
    }

    //Start Game Button, Does not Let Anyone But the Master Client to Start the Game
    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != playerTeamManager.teamMembers[0] + playerTeamManager.teamMembers[1])
            {
                if (errorPanel != null && !errorPanel.activeInHierarchy) { errorPanel.SetActive(true); }
                Debug.LogWarning("There are players that have not chosen a Team!");
                return;
            }

            StartCoroutine(StartGame());
        }
        else
        {
            Debug.LogWarning("You are not the Master Client");
            //pop-up error Screen
            if (errorPanel != null && !errorPanel.activeInHierarchy) { errorPanel.SetActive(true); }
        }
    }

    IEnumerator StartGame()
    {
        loadingPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

}
