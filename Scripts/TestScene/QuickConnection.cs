using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConnection : MonoBehaviourPunCallbacks
{
    //Quick Connection Scripts are for Testing Purposes
    [Header("Game Version")]
    [SerializeField]
    public static string gameVersion = "0.9";

    public GameObject loadingScreen;
    public void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            loadingScreen.SetActive(true);
            PhotonConnect();
        }
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
            Debug.Log("Logging in as: " + PhotonNetwork.NickName);
        }
        else
        {
            Debug.LogWarning("Already Connected as" + PhotonNetwork.NickName);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log("Connection made to " + PhotonNetwork.CloudRegion + " Server.");
        Debug.Log("My NIckname is " + PhotonNetwork.LocalPlayer.NickName, this);

        if (!PhotonNetwork.InLobby)
        {
            //after connecting to server join lobby
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.LogWarning("No Room Found, Creating one");
        CreateRoom();
    }

    public void CreateRoom()
    {

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("You are not Connected to Photon");
            return;
        }
        int roomName = Random.Range(0, 1000);
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = (byte)8;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.CreateRoom(roomName.ToString(), options, TypedLobby.Default);

    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Successfully");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room Successfully");
        loadingScreen.SetActive(false);

    }

}
