using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WorldContextSetting : MonoBehaviour
{
    string worldContext;

    [SerializeField] TMP_InputField input;
    [SerializeField] TextMeshProUGUI inputText;

    private ChatGPTManager gptManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gptManager = FindAnyObjectByType<ChatGPTManager>();

        input.text = gptManager.worldContext;
    }

    

    public void OnClickUpdateWorldContext()
    {
        worldContext = input.text;

        StartCoroutine(TextFlash(inputText));

        gptManager.UpdateWorldContext(worldContext);
    }

    IEnumerator TextFlash(TextMeshProUGUI inputText)
    {
        inputText.color = Color.green;

        yield return new WaitForSeconds(0.2f);

        inputText.color = Color.white;
    }
}
