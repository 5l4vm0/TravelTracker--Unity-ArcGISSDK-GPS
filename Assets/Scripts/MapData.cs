using UnityEngine;
using Esri.GameEngine.Geometry;

[System.Serializable] //able to save it to file
public class MapData 
{
    public double[] visitedPos;

    public MapData(ArcGISPoint visitedGITPoint)
    {
        visitedPos = new double[2];
        visitedPos[0] = visitedGITPoint.X;
        visitedPos[1] = visitedGITPoint.Y;
    }
}
