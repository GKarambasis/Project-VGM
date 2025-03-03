using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPCController : NPCBehaviour
{
    //This class controlls the NPC's Conversation Actions
    
    [Header("HUD")]
    public TextMeshProUGUI textPrompt;
    public TMP_InputField inputField;
    public NPCConversation npcConversation;
    public string textPromptText;

    [Header("Conversation")]
    bool interactable = false;
    bool interacting = false;
    bool playerIsInteracting = false;
    public bool processingMessage = false;

    private GameObject currentPlayer;
    private NPCAttack npcAttack;
    private Animator animator;

    //Get References to other Scripts
    private void Awake()
    {
        SetUpAttributes();
        if(GameObject.Find("ActionPromptText") != null)
        {
            textPrompt = GameObject.Find("ActionPromptText").GetComponent<TextMeshProUGUI>();
        }
        npcConversation = FindObjectOfType < NPCConversation >(true);
        chatGPTManager = FindAnyObjectByType<ChatGPTManager>();       
        animator = GetComponentInChildren<Animator>();
        npcAttack = GetComponent<NPCAttack>();
    }


    private void Update()
    {
        Conversation();
    }

    //Enables Conversation stage with the NPC upon being eligible to interact with it
    void Conversation()
    {
        //Enter Conversation
        if (interactable && Input.GetButtonDown("Interact") && !interacting)
        {
            EnableConversation();

        }
        
        //Exit Conversation
        if (Input.GetKeyDown(KeyCode.Escape) && interacting) 
        {
            DisableConversation();
        }
        
        //Send the Message
        if (Input.GetButtonDown("Submit") && interacting)
        {
            //Check that the message is not being processed and not being printed
            if (!processingMessage && npcConversation.currentCoroutine == null)
            {
                processingMessage = true;
                SendMessage();
            }
            else
            {
                Debug.LogWarning("Still printing out response");
            }
        }
    }

    //Conversation Methods
    public void EnableConversation()
    {
        interacting = true;
        photonView.RPC("EnablePlayerInteraction", RpcTarget.Others);
        
        animator.SetBool("Talking", true);
        

        //Enable Prompt to press E
        PromptText(false);

        //Enable the first child
        npcConversation.EnableChatPanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        //add code to enable llm npc interaction here
        //Set the manager's messages to this npc's current messages
        chatGPTManager.npcMessages = messages;
        SetUpNPC();

        //Disable Movement during conversation
        currentPlayer.GetComponent<AdventurerMovement>().enabled = false;
        currentPlayer.GetComponent<AdventurerAttack>().enabled = false;
    }
    public void DisableConversation()
    {
        currentPlayer.GetComponent<AdventurerMovement>().enabled = true;
        currentPlayer.GetComponent<AdventurerAttack>().enabled = true;
        
        interacting = false;
        photonView.RPC("DisablePlayerInteraction", RpcTarget.Others);

        
        animator.SetBool("Talking", false);
        

        //Disable Chat 
        npcConversation.DisableChatPanel();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }
    public void SendMessage()
    {
        string message = PhotonNetwork.NickName + ": " + npcConversation.inputField.text;
        
        //Clear Messages to prepare for the new one
        npcConversation.ClearChat();


        photonView.RPC("InvokeUpdateMessageList", RpcTarget.Others, message);
        
        
        chatGPTManager.AskChatGPT(message, gameObject);

    }


    //other players that have not interacted with the npc lose the ability to do so and animations get synced
    [PunRPC]
    void EnablePlayerInteraction()
    {
        playerIsInteracting = true;
        
        animator.SetBool("Talking", true);
        

    }
    [PunRPC]
    void DisablePlayerInteraction()
    {
        playerIsInteracting = false;
        
        animator.SetBool("Talking", false);
        
    }



    // Allow players to interact with this npc upon being close
    private void OnTriggerEnter(Collider other)
    {
        //Ensure that player is not interracting already
        if (other.tag == "Player" && !playerIsInteracting)
        {
            if (!GetAttackState())
            {
                OnCollisionEnableInteraction(other.gameObject);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {

        //If the player is in range, and there is no other player interacting and they are not in the interactable state
        if (other.tag == "Player" && !playerIsInteracting && !interactable)
        {
            //check if the npc is attacking
            if (!GetAttackState())
            {
                OnCollisionEnableInteraction(other.gameObject);
            }
            else
            {
                OnCollisionDisableInteraction();
            }
        }
        else if (other.tag == "Player" && playerIsInteracting && interactable)
        {
            OnCollisionDisableInteraction();
        }
   
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnCollisionDisableInteraction();
        }
    }


    //Enable and Disable the ability to interact with the NPC upon entering the interaction collision sphere
    void OnCollisionEnableInteraction(GameObject other)
    {
        // Pop up screen prompting players to press a button
        PromptText(true);

        // Allow player to interact 
        interactable = true;

        if (other.GetPhotonView().IsMine)
        {
            currentPlayer = other;
        }

    }
    public void OnCollisionDisableInteraction()
    {
        PromptText(false);
        interactable = false;

        currentPlayer = null;
    }
    
    //enables and disables the input prompt for the npc
    void PromptText(bool enabled)
    {
        if (textPrompt != null)
        {
            if (enabled)
            {
                textPrompt.text = textPromptText;
                textPrompt.enabled = true;
            }
            else
            {
                textPrompt.enabled = false;
            }
        }
    }

    //Gets the NPC Attack State to allow the ability to converse or not.
    bool GetAttackState()
    {
        if(npcAttack != null)
        {
            if (npcAttack.isActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}
