using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI _roomName;

    public void OnClick_CreateRoom()
    {
        
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("You are not Connected to Photon");
            return;
        }
        
        //CreateRoom
        if(_roomName != null)
        {
            RoomOptions options = new RoomOptions();
            options.BroadcastPropsChangeToAll = true;
            options.MaxPlayers = (byte)5;
            options.CleanupCacheOnLeave = true;
            PhotonNetwork.CreateRoom(_roomName.text, options, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Input A Valid Room Name");
        }
        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed To Create Room" + message);
    }
}
