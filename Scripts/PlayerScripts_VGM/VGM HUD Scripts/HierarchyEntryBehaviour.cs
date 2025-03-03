using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyEntryBehaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI entryName;
    [SerializeField] Image entryImage;
    public GameObject entryInstance;

    VGMInputController inputManager;

    private void Start()
    {
        inputManager = FindFirstObjectByType<VGMInputController>();
    }

    public void SetupButton(GameObject entryInstance)
    {
        SpawnableObjects info = entryInstance.GetComponent<ObjectInfo>().objectInformation;
        
        entryName.text = info.objectName;
        entryImage.sprite = info.objectSprite;
        this.entryInstance = entryInstance;
    }

    public void OnClick_SelectEntry()
    {
        if(entryInstance != null)
        {
            inputManager.SelectPlacedObject(entryInstance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
