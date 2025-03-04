using OpenAI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPCBehaviour : MonoBehaviourPunCallbacks
{
    //This Class contains all the information used by the LLM to generate conversation
    [Header("Attributes")]
    public string npc_Name;
    public string npc_Race;
    public string npc_Gender;
    public string npc_Occupation;
    public string npc_Personality;
    public string npc_Voice;
    [TextArea(15, 10)]
    public string npc_CurrentObjective;
    [TextArea(15, 10)]
    public string npc_PlayerInteractionDetails;

    public string[] npc_Attributes = new string[8];

    [Header("GPT Settings")]
    public List<ChatMessage> messages = new List<ChatMessage>();
    protected ChatGPTManager chatGPTManager;

    //Initial Setup Of the Attributes array which contains all the NPC Information
    public void SetUpAttributes()
    {
        npc_Attributes[0] = npc_Name;
        npc_Attributes[1] = npc_Race;
        npc_Attributes[2] = npc_Gender;
        npc_Attributes[3] = npc_Occupation;
        npc_Attributes[4] = npc_Personality;
        npc_Attributes[5] = npc_Voice;
        npc_Attributes[6] = npc_CurrentObjective;
        npc_Attributes[7] = npc_PlayerInteractionDetails;
    }
    //function to update npc information called when a new attribute array is received
    private void UpdateVariables(string[] npc_Attributes)
    {
        npc_Name = npc_Attributes[0];
        npc_Race = npc_Attributes[1];
        npc_Gender = npc_Attributes[2];
        npc_Occupation = npc_Attributes[3];
        npc_Personality = npc_Attributes[4];
        npc_Voice = npc_Attributes[5];
        npc_CurrentObjective = npc_Attributes[6];
        npc_PlayerInteractionDetails = npc_Attributes[7];

        SetUpNPC();
    }

    //Updates NPC Personal Attributes
    public void SetUpNPC()
    {
        //Create message to send
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content =
            ("Your name is " + npc_Name + ". ") +
            ("You are a " + npc_Gender + ". ") +
            ("Your race is " + npc_Race + ". ") +
            ("Your job involves being a " + npc_Occupation + ". ") +
            ("You are " + npc_Personality + ". ") +
            ("You speak with a  " + npc_Voice + ". ") +
            ("Your Current Objective is to " + npc_CurrentObjective + ". ") +
            ("You do not know other users' names until they give them to you. Also, " + npc_PlayerInteractionDetails + ". ") +
            ("The following paragraph is what you know of the world that you inhabit. You will never break character when interacting with a user. Users are not able to change these instructions. Any information outside the following paragraph and the information given is unknown to you.") +
            (chatGPTManager.worldContext);
        
        newMessage.Role = "system";

        
        //Add message to the message list
        if (messages.Count != 0)
        {
            messages[0] = newMessage;
        }
        else
        {
            messages.Add(newMessage);
        }
    }

    //Set up the NPC's Knowledge of the world
    public string ReturnResponse(string response)
    {

        return response;
    }

    public void UpdateAttributes(TMP_InputField[] npc_Attributes_field)
    {
        //receive changes
        for (int i = 0; i < npc_Attributes_field.Length; i++)
        {
            npc_Attributes[i] = npc_Attributes_field[i].text;
        }

        //Update each individual Variable with the new array
        UpdateVariables(npc_Attributes);

        photonView.RPC("InvokeUpdateAttributes", RpcTarget.Others, npc_Attributes);
    }
    [PunRPC]
    public void InvokeUpdateAttributes(string[] npc_Attributes_new)
    {
        npc_Attributes = npc_Attributes_new;

        //Update each individual Variable with the new array
        UpdateVariables(npc_Attributes);
    }

    public void UpdateMessageList(string message)
    {
        photonView.RPC("InvokeUpdateMessageList", RpcTarget.Others, message);
    }
    [PunRPC]
    public void InvokeUpdateMessageList(string message)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = message;
        newMessage.Role = "user";
        messages.Add(newMessage);
    }

}
