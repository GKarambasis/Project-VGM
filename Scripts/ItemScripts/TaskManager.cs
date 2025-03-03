using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TaskManager : MonoBehaviourPunCallbacks
{
    NewTask newTask;
    private float fixedDeltaTime;

    private void Awake()
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }
    private void Start()
    {
        newTask = FindObjectOfType<NewTask>(true);
    }

    public void NewTask(string text, int seconds)
    {
        photonView.RPC("InvokeNewTask", RpcTarget.Others, text, seconds);
    }

    [PunRPC]
    public void InvokeNewTask(string text, int seconds)
    {
        newTask.AssignTask(text, seconds);
    }


    public void UpdateTime(float value)
    {
        photonView.RPC("InvokeUpdateTime", RpcTarget.Others, value);
        
        Time.timeScale = value;

        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    [PunRPC]
    public void InvokeUpdateTime(float value)
    {
        Time.timeScale = value;

        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}
