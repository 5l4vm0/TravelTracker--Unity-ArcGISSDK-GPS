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


    void Start()
    {
        Instance = this;
        updateViewportPoints();
    }


    void Update()
    {
        if((Input.touchCount ==1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if((Input.touchCount >0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))) //To prevent interact with map when there's UI
            {
                Debug.Log($"return{Input.touchCount}, {EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)}");
                return;
            }
            
            if(Input.touchCount > 0)
            {
                _touchStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            LocationService.Instance.CameraNotInCentreBehaviour();
        }

        if( (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            if ((Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))) //To prevent interact with map when there's UI
            {
                return;
            }
            if (Input.touchCount >0)
            {
                Vector3 direction = _touchStart - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Camera.main.transform.position += direction;
            }
        }
        if( (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if ((Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))) //To prevent interact with map when there's UI
            {
                return;
            }
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

    public void updateViewportPoints()
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
