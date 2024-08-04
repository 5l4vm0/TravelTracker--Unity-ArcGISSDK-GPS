using Esri.GameEngine.Geometry;
using UnityEngine;
using UnityEngine.UI;

public class GISPosShader : MonoBehaviour
{
    public Texture2D maskTexture; // Mask texture to store alpha changes
    [SerializeField] private Vector2 tileNumber;
    [SerializeField] private GisPosToPixel _gisPosToPixelRef;

    void Start()
    {
        Material materialInstance = new Material(Shader.Find("Custom/GrayFilterWithAlphaOnClick"));
        materialInstance.name = $"materialInstance[{tileNumber.x},{tileNumber.y}]";
        // Initialize the mask texture with white color (alpha = 1)
        maskTexture = new Texture2D(1000, 1000, TextureFormat.RFloat, false);
        for (int y = 0; y < maskTexture.height; y++)
        {
            for (int x = 0; x < maskTexture.width; x++)
            {
                maskTexture.SetPixel(x, y, Color.white);
            }
        }
        maskTexture.Apply();

        // Assign the mask texture to the material
        materialInstance.SetTexture("_MaskTex", maskTexture);
        this.GetComponent<Image>().material = materialInstance;
        
        //Draw the saved pos 
        if (LocationService.Instance.SavedPos != null)
        { 
            foreach (ArcGISPoint point in LocationService.Instance.SavedPos)
            {
                if (_gisPosToPixelRef.V0_0GIS.X <= point.X && point.X <= _gisPosToPixelRef.V1_0GIS.X && _gisPosToPixelRef.V0_0GIS.Y <= point.Y && point.Y <= _gisPosToPixelRef.V0_1GIS.Y)
                {
                    Vector2 _pointInUV = _gisPosToPixelRef.gisPosToPixelMethod(point);
                    updatePositionInTexture(_pointInUV);
                }
            }
        }

        // Calculate and set the aspect ratio
        //float aspectRatio = (float)Screen.width / Screen.height;
        //material.SetFloat("_AspectRatio", aspectRatio);
    }

    void Update()
    {
        //if (Input.GetMouseButton(0))  // Detect left mouse button
        //{
        //    Vector3 mousePos = Input.mousePosition;
        //    Vector2 uvMousePos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);

        //    // Update the mask texture
        //    int x = (int)(uvMousePos.x * maskTexture.width);
        //    int y = (int)(uvMousePos.y * maskTexture.height); // Invert y-axis
        //    for (int i = -10; i <= 10; i++) // Update pixels in a small area around the mouse click
        //    {
        //        for (int j = -10; j <= 10; j++)
        //        {
        //            if (x + i >= 0 && x + i < maskTexture.width && y + j >= 0 && y + j < maskTexture.height)
        //            {
        //                maskTexture.SetPixel(x + i, y + j, Color.black);
        //            }
        //        }
        //    }
        //    maskTexture.Apply();
        //}
    }

    public void AssignTileNumber(int tileNumX, int tileNumY)
    {
        tileNumber = new Vector2(tileNumX, tileNumY);
    }

    public void updatePositionInTexture(Vector2 pointInUV)
    {
        int x = (int)(pointInUV.x * maskTexture.width);
        int y = (int)(pointInUV.y * maskTexture.height);
        int radius = (int)(0.01f * maskTexture.width);
        
        for (int i = -radius; i <= radius; i++) 
        {
            for (int j = -radius; j <= radius; j++)
            {
                if(i*i + j*j <= radius*radius)
                {
                    if (x + i >= 0 && x + i < maskTexture.width && y + j >= 0 && y + j < maskTexture.height)
                    {
                        maskTexture.SetPixel(x + i, y + j, Color.black);
                    }
                }
            }
        }
        maskTexture.Apply();
    }

    public void updateLineInTexture(Vector2 pointNew, Vector2 pointOld)
    {
        int x0 = (int)(pointOld.x * maskTexture.width);
        int y0 = (int)(pointOld.y * maskTexture.height);
        int x1 = (int)(pointNew.x * maskTexture.width);
        int y1 = (int)(pointNew.y * maskTexture.height);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int radius = (int)(0.01f * maskTexture.width);

        while (true)
        {
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (i * i + j * j <= radius * radius)
                    {
                        int pixelX = x0 + i;
                        int pixelY = y0 + j;

                        if (pixelX >= 0 && pixelX < maskTexture.width && pixelY >= 0 && pixelY < maskTexture.height)
                        {
                            maskTexture.SetPixel(pixelX, pixelY, Color.black);
                        }
                    }
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        maskTexture.Apply();
    }
}
