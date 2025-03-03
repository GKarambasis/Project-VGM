using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatorEntryBehaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatorEntryName;
    [SerializeField] Image creatorEntrySprite;
    [SerializeField] Button button;
    public GameObject creatorEntryPrefab;

    VGMInputController inputManager;

    private void Start()
    {
       inputManager =  FindFirstObjectByType<VGMInputController>();
    }

    public void SetupButton(SpawnableObjects spawnableObject)
    {
        creatorEntryName.text = spawnableObject.objectName;
        gameObject.name = "Button_"+creatorEntryName.text;

        creatorEntrySprite.sprite = spawnableObject.objectSprite;

        creatorEntryPrefab = spawnableObject.prefab;
    }

    public void OnClickSelected()
    {
        if(inputManager == null) 
        {
            inputManager = FindFirstObjectByType<VGMInputController>();
        }
        inputManager.SelectObjectButton(gameObject);
    }

    
}
