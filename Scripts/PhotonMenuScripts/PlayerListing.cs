using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public Player Player { get; private set; }

    //Load Player Name & Team when Joining
    public void SetPlayerInfo(Player player)
    {
        Player = player;

        SetPlayerText(player);
    }

    //Update it For All Players if Team is Modified
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if(targetPlayer != null && targetPlayer == Player )
        {
            if (changedProps.ContainsKey("TEAM"))
            {
                SetPlayerText(targetPlayer);
            }
        }
    }
    
    private void SetPlayerText(Player player)
    {
        int team = -1;
        if (player.CustomProperties.ContainsKey("TEAM"))
        {
            team = (int)player.CustomProperties["TEAM"];
        }

        switch (team)
        {
            case -1:
                _text.text = player.NickName;
                break;
            case 0:
                _text.text = player.NickName + ", Adventurer";
                break;
            case 1:
                _text.text = player.NickName + ", VGM";
                break;
            default:
                _text.text = player.NickName;
                break;
        }
    }
}   
