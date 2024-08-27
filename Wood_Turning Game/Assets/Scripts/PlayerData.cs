using System.IO;
using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public static string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }

    private static string filePath;

    public int levelIndex = 1;
    public int moneyScore = 0;

    public void Save()
    {
        string json = JsonUtility.ToJson(this);

        using (FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate))
        using (StreamWriter sw = new StreamWriter(fileStream))
        {
            sw.Write(json);
        }
    }

    public void Load()
    {
        // Check if the file exists before trying to load it
        if (!File.Exists(FilePath))
        {
            Debug.LogError("File not found at " + FilePath);

            // Optionally, you could create a default file here if it doesn't exist
            Save();
            Debug.Log("Default file created at " + FilePath);
            return;
        }

        using (FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate))
        using (StreamReader sr = new StreamReader(fileStream))
        {
            string json = sr.ReadToEnd();

            if (!string.IsNullOrEmpty(json))
            {
                PlayerData copy = JsonUtility.FromJson<PlayerData>(json);

                if (copy != null)
                {
                    levelIndex = copy.levelIndex;
                    moneyScore = copy.moneyScore;
                }
            }
        }
    }
}
