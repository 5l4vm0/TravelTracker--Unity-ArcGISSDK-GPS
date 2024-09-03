using UnityEngine;
using System.Collections;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.SDK.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class LocationService : MonoBehaviour
{
    [SerializeField] private ArcGISLocationComponent _cameraRef;
    [SerializeField] private ArcGISLocationComponent _playerDotRef;
    [SerializeField] private ArcGISPoint _gpsPosition;
    [SerializeField] private ArcGISMapComponent _mapRef;
    private List<ArcGISPoint> _visitedPosList = new List<ArcGISPoint>();
    private float _elapsedTime;
    private StringBuilder _allVisitedPos;
    public List<ArcGISPoint> SavedPos;
    [SerializeField] private GisPosToPixel _gisPostToPixel;
    [SerializeField] private GISPosShader _shaderImage;
    private Vector2 _pointInUV;
    private ArcGISPoint _lastPosition;
    private Vector2 _lastPointInUV;
    public bool CamInCentre = true;
    [SerializeField] private ButtonBehaviour _reCentreButton;
    [SerializeField] private Image testUIGPS;
    [SerializeField] private Image testUIWifi;
    private bool isFirstTimeReload = true;
    private bool _isFirstGettingGPS = true;
    private float _latitude;
    private float _longitude;
    private float _altitude;
    private float _gpsNotActiveTime;

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

        // Wait until service initializes
        int maxWait = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

        // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
#if UNITY_EDITOR
        int editorMaxWait = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            editorMaxWait--;
        }
#endif
        // Service didn't initialize in 15 seconds
        if (maxWait < 1)
        {
            // TODO Failure
            Debug.LogFormat("Timed out");
            yield break;
        }

        // Connection has failed
        if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
        {
            // TODO Failure
            Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
            testUIGPS.color = Color.red;

            //Try to start location service again 
            UnityEngine.Input.location.Stop();
            UnityEngine.Input.location.Start(25f, 0.1f);
        }

        while (true)
        {
            CheckInternet();
            if (UnityEngine.Input.location.status == LocationServiceStatus.Running)
            {
                GetGPS();
            }
            yield return new WaitForSecondsRealtime(3);
        }


    }

    // Stop service if there is no need to query location updates continuously
    //UnityEngine.Input.location.Stop();


    private void Update()
    {
        _elapsedTime += Time.deltaTime;


        if(UnityEngine.Input.location.status == LocationServiceStatus.Running && _elapsedTime >= 10)
        {
            // This foreach loop just to print allVisitedPos in editor
            foreach (ArcGISPoint point in _visitedPosList)
            {
                _allVisitedPos.Append("pointX"+point.X+" pointY"+point.Y+"/");
            }
           // Debug.Log("visited positions" + _allVisitedPos.ToString());
            _allVisitedPos.Clear();

            _elapsedTime = 0;
            SaveSystem.SavePositions(_visitedPosList);
        }

    }
    private void GetGPS()
    {
        if ((_latitude != UnityEngine.Input.location.lastData.latitude || _longitude != UnityEngine.Input.location.lastData.longitude)) //GPS is working
        {
            testUIGPS.color = Color.green;

            _gpsNotActiveTime = 0;

            Debug.Log("here");
            CheckInternet();
            //Debug.LogFormat("Location service live. status {0}", UnityEngine.Input.location.status);
            // Access granted and location value could be retrieved
            //Debug.LogFormat("Location: "
            //    + UnityEngine.Input.location.lastData.latitude + " "
            //    + UnityEngine.Input.location.lastData.longitude + " "
            //    + UnityEngine.Input.location.lastData.altitude + " "
            //    + UnityEngine.Input.location.lastData.horizontalAccuracy + " "
            //    + UnityEngine.Input.location.lastData.timestamp);

            if (_isFirstGettingGPS)
            {
                ShaderTextureTilingController.Instance.InitialiseTextureForCameraWhenFirstGetGPS();
                _isFirstGettingGPS = false;
            }

            Debug.Log("here1");
            _latitude = UnityEngine.Input.location.lastData.latitude;
            _longitude = UnityEngine.Input.location.lastData.longitude;
            _altitude = UnityEngine.Input.location.lastData.altitude;


            //Get round up to 5 decimal points
            _gpsPosition = new ArcGISPoint(Mathf.Round(_longitude * 100000f) / 100000f, Mathf.Round(_latitude * 100000f) / 100000f, Mathf.Round(_altitude * 100000f) / 100000f, ArcGISSpatialReference.WGS84());
            Debug.Log($"gpsPosition: {_gpsPosition.X},{_gpsPosition.Y}");



            if (CamInCentre)
            {
                _cameraRef.Position = new ArcGISPoint(_gpsPosition.X, _gpsPosition.Y, 500, _gpsPosition.SpatialReference);
            }

            if (!_mapRef.HasSpatialReference())
            {
                Debug.Log("no spatial refernece");
                //_playerDotRef.gameObject.transform.position = new Vector3(_mapRef.GeographicToEngine(_gpsPosition).x, _mapRef.GeographicToEngine(_gpsPosition).y, 0.1f);
            }
            else
            {
                Debug.Log("here2");
                _playerDotRef.Position = new ArcGISPoint(_gpsPosition.X, _gpsPosition.Y, 0.1, _gpsPosition.SpatialReference);
            }
        }
        else  //Lost GPS 
        {
            Debug.Log("hereLosing");
            _gpsNotActiveTime++;
        }
        Debug.Log("here7");
        if (_gpsNotActiveTime > 15f) //Lost GPS for more than 15f
        {
            testUIGPS.color = Color.red;

        }
        Debug.Log("here8");

        //store gpsPosition into _visitedPosList if the gps position has not existed in the list yet
        //In this case, we can have a list to record all gps position data that we have been visited
        //Create a new comparer so Contains function is comparing the X and Y values
        ArcGISPointEqualityComparer comparer = new ArcGISPointEqualityComparer();
        if (!_visitedPosList.Contains(_gpsPosition, comparer))
        {
            _visitedPosList.Add(_gpsPosition);
            Debug.Log("here4");
            if (!_mapRef.HasSpatialReference())
            {
                Debug.Log("Waiting for ArcGISMapComponent to have a valid spatial reference...");
                _gisPostToPixel = ShaderTextureTilingController.Instance.tiles[new Vector2(CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item1, CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item2)].transform.GetChild(0).GetComponent<GisPosToPixel>();
                //_pointInUV = _gisPostToPixel.gisPosToPixelMethodOffline(_mapRef.GeographicToEngine(_gpsPosition));
                //_shaderImage = _gisPostToPixel.gameObject.GetComponent<GISPosShader>();
                //_shaderImage.updatePositionInTexture(_pointInUV);
                //yield return null;

            }
            _gisPostToPixel = ShaderTextureTilingController.Instance.tiles[new Vector2(CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item1, CameraMovement.Instance.GetCameraCentralTile(_mapRef.GeographicToEngine(_cameraRef.Position)).Item2)].transform.GetChild(0).GetComponent<GisPosToPixel>();
            _pointInUV = _gisPostToPixel.gisPosToPixelMethod(_gpsPosition);
            _shaderImage = _gisPostToPixel.gameObject.GetComponent<GISPosShader>();
            Debug.Log("here5");
            _shaderImage.updatePositionInTexture(_pointInUV);

            if (_gpsNotActiveTime < 15f)
            {
                //Draw line between _lastPosition and current gps position (Designed for losing gps for short period of time)
                if (_lastPosition == null)
                {
                    _lastPosition = _gpsPosition;
                }
                _lastPointInUV = _gisPostToPixel.gisPosToPixelMethod(_lastPosition);
                _shaderImage.updateLineInTexture(_pointInUV, _lastPointInUV);
                _lastPosition = _gpsPosition;
            }
            Debug.Log("here6");
            
        }

    }

    private void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            testUIWifi.color = Color.red;
            
        }
        else if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            testUIWifi.color = Color.green;
            if (!_mapRef.HasSpatialReference() && isFirstTimeReload)
            {
                Debug.Log("try initialising map");
                StartCoroutine(ReInitialiseMap());
                isFirstTimeReload = false;
            }
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

    public void CameraMoveBackToPlayerDotCentre()
    {
        _cameraRef.gameObject.transform.position = new Vector3(_playerDotRef.gameObject.transform.position.x, _cameraRef.gameObject.transform.position.y, _playerDotRef.gameObject.transform.position.z);
        _reCentreButton.IconBackToDefault();
        CamInCentre = true;
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
        yield return new WaitForSecondsRealtime(1);
        _mapRef.enabled = false;
        yield return new WaitForSecondsRealtime(1);
        _mapRef.enabled = true;
    }

}
