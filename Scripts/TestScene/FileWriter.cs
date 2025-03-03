using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileWriter
{
    private static string folderPath = Path.Combine(Application.dataPath, "ChatGPTLogs");
    private static string filePath = Path.Combine(folderPath, "GPTResponseTimes.txt");

    /// <summary>
    /// Used for logging ChatGPT's response times for quantitative studies 
    /// </summary>
    /// <param name="value">The seconds it took ChatGPT to generate and send the response</param>
    public static void WriteToFile(float value)
    {
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Write or append to the file
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(value);
        }

        Debug.Log($"Float value {value} written to {filePath}");
    }
}
