using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.GameEngine.Geometry;
using System;
using UnityEngine.EventSystems;

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
        updateViewportPoints();
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject()) //To prevent interact with map when there's UI
            {
                return;
            }
            _touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                return;
            }
            Vector3 direction = _touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
            _locationSeerviceRef.CamInCentre = false;
        }
        if(Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            LocationService.Instance.CameraNotInCentreBehaviour();
            Vector3 _cameraCentre = this.transform.position;
            ShaderTextureTilingController.Instance.loopThroughViewport(GetCameraCentralTile(_cameraCentre).Item1, GetCameraCentralTile(_cameraCentre).Item2);
        }
        
        if(Input.touchCount ==2 )
        {
            
            LocationService.Instance.CameraNotInCentreBehaviour();
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

    #region private method
    void Zoom (float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        updateViewportPoints();
        Vector3 _cameraCentre = this.transform.position;
        ShaderTextureTilingController.Instance.loopThroughViewport(GetCameraCentralTile(_cameraCentre).Item1, GetCameraCentralTile(_cameraCentre).Item2);
    }

    void updateViewportPoints()
    {
        BottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        BottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        TopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        TopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }

    #endregion

    #region public method
    public ArcGISPoint GetCameraGISPos()
    {
        ArcGISPoint CameraGISPos = this.GetComponent<ArcGISPoint>();
        return CameraGISPos;
    }

    public ValueTuple<int, int> GetCameraCentralTile(Vector3 CentralPoint)
    {
        ValueTuple<int, int> centralTile = ShaderTextureTilingController.Instance.CalculateTileNumber(CentralPoint);
        return centralTile;
    }
    #endregion
}
