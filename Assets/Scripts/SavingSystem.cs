using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavingSystem // the system is static so we can't create another one and by mistake have 2 database
{
    public static void SaveGame(GameData data) 
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, "game.data");
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static GameData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "game.data");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;

            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void DeleteGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "game.data");

        if (File.Exists(path)) File.Delete(path);
    }

     public static void SaveShipsList(List<int> ships)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, "ships.data");
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, ships);
        stream.Close();
    }

    public static List<int> LoadShips()
    {
        string path = Path.Combine(Application.persistentDataPath, "ships.data");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            List<int> ships = formatter.Deserialize(stream) as List<int>;

            stream.Close();

            return ships;
        }
        else
        {
            return null;
        }
    }
}
