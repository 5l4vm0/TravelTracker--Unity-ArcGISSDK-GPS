using UnityEngine;
using Esri.GameEngine.Geometry;

[System.Serializable] //able to save it to file
public class MapData 
{
    public double[] visitedPos;

    public MapData(ArcGISPoint visitedGISPoint)
    {
        visitedPos = new double[2];
        visitedPos[0] = visitedGISPoint.X;
        visitedPos[1] = visitedGISPoint.Y;
    }
}
