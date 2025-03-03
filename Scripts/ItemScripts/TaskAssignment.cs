using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskAssignment : MonoBehaviour
{

    [SerializeField] TMP_InputField taskText;
    [SerializeField] TMP_InputField taskTime;

    TaskManager taskManager;

    private void Start()
    {
        taskManager = FindObjectOfType<TaskManager>(true);
    }

    public void OnClick_AssignTask()
    {
        int time;

        //ensure that it can be converted into an integer
        try
        {
            time = int.Parse(taskTime.text);
        }
        catch (FormatException ex)
        {
            Debug.LogError(ex.Message);
            return;
        }


        taskManager.NewTask(taskText.text, time);
    }


}
