using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.HPFramework;
using Esri.ArcGISMapsSDK.Components;

public class ShaderTextureTilingController : MonoBehaviour
{
    public GameObject ShaderTextureTilePrefab;
    [SerializeField] private GisPosToPixel _greyImageBasedRef;
    [SerializeField] private Vector3 _basedRefPos;

    private void Start()
    {
        
        _basedRefPos = new Vector3(CameraMovement.Instance.BottomLeft.x + 1500, 0, CameraMovement.Instance.BottomLeft.y + 1500);
       // AddShaderTexture(0, 0, _basedRefPos);


        for (int x =0; x< (CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x)/3000; x++)
        {
            for (int y = 0; y < (CameraMovement.Instance.TopLeft.y - CameraMovement.Instance.BottomLeft.y) / 3000; y++)
            {
                AddShaderTexture(x, y, _basedRefPos);
            }
        }
    }

    public void AddShaderTexture(float TileNumberX, float TileNumberY, Vector3 position)
    {
        GameObject newTile = Instantiate(ShaderTextureTilePrefab, new Vector3(position.x+3000*TileNumberX, 25, position.y+3000*TileNumberY), new Quaternion(0,0,0,0), this.transform);
        
        if(_greyImageBasedRef == null)
        {
            _greyImageBasedRef = newTile.GetComponent<GisPosToPixel>();
        }
    }
}
