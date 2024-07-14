using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.GameEngine.Geometry;

public class CameraMovement : MonoBehaviour
{
    Vector3 _touchStart;
    [SerializeField] private float zoomOutMin = 600;
    [SerializeField] private float zoomOutMax = 2000;
    [SerializeField] private LocationService _locationSeerviceRef;


    public Vector3 BottomLeft;
    public Vector3 BottomRight;
    public Vector3 TopLeft;
    public Vector3 TopRight;

    //Singleton
    public static CameraMovement Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        BottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        BottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        TopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        TopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 direction = _touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
            _locationSeerviceRef.CamInCentre = false;
        }
        if(Input.GetMouseButtonUp(0))
        {
            
            Vector3 _cameraCentre = this.transform.position;
            ShaderTextureTilingController.Instance.AddShaderTexture(ShaderTextureTilingController.Instance.CalculateTileNumber(_cameraCentre).Item1, ShaderTextureTilingController.Instance.CalculateTileNumber(_cameraCentre).Item2);
        }
        
        if(Input.touchCount ==2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrePos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrePos = touchOne.position - touchOne.deltaPosition;

            float preMagniture = (touchZeroPrePos - touchOnePrePos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - preMagniture;
            Zoom(difference);
        }
    }

    void Zoom (float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);

    }

    public ArcGISPoint GetCameraGISPos()
    {
        ArcGISPoint CameraGISPos = this.GetComponent<ArcGISPoint>();
        return CameraGISPos;
    }

}
