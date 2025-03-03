using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] public GameObject[] menuPanels;
    [SerializeField] GameObject errorPanel;
    [SerializeField] GameObject startingPanel;

    public void MenuButton(int index)
    {
        if (menuPanels[index] == null)
        {
            Debug.LogWarning("No menu Screen found in index: " + index);
        }
        else
        {
            if (menuPanels[index].activeInHierarchy)
            {
                Debug.Log("Panel Already Active in Hierarchy");
                return;
            }

            if(startingPanel!=null)
            {
                //Error Message if player tries to open menus or start the game without logging in
                if (startingPanel.activeInHierarchy)
                {
                    errorPanel.SetActive(true);
                    return;
                }
            }
            

            //Close down any opened menu tabs when opening another
            for (int i = 0; i < menuPanels.Length; i++)
            {
                if (menuPanels[i].activeInHierarchy)
                {
                    menuPanels[i].SetActive(false);
                }
            }
            
            //Show Menu
            menuPanels[index].SetActive(true);
        }        
    }

    public void MenuBackButton(int index)
    {
        if (menuPanels[index] == null)
        {
            Debug.Log("Error in finding Menu");
        }
        else
        {

            menuPanels[index].gameObject.SetActive(false);

        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
