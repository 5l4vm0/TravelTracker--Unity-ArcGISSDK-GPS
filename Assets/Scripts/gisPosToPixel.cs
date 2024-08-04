using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.Components;

public class GisPosToPixel : MonoBehaviour
{
    [SerializeField] private ArcGISMapComponent mapRef;
    [SerializeField] private GameObject V0_0;
    [SerializeField] private ArcGISPoint v0_0GIS;
    [SerializeField] private GameObject V1_0;
    [SerializeField] private ArcGISPoint v1_0GIS;
    [SerializeField] private GameObject V0_1;
    [SerializeField] private ArcGISPoint v0_1GIS;

    //properties
    public ArcGISPoint V0_0GIS 
    {
        get { return v0_0GIS; } 
    }
    public ArcGISPoint V1_0GIS
    {
        get { return v1_0GIS; }
    }
    public ArcGISPoint V0_1GIS
    {
        get { return v0_1GIS; }
    }

    public void Awake()
    {
        StartCoroutine(InitialiseGisPos());
    }

    private IEnumerator InitialiseGisPos()
    {
        mapRef = GameObject.Find("ArcGISMap").GetComponent<ArcGISMapComponent>();

        while (!mapRef.HasSpatialReference())
        {
            //Debug.Log("Waiting for ArcGISMapComponent to have a valid spatial reference...");
            yield return null;
        }


        //Convert unity global position to geographic position by mapRef.EngineToGeographic, then project it to coordinate WGS84 so it shows as latitude and longtitude
        v0_0GIS = (ArcGISPoint)ArcGISGeometryEngine.Project(mapRef.EngineToGeographic(new Vector3(transform.position.x - 1500, 0, transform.position.z - 1500)), ArcGISSpatialReference.WGS84());
        v1_0GIS = (ArcGISPoint)ArcGISGeometryEngine.Project(mapRef.EngineToGeographic(new Vector3(transform.position.x + 1500, 0, transform.position.z - 1500)), ArcGISSpatialReference.WGS84());
        v0_1GIS = (ArcGISPoint)ArcGISGeometryEngine.Project(mapRef.EngineToGeographic(new Vector3(transform.position.x - 1500, 0, transform.position.z + 1500)), ArcGISSpatialReference.WGS84());

        //Enable ArcGisLocationComponent so the object is placed at the right place. Not sure why but when it first spawns, ArcGisLocationComponent is disabled
        V0_0.GetComponent<ArcGISLocationComponent>().enabled = true;
        V1_0.GetComponent<ArcGISLocationComponent>().enabled = true;
        V0_1.GetComponent<ArcGISLocationComponent>().enabled = true;

        //Assign gameobject to the geographic position
        V0_0.GetComponent<ArcGISLocationComponent>().Position = v0_0GIS;
        V1_0.GetComponent<ArcGISLocationComponent>().Position = v1_0GIS;
        V0_1.GetComponent<ArcGISLocationComponent>().Position = v0_1GIS;
    }

    public Vector2 gisPosToPixelMethod(ArcGISPoint point)
    {
        float xInUV = (float)((point.X - v0_0GIS.X) / (v1_0GIS.X - v0_0GIS.X));
        float yInUV = (float)((point.Y - v0_0GIS.Y) / (v0_1GIS.Y - v0_0GIS.Y));
        return new Vector2(xInUV, yInUV);
    }

}


//public class Test : MonoBehaviour
//{
//    public ArcGISMapComponent mapRef;
//    public GameObject prefab;
//    public Vector3 position = new Vector3(10, 0, 0);

//    // Start is called before the first frame update
//    void Start()
//    {

//        Instantiate(prefab, Vector3.zero, new Quaternion(0, 0, 0, 0), this.transform);
//        //double3 worldPosition = math.inverse(mapRef.WorldMatrix).HomogeneousTransformPoint(position.ToDouble3());
//        //ArcGISPoint arcgispoint = mapRef.View.WorldToGeographic(worldPosition);
//        ArcGISPoint arcgispoint = mapRef.EngineToGeographic(position);

//        ArcGISPoint gispoint = (ArcGISPoint)ArcGISGeometryEngine.Project(arcgispoint, ArcGISSpatialReference.WGS84());
//        prefab.GetComponent<ArcGISLocationComponent>().Position = gispoint;
//        Debug.Log($"position {gispoint.X}, {gispoint.Y}");
//    }
//}
