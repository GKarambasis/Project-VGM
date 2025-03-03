using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using Photon.Pun;

public class ChatGPTManager : MonoBehaviourPunCallbacks
{
    //Unsafe practise, but made to save on time. Real keys have been redacted. 
    private OpenAIApi openAI = new OpenAIApi("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "org-xxxxxxxxxxxx");
    public List<ChatMessage> npcMessages;
    public NPCConversation npcConversation;

    [Header("Global NPC Info")]
    [SerializeField] string triggerWord = "Attack!";
    [TextArea(15, 10)]
    public string worldContext;


    private void Awake()
    {
        npcConversation = FindObjectOfType<NPCConversation>(true);
    }

    public async void AskChatGPT(string text, GameObject npc)
    {
        float timeBefore = Time.time;
        
        //define the new message
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = text;
        newMessage.Role = "user";

        //Add it to the list of messages
        npcMessages.Add(newMessage);

        //Create API Request
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = npcMessages;
        request.Model = "gpt-3.5-turbo";
        request.MaxTokens = 100;

        //Send the API Request and await for the response
        var response = await openAI.CreateChatCompletion(request);

        //If GPT Returns a Response
        if(response.Choices != null && response.Choices.Count > 0)
        {
            float timeAfter = Time.time;
            float timeToProcess = timeAfter - timeBefore;

            Debug.Log("Response returned in: " + timeToProcess + " Seconds");
            FileWriter.WriteToFile(timeToProcess);

            //Get the first response option
            var chatResponse = response.Choices[0].Message;
            
            //Add the Response to the List of Messages
            npcMessages.Add(chatResponse);

            //Get a reference to the NPC's controller and synchronise Message List to all players
            NPCController npcController = npc.GetComponent<NPCController>();
            npcController.UpdateMessageList(chatResponse.Content);
            
            //Check if the Response contains a triggerword
            if (chatResponse.Content.Contains(triggerWord))
            {
                npcController.DisableConversation();
                npc.GetComponent<NPCAttack>().AttackState(true);
                return;
            }
            
            
            Debug.Log(chatResponse.Content);
            npcConversation.TypeWriterText(chatResponse.Content);
            npcController.processingMessage = false;
        }
    }
    
    //Updates the World Context for all players
    public void UpdateWorldContext(string newWorldContext) 
    {
        worldContext = newWorldContext;
        photonView.RPC("InvokeUpdateWorldContext", RpcTarget.Others, newWorldContext);
    }
    [PunRPC]
    void InvokeUpdateWorldContext(string newWorldContext)
    {
        worldContext = newWorldContext;
    }
}
