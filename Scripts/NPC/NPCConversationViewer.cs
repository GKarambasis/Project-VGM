using OpenAI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCConversationViewer : MonoBehaviour
{

    private VGMInputController vgmController;
    private NPCBehaviour selectedNPCBehaviour;

    [SerializeField]
    GameObject Content;
    [SerializeField]
    TextMeshProUGUI responseText;

    private List<ChatMessage> messages;


    private void Awake()
    {
        vgmController = FindAnyObjectByType<VGMInputController>();

    }
    
    private void OnEnable()
    {
        //Check to see if the object is an npc (it should be but check anyhow)
        //Get the selected object from the VGM controller
        selectedNPCBehaviour = vgmController.selectedPlacedObject.GetComponent<NPCBehaviour>();

        messages = selectedNPCBehaviour.messages;

        foreach (ChatMessage message in messages)
        {
            string messageContent = message.Content;

            TextMeshProUGUI newText = Instantiate(responseText, Content.transform);

            //Show NPC Messages in white and Player Messages in Red
            if (!messageContent.Contains(":"))
            {
                string newContent = "NPC: " + messageContent;
                messageContent = newContent;
            }
            else
            {
                newText.color = Color.red;
            }

            //Debug.LogWarning(messageContent);
            newText.text = messageContent;
            newText.GetComponent<AutoResizeText>().ScaleHeightToFitText();
        }
    }

    //Clear the conversation upon closing the panel
    private void OnDisable()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
        
    }
}
