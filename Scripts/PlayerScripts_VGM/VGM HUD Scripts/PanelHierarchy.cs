using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHierarchy : MonoBehaviour
{
    [Header("Entry Variables")]
    [SerializeField] GameObject entryButtonPrefab;
    [SerializeField] GameObject entryContainer;

    [SerializeField]List<GameObject> entryButtons = new List<GameObject>();

    public void AddEntry(GameObject entryInstance)
    {
        GameObject entryButton = Instantiate(entryButtonPrefab, entryContainer.transform);

        //SetUp Instantiated Button
        entryButton.GetComponent<HierarchyEntryBehaviour>().SetupButton(entryInstance);

        entryButtons.Add(entryButton);
    }


    public void RemoveEntry()
    {
        StartCoroutine(RemoveEntryCoroutine());
    }

    IEnumerator RemoveEntryCoroutine()
    {
        yield return new WaitForSeconds(0.0f);
        int index = -1;
        for (int i = 0;  i < entryButtons.Count; i++)
        {
            if (entryButtons[i].GetComponent<HierarchyEntryBehaviour>().entryInstance == null)
            {
                index = i;
                Destroy(entryButtons[i]);
            }
        }
        if(index != -1)
        {
            entryButtons.Remove(entryButtons[index]);
            Debug.Log(index.ToString());
        }
    }

}
