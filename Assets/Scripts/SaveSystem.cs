using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Esri.GameEngine.Geometry;

public static class SaveSystem 
{
    /// <summary>
    /// save data using binary 
    /// </summary>
    /// <param name="visitedGISPoint"></param>
    public static void SavePoints(ArcGISPoint visitedGISPoint)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/mapData.txt"; // where the path is going to be saved
        FileStream stream = new FileStream(path, FileMode.Create);

        MapData mapdata = new MapData(visitedGISPoint);

        formatter.Serialize(stream, mapdata); // write data to the file
        stream.Close();
    }
    /// <summary>
    /// Load data using binary
    /// </summary>
    /// <returns></returns>
    public static MapData LoadPoints()
    {
        string path = Application.persistentDataPath + "/mapData.txt";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            MapData data = formatter.Deserialize(stream) as MapData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Save data using JSON
    /// </summary>
    /// <param name="visitedGISPoint"></param>
    public static void SavePositions(ArcGISPoint visitedGISPoint)
    {
        MapData mapdata = new MapData(visitedGISPoint);
        string positionsData = JsonUtility.ToJson(mapdata);
        string path = Application.persistentDataPath + "/mapData.json";
        File.WriteAllText(path, positionsData);
    }

    /// <summary>
    /// Load data from JSON file
    /// </summary>
    public static MapData LoadPositions()
    {
        string path = Application.persistentDataPath + "/mapData.json";
        if(File.Exists(path))
        {
            string positionsData = File.ReadAllText(path);
            MapData mapdata = JsonUtility.FromJson<MapData>(positionsData);
            return mapdata;
        }
        else
        {
            return null;
        }
    }
}
