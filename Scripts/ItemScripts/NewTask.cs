using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewTask : MonoBehaviour
{
    [SerializeField] GameObject task;
    [SerializeField] TextMeshProUGUI taskText;

    public void AssignTask(string text, int seconds)
    {
        StartCoroutine(TaskCoroutine(text, seconds));
    }

    IEnumerator TaskCoroutine(string text, int seconds)
    {
        taskText.text = "New Task: " + text;
        task.SetActive(true);

        yield return new WaitForSeconds(seconds);

        task.SetActive(false);
    }

}
