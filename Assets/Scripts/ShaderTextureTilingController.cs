using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.HPFramework;
using Esri.ArcGISMapsSDK.Components;
using System;

public class ShaderTextureTilingController : MonoBehaviour
{
    //Singleton
    public static ShaderTextureTilingController Instance;

    public GameObject ShaderTextureTilePrefab;
    [SerializeField] private GisPosToPixel _greyImageBasedRef;
    public Vector3 BasedRefBottomLeftPos;

    private void Start()
    {
        Instance = this;
        BasedRefBottomLeftPos = new Vector3(CameraMovement.Instance.BottomLeft.x, 0, CameraMovement.Instance.BottomLeft.z );

        for (int x = 0; x < (CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x)/3000; x++)
        {
            for (int y = 0; y < (CameraMovement.Instance.TopLeft.z - CameraMovement.Instance.BottomLeft.z) / 3000; y++)
            {
                AddShaderTexture(x, y);
            }
        }
    }

    public ValueTuple<int, int> CalculateTileNumber(Vector3 EndPoint)
    {
        int tileX = Mathf.FloorToInt((EndPoint.x - BasedRefBottomLeftPos.x) / 3000);
        int tileY = Mathf.FloorToInt((EndPoint.z - BasedRefBottomLeftPos.z) / 3000); 
        Debug.Log("EndPoint: " + EndPoint + "BasedRefPos: " + BasedRefBottomLeftPos);
        Debug.Log("tileX: "+tileX+ "tileY: "+tileY);
        return (tileX, tileY);
    }

    public void AddShaderTexture(float TileNumberX, float TileNumberY)
    {
        GameObject newTile = Instantiate(ShaderTextureTilePrefab, new Vector3(BasedRefBottomLeftPos.x+1500+3000*TileNumberX, 25, BasedRefBottomLeftPos.z+1500+3000*TileNumberY), new Quaternion(0,0,0,0), this.transform);
        if(_greyImageBasedRef == null)
        {
            _greyImageBasedRef = newTile.transform.GetChild(0).GetComponent<GisPosToPixel>();
            //BasedRefPos = _greyImageBasedRef.transform.parent.transform.position;
        }
    }


}
