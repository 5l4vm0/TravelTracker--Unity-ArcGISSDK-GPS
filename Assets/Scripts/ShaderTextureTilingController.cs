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
    public Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();

    private void Start()
    {
        Instance = this;
        BasedRefBottomLeftPos = new Vector3(CameraMovement.Instance.BottomLeft.x, 0, CameraMovement.Instance.BottomLeft.z );

        for (int x = 0; x < (CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x)/3000; x++)
        {
            for (int y = 0; y < (CameraMovement.Instance.TopLeft.z - CameraMovement.Instance.BottomLeft.z) / 3000; y++)
            {
                
                if(!tiles.ContainsKey(new Vector2(x, y)))
                {
                    AddShaderTexture(x, y);
                }
            }
        }
    }

    public ValueTuple<int, int> CalculateTileNumber(Vector3 EndPoint)
    {
        int tileX = Mathf.FloorToInt((EndPoint.x - BasedRefBottomLeftPos.x) / 3000);
        int tileY = Mathf.FloorToInt((EndPoint.z - BasedRefBottomLeftPos.z) / 3000);
        return (tileX, tileY);
    }

    public GameObject AddShaderTexture(int TileNumberX, int TileNumberY)
    {
        if (!tiles.ContainsKey(new Vector2(TileNumberX,TileNumberY)))
        {
            GameObject newTile = Instantiate(ShaderTextureTilePrefab, new Vector3(BasedRefBottomLeftPos.x + 1500 + 3000 * TileNumberX, 25, BasedRefBottomLeftPos.z + 1500 + 3000 * TileNumberY), new Quaternion(0, 0, 0, 0), this.transform);
            newTile.name = $"ShaderTextureTilePrefab [{TileNumberX},{TileNumberY}]";
            newTile.transform.GetChild(0).GetComponent<GISPosShader>().AssignTileNumber(TileNumberX, TileNumberY);
            tiles.Add(new Vector2(TileNumberX, TileNumberY), newTile);
            return newTile;
        }
        else
        {
            return tiles[new Vector2(TileNumberX, TileNumberY)];
        }
    }

    public void loopThroughViewport(int CentreTileNumberX, int CentreTileNumberY)
    {
        
        //Get centre tile number and use BottomRight - BottomLeft/3000 to know how many tiles are visble, divide into 2 so we know how many for each side(left and right), final +1 is for forloop to loop through 
        for (int x = CentreTileNumberX - Mathf.RoundToInt((CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x) / 3000 / 2); x <CentreTileNumberX+ Mathf.RoundToInt((CameraMovement.Instance.BottomRight.x - CameraMovement.Instance.BottomLeft.x) / 3000 / 2)+1; x++)
        {
            for (int y = CentreTileNumberY - Mathf.RoundToInt((CameraMovement.Instance.TopLeft.z - CameraMovement.Instance.BottomLeft.z) / 3000 / 2); y < CentreTileNumberY + Mathf.RoundToInt((CameraMovement.Instance.TopLeft.z - CameraMovement.Instance.BottomLeft.z) / 3000 / 2)+1; y++)
            {
                AddShaderTexture(x, y);
            }
        }
    }


}
