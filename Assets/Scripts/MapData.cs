using UnityEngine;
using System;
using Esri.GameEngine.Geometry;
using System.Collections.Generic;

[Serializable] //able to save it to file
public class MapData 
{
    public Point[] visitedPos;

    public MapData(List<ArcGISPoint> visitedGISPoint)
    {
        visitedPos = new Point[visitedGISPoint.Count];
        for(int i = 0; i < visitedGISPoint.Count; i++)
        {
            visitedPos[i] = new Point(visitedGISPoint[i].X, visitedGISPoint[i].Y);
        }
        
    }
}

[Serializable]
public class Point
{
    public double x;
    public double y;

    public Point(double X, double Y)
    {
        x = X;
        y = Y;
    }
}
