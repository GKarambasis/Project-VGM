using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCConversation : MonoBehaviour
{
    [Header("Object Assignments")]
    public TextMeshProUGUI responseText;
    public TMP_InputField inputField;
    public GameObject chatPanel;


    public string fullText;    // Full input to display
    public Coroutine currentCoroutine;

    [Header("Settings")]
    public float delay = 0.1f; // Delay between each character

    public void TypeWriterText(string text)
    {
        fullText = text;
        currentCoroutine = StartCoroutine(ShowText(text));
    }

    IEnumerator ShowText(string fulltext)
    {

        responseText.text = ""; // Clear the input initially
        
        foreach (char c in fulltext)
        {
            responseText.text += c; // Add one character at a time
            yield return new WaitForSeconds(delay); // Wait for the delay before showing next character
        }

        currentCoroutine = null;
    }

    public void EnableChatPanel()
    {
        ClearChat();
        chatPanel.SetActive(true);
    }
    public void DisableChatPanel()
    {
        chatPanel.SetActive(false);
        ClearChat();

    }

    public void ClearChat()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        inputField.text = "";
        responseText.text = "";
    }

}
