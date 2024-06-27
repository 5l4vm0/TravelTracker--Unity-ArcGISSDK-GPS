using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.Components;

public class gisPosToPixel : MonoBehaviour
{
    public ArcGISLocationComponent _playeDot;
    [SerializeField] private GameObject V0_0;
    [SerializeField] private ArcGISPoint v0_0GIS;
    [SerializeField] private GameObject V1_0;
    [SerializeField] private ArcGISPoint v1_0GIS;
    [SerializeField] private GameObject V0_1;
    [SerializeField] private ArcGISPoint v0_1GIS;

    public void Start()
    {
        v0_0GIS = V0_0.GetComponent<ArcGISLocationComponent>().Position;
        v1_0GIS = V1_0.GetComponent<ArcGISLocationComponent>().Position;
        v0_1GIS = V0_1.GetComponent<ArcGISLocationComponent>().Position;
        
        gisPosToPixelMethod(_playeDot.Position);
    }

    public void gisPosToPixelMethod(ArcGISPoint point)
    {
        float xInUV = (float)((point.X - v0_0GIS.X) / (v1_0GIS.X - v0_0GIS.X));
        float yInUV = (float)((point.Y - v0_0GIS.Y) / (v0_1GIS.Y - v0_0GIS.Y));
        Debug.Log("xInUV "+xInUV+"  yInUV "+yInUV);
    }

}
