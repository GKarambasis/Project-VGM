using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System;
using System.Reflection;
using TMPro;
using Photon.Realtime;


public class PlayerTeamManager : MonoBehaviourPunCallbacks
{
    private ExitGames.Client.Photon.Hashtable _playerProperties = new ExitGames.Client.Photon.Hashtable();

    [SerializeField]
    public int[] teamMembers = new int[2];
    public int[] teamSize = new int[2];

    [Header("All Available Room Listings")]
    [SerializeField] RoomListing[] roomListings;

    [SerializeField]
    private TextMeshProUGUI[] teamTexts;
    //check that the team doesn't have more than two players
    //add player to team
    //increase the amount of players in team
    //if player leaves, remove the number of members in the team 


    private void Update()
    {
        for (int i = 0; i < teamTexts.Length; i++)
        {
            teamTexts[i].text = teamMembers[i].ToString() + " / " + teamSize[i].ToString();
        }

    }

    //Player clicks on button to join team (Buttons are labeled with an integer based on the team number)
    public void OnClick_JoinTeam(int teamNumber)
    {
        //If the team has less than two players
        if (teamMembers[teamNumber] < teamSize[teamNumber])
        {            
            //remove player from their previous team, if they are in one
            removeTeam();

            //update the number of players in the team the player is joining
            int teamsizeUpdate = teamMembers[teamNumber] + 1;

            //call the Update Method to sync it with other players
            UpdateArray(teamNumber, teamsizeUpdate);

            //Set the player properties team
            SetTeam(teamNumber);
        }
        else
        {
            Debug.LogWarning("Team " + teamNumber.ToString() + " is Full");
        }
        
    }


    private void removeTeam()
    {
        if (_playerProperties.ContainsKey("TEAM"))
        {
            //Get the team number of the old team
            int oldTeam = (int)_playerProperties["TEAM"];
            //get the old team members and remove one
            int oldTeamSize = teamMembers[oldTeam] - 1;
            //update the array
            UpdateArray(oldTeam, oldTeamSize);
        }
    }

    private void SetTeam(int team)
    {
        _playerProperties["TEAM"] = team;
        PhotonNetwork.SetPlayerCustomProperties(_playerProperties);


        
        PlayerData.team = team;
    }

    void Start()
    {
        // If this is the local player, initialize the array
        for (int i = 0; i < teamMembers.Length; i++)
        {
            teamMembers[i] = 0;
        }
    }

    // Update the array
    //when a player joins a team write a function that pings to everyone that they joined the team
    public void UpdateArray(int index, int value)
    {
        // Update the local array
        teamMembers[index] = value;

        // Send array updates to other players via Photon RPC
        photonView.RPC("ReceiveArrayUpdates", RpcTarget.Others, index, value);
    }


    [PunRPC]
    void ReceiveArrayUpdates(int index, int value)
    {
        // Update the local array with received data
        teamMembers[index] = value;
    }

    //On player entering the room Update already existing players in the room from the master client
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < teamMembers.Length; i++)
            {
                UpdateArray(i, teamMembers[i]);
            }
        }
    }

    //On Leave Room Remove Player from their team and clear their properties
    public void OnClick_LeaveRoomAndTeam()
    {
        removeTeam();
        _playerProperties.Clear();

    }

}
