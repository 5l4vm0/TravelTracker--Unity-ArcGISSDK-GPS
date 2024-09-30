using UnityEngine;
using System.Collections;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.SDK.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System;

public class LocationService : MonoBehaviour
{
    [SerializeField] private ArcGISLocationComponent _cameraRef;
    [SerializeField] private ArcGISLocationComponent _playerDotRef;
    [SerializeField] private ArcGISPoint _gpsPosition;
    [SerializeField] private ArcGISMapComponent _mapRef;
    private List<ArcGISPoint> _visitedPosList = new List<ArcGISPoint>();
    private StringBuilder _allVisitedPos;
    public List<ArcGISPoint> SavedPos;
    [SerializeField] private GisPosToPixel _gisPostToPixel;
    [SerializeField] private GISPosShader _shaderImage;
    private Vector2 _pointInUV;
    private ArcGISPoint _lastPosition;
    private Vector2 _lastPointInUV;
    public bool CamInCentre = true;
    [SerializeField] private ButtonBehaviour _reCentreButton;
    [SerializeField] private Image _testUIGPS;
    [SerializeField] private Sprite[] _globeImage;
    [SerializeField] private Image _testUIWifi;
    private bool _isFirstTimeReload = true;
    private bool _isCameraSetToStartingGPSPos = false;
    private bool _firstGetGPS;
    private DateTime _lastGotGPSTime;
    private DateTime _lastLocationSavedTime;
    private DateTime _lastCheckGPSTime;

    //Singleton
    public static LocationService Instance;

    private void Start()
    {
        Instance = this;

        StartCoroutine(LocationCoroutine());
        _allVisitedPos = new StringBuilder();
        SavedPos = SaveSystem.LoadPositions();
        
        if(SavedPos != null)
        {
            foreach (ArcGISPoint point in SavedPos)
            {
                _visitedPosList.Add(point);
                _allVisitedPos.Append("pointX" + point.X + " pointY" + point.Y + "/");
                //_pointInUV = _gisPostToPixel.gisPosToPixelMethod(point);
                //_shaderImage.updatePositionInTexture(_pointInUV);
            }
            Debug.Log("Loaded visited positions" + _allVisitedPos.ToString());
            _allVisitedPos.Clear();
        }
    }

    IEnumerator LocationCoroutine()
    {
        // Uncomment if you want to test with Unity Remote
        /*#if UNITY_EDITOR
                yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
                yield return new WaitForSecondsRealtime(5f);
        #endif*/
#if UNITY_EDITOR
        // No permission handling needed in Editor
#elif UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }

        // First, check if user has location service enabled
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure: if
            Debug.LogFormat("Android and Location not enabled");
            //yield break;
        }
        
#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("IOS and Location not enabled");
            yield break;
        }
#endif
        // Start service before querying location
        UnityEngine.Input.location.Start(25f, 0.1f); //Start(float desiredAccuracyInMeters, float updateDistanceInMeters);
        // desiredAccuracyInMeters = The service accuracy you want to use, in meters. This determines the accuracy of the device's last location coordinates.
        // updateDistanceInMeters = The minimum distance, in meters, that the device must move laterally before Unity updates location
        yield return null;
    }


    private void Update()
    {
        
        CheckInternet();

        if (UnityEngine.Input.location.status == LocationServiceStatus.Stopped || UnityEngine.Input.location.status == LocationServiceStatus.Failed)
        {
            RestartLocationService();
        }

        if (UnityEngine.Input.location.status == LocationServiceStatus.Running && (DateTime.UtcNow -_lastLocationSavedTime).TotalSeconds > 10)
        {
            SaveLocationsToFile();
        }

        if (UnityEngine.Input.location.status == LocationServiceStatus.Running && (DateTime.UtcNow - _lastCheckGPSTime).TotalSeconds > 3 && _mapRef.HasSpatialReference())
        {
            GetGPS();
        }
    }

    private void SaveLocationsToFile()
    {
        // This foreach loop just to print allVisitedPos in editor
        foreach (ArcGISPoint point in _visitedPosList)
        {
            _allVisitedPos.Append("pointX" + point.X + " pointY" + point.Y + "/");
        }
        // Debug.Log("visited positions" + _allVisitedPos.ToString());
        _allVisitedPos.Clear();

        SaveSystem.SavePositions(_visitedPosList);
        _lastLocationSavedTime = DateTime.UtcNow;
    }

    private void RestartLocationService()
    {
        _testUIGPS.sprite = _globeImage[0];

        //Try to start location service again 
        UnityEngine.Input.location.Stop();
        UnityEngine.Input.location.Start(25f, 0.1f);
    }

    private void GetGPS()
    {
        //GPS is working
        if ((_gpsPosition.X != UnityEngine.Input.location.lastData.latitude || _gpsPosition.Y != UnityEngine.Input.location.lastData.longitude))
        {
            //Set GPS Icon to coloured
            _testUIGPS.sprite = _globeImage[1];
           
            float latitude = UnityEngine.Input.location.lastData.latitude;
            float longtitude = UnityEngine.Input.location.lastData.longitude;
            float altitude = UnityEngine.Input.location.lastData.altitude;

            //Get round up to 5 decimal points
            _gpsPosition = new ArcGISPoint(
                Mathf.Round(longtitude * 100000f) / 100000f, 
                Mathf.Round(latitude * 100000f) / 100000f, 
                Mathf.Round(altitude * 100000f) / 100000f, 
                ArcGISSpatialReference.WGS84()
            );

            //Debug.Log($"gpsPosition: {_gpsPosition.X},{_gpsPosition.Y}");

            if (CamInCentre)
            {
                _cameraRef.Position = new ArcGISPoint(_gpsPosition.X, _gpsPosition.Y, 500, _gpsPosition.SpatialReference);
            }

             _playerDotRef.Position = new ArcGISPoint(_gpsPosition.X, _gpsPosition.Y, 26, _gpsPosition.SpatialReference);

            //Return if it's first time getting gps location
            //In order to give time to Unity cooridinate update with ArcGis gps location for camera and player dot
            if (!_firstGetGPS)
            {
                _firstGetGPS = true;
                return;
            }

            
            if (!_isCameraSetToStartingGPSPos)
            {
                
                _cameraRef.Position = new ArcGISPoint(_gpsPosition.X, _gpsPosition.Y, 500, _gpsPosition.SpatialReference);
                CameraMovement.Instance.updateViewportPoints();
                ShaderTextureTilingController.Instance.BasedRefBottomLeftPos = new Vector3(CameraMovement.Instance.BottomLeft.x, 0, CameraMovement.Instance.BottomLeft.z);
                ShaderTextureTilingController.Instance.InitialiseTextureForCameraWhenFirstGetGPS();
                _isCameraSetToStartingGPSPos = true;
                return;
            }
            
            Vector2 cameraInTextureTileNumber = new Vector2(
                CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item1, 
                CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item2
            );

            //Get current texture tile
            _gisPostToPixel = ShaderTextureTilingController.Instance.tiles[cameraInTextureTileNumber].transform.GetChild(0).GetComponent<GisPosToPixel>();
            _pointInUV = _gisPostToPixel.gisPosToPixelMethod(_gpsPosition);
            _shaderImage = _gisPostToPixel.gameObject.GetComponent<GISPosShader>();
            
            if (_lastPosition != null && _lastGotGPSTime != default && (DateTime.UtcNow - _lastGotGPSTime).TotalSeconds < 15)
            {
                //Draw line between _lastPosition and current gps position (Designed for losing gps for short period of time)
                _lastPointInUV = _gisPostToPixel.gisPosToPixelMethod(_lastPosition);
                _shaderImage.updateLineInTexture(_pointInUV, _lastPointInUV);
            }
           
            _lastPosition = _gpsPosition;
            _lastGotGPSTime = DateTime.UtcNow;

        }
        else  
        {
            Debug.Log("Lose GPS data");
        }

        if (_lastGotGPSTime!= null && ((DateTime.UtcNow - _lastGotGPSTime).TotalSeconds >= 15) ) //Lost GPS for more than 15f
        {
            _testUIGPS.sprite = _globeImage[0];
            Debug.Log("Time over 15sec");
        }

        //store gpsPosition into _visitedPosList if the gps position has not existed in the list yet
        //In this case, we can have a list to record all gps position data that we have been visited
        //Create a new comparer so Contains function is comparing the X and Y values
        ArcGISPointEqualityComparer comparer = new ArcGISPointEqualityComparer();
        if (!_visitedPosList.Contains(_gpsPosition, comparer))
        {
            _visitedPosList.Add(_gpsPosition);

            Vector2 cameraInTextureTileNumber = new Vector2(
                CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item1,
                CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item2
            );

            _gisPostToPixel = ShaderTextureTilingController.Instance.tiles[cameraInTextureTileNumber].transform.GetChild(0).GetComponent<GisPosToPixel>();
            _pointInUV = _gisPostToPixel.gisPosToPixelMethod(_gpsPosition);
            _shaderImage = _gisPostToPixel.gameObject.GetComponent<GISPosShader>();
            _shaderImage.updatePositionInTexture(_pointInUV);
            
        }
        _lastCheckGPSTime = DateTime.UtcNow;


    }

    private void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _testUIWifi.color = Color.red;
            
        }
        else if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            _testUIWifi.color = Color.green;
            //if (!_mapRef.HasSpatialReference() && _isFirstTimeReload)
            //{
            //    Debug.Log("try initialising map");
            //    StartCoroutine(ReInitialiseMap());
            //    _isFirstTimeReload = false;
            //}
        }
    }

    void SetMapCentre(ArcGISPoint point)
    {
        Debug.Log($"setting map centre:{point.X},{point.Y}");
        // update map geographic centre position based on GPS position
        var newExtent = _mapRef.Extent;
        newExtent.GeographicCenter = point;
        _mapRef.Extent = newExtent;
    }

    //Being called when click on recentre button
    public void CameraMoveBackToPlayerDotCentre()
    {
        Debug.Log("button pressed");
        _cameraRef.gameObject.transform.position = new Vector3(_playerDotRef.gameObject.transform.position.x, _cameraRef.gameObject.transform.position.y, _playerDotRef.gameObject.transform.position.z);
        _reCentreButton.IconBackToDefault();
        CamInCentre = true;
        Debug.Log("cam in centre");
    }

    public void CameraNotInCentreBehaviour()
    {
        _reCentreButton.SwapIcon();
        CamInCentre = false;
    }

    /// <summary>
    /// ReInitialiseMap by disabling and enabling ArcGisMap component
    /// </summary>
    /// <returns></returns>
    IEnumerator ReInitialiseMap()
    {
       _mapRef.enabled = false;
       yield return new WaitForSecondsRealtime(1);
       _mapRef.enabled = true;
    }

}
