using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveGameData
{
    public int lastLevelPlayed;
}

public class SaveGame : MonoBehaviour
{
    static public void SaveLevelProgress(int levelIndex)
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }

        SaveGameData data = new SaveGameData();
        data.lastLevelPlayed = levelIndex; // TODO set to the current level
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    static public int LoadLastLevelPlayed()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            return 1;
        }

        BinaryFormatter bf = new BinaryFormatter();
        SaveGameData data = (SaveGameData)bf.Deserialize(file);
        file.Close();

        return data.lastLevelPlayed;
    }
}
