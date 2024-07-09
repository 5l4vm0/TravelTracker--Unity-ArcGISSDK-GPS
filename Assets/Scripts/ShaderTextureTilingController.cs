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
    public Vector3 BasedRefPos;

    private void Start()
    {
        Instance = this;
        BasedRefPos = new Vector3(CameraMovement.Instance.BottomLeft.x + 1500, 0, CameraMovement.Instance.BottomLeft.y + 1500);

        for (int x = 0; x < (CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x)/3000; x++)
        {
            for (int y = 0; y < (CameraMovement.Instance.TopLeft.y - CameraMovement.Instance.BottomLeft.y) / 3000; y++)
            {
                AddShaderTexture(x, y);
            }
        }
    }

    public ValueTuple<int, int> CalculateTileNumber(Vector3 EndPoint)
    {
        int tileX = Mathf.FloorToInt((EndPoint.x - BasedRefPos.x) / 3000);
        int tileY = Mathf.FloorToInt((EndPoint.z - BasedRefPos.z) / 3000)+1; //TODO: investigate why need to plus 1
        return (tileX, tileY);
    }

    public void AddShaderTexture(float TileNumberX, float TileNumberY)
    {
        GameObject newTile = Instantiate(ShaderTextureTilePrefab, new Vector3(BasedRefPos.x+3000*TileNumberX, 25, BasedRefPos.y+3000*TileNumberY), new Quaternion(0,0,0,0), this.transform);
        
        if(_greyImageBasedRef == null)
        {
            _greyImageBasedRef = newTile.GetComponent<GisPosToPixel>();
        }
    }


}
