using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManipulation : MonoBehaviour
{
    TaskManager taskManager;
    private Slider slider;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        taskManager = FindAnyObjectByType<TaskManager>();
    }

    public void UpdateTime()
    {
        taskManager.UpdateTime(slider.value);
    }
}
