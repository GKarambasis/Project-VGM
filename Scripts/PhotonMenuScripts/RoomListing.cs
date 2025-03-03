using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _roomNameText, _roomSizeText;

    [SerializeField]
    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomNameText.text = roomInfo.Name;
        _roomSizeText.text = roomInfo.PlayerCount.ToString()+ " / " + roomInfo.MaxPlayers.ToString();
    }

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(_roomNameText.text);
        
    }
}
