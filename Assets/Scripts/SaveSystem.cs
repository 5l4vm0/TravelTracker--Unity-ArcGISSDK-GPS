using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Esri.GameEngine.Geometry;

public static class SaveSystem 
{
    public static void SavePoints(ArcGISPoint visitedGISPoint)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/mapData.txt"; // where the path is going to be saved
        FileStream stream = new FileStream(path, FileMode.Create);

        MapData mapdata = new MapData(visitedGISPoint);

        formatter.Serialize(stream, mapdata); // write data to the file
        stream.Close();
    }

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
}
