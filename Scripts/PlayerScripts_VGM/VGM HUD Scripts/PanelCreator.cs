using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCreator : MonoBehaviour
{
    [SerializeField]
    GameObject[] contentPanels;
    [SerializeField]
    SpawnableObjectInventory[] objectInventories;
    [SerializeField]
    GameObject buttonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //For every scrollview get the content
        for (int i = 0; i < objectInventories.Length; i++)
        {

            if (objectInventories[i] != null)
            {
                for (int j = 0; j < objectInventories[i].spawnableObjects.Length; j++)
                {
                    GameObject button = Instantiate(buttonPrefab, contentPanels[i].transform);
                    button.GetComponent<CreatorEntryBehaviour>().SetupButton(objectInventories[i].spawnableObjects[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
