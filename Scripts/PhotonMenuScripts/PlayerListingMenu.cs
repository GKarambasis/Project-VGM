using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [Header("Content Transform")]
    [SerializeField]
    private Transform _content;
    
    [Header("Player Listing Variables")]
    [SerializeField]
    private PlayerListing _playerListing;
    private List<PlayerListing> _listings = new List<PlayerListing>();
    [SerializeField]
    private PlayerTeamManager teamManager;

    [Header("Loading Screens")]
    [SerializeField] GameObject playerErrorScreen;
    [SerializeField] GameObject loadingScreen;

    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for(int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }

        _listings.Clear();

        //Wipe the List
        for (int i = 0; i < teamManager.teamMembers.Length; i++)
        {
            teamManager.teamMembers[i] = 0;
        }
    }

    private void GetCurrentRoomPlayers()
    {
       if (!PhotonNetwork.IsConnected)
       {
            Debug.Log("You are not Connected");
            return;
       }
       if(PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
       {
            Debug.Log("Cannot Find Current Room or Current Room Players");
            return;
       }
       foreach(KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
       {
            AddPlayerListing(playerInfo.Value);
       }
    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
    }

    //When a Player Joins the Room add their name to the player listing
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    //When a player leaves the room remove their name from the player listing
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //ensures that Players are deleted upon leaving
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    

    //Leave Room Button
    public void OnClick_LeaveRoom()
    {
        //Temporary, Should Remove if I use the HashTable More
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom(true);
        
        teamManager.OnClick_LeaveRoomAndTeam();
        

    }
}
