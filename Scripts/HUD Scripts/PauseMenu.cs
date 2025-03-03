using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool menuEnabled = false;
    Image backgroundImage;
    [SerializeField] GameObject pauseMenu;

    CursorLockMode previousLockMode;
    bool previousVisibility;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!menuEnabled)
            {
                EnablePauseMenu();
            }
            else
            {
                DisablePauseMenu();
            }
        }
    }
    
    void EnablePauseMenu()
    {
        menuEnabled = true;

        backgroundImage.enabled = true;
        pauseMenu.SetActive(true);

        previousLockMode = Cursor.lockState;
        previousVisibility = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void DisablePauseMenu()
    {
        menuEnabled = false;

        backgroundImage.enabled = false;
        pauseMenu.SetActive(false);

        Cursor.lockState = previousLockMode;
        Cursor.visible = previousVisibility;
    }

    public void OnClick_LeaveLobby()
    {
        PlayerData.LogOut();
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void OnClick_Continue()
    {
        DisablePauseMenu();
    }
}
