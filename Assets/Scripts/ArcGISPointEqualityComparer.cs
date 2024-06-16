using System;
using System.Collections.Generic;
using Esri.GameEngine.Geometry;

public class ArcGISPointEqualityComparer : IEqualityComparer<ArcGISPoint>
{
    public bool Equals(ArcGISPoint p1, ArcGISPoint p2)
    {
        if (p1 == null && p2 == null)
            return true;
        if (p1 == null || p2 == null)
            return false;
        return p1.X == p2.X && p1.Y == p2.Y;
    }
    public int GetHashCode(ArcGISPoint p)
    {
        // This implementation is necessary for collections like HashSet or Dictionary
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + p.X.GetHashCode();
            hash = hash * 23 + p.Y.GetHashCode();
            return hash;
        }
    }
}
