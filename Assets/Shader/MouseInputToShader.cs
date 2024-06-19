using UnityEngine;

public class MouseInputToShader : MonoBehaviour
{
    public Material material;  // Assign the material using this shader in the inspector
    public float mouseRadius = 0.1f;
    public Texture2D maskTexture; // Mask texture to store alpha changes

    void Start()
    {
        // Initialize the mask texture with white color (alpha = 1)
        maskTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RFloat, false);
        for (int y = 0; y < maskTexture.height; y++)
        {
            for (int x = 0; x < maskTexture.width; x++)
            {
                maskTexture.SetPixel(x, y, Color.white);
            }
        }
        maskTexture.Apply();

        // Assign the mask texture to the material
        material.SetTexture("_MaskTex", maskTexture);

        // Calculate and set the aspect ratio
        float aspectRatio = (float)Screen.width / Screen.height;
        material.SetFloat("_AspectRatio", aspectRatio);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))  // Detect left mouse button
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 uvMousePos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height); 

            // Update the mask texture
            int x = (int)(uvMousePos.x * maskTexture.width);
            int y = (int)(uvMousePos.y * maskTexture.height); // Invert y-axis
            for (int i = -10; i <= 10; i++) // Update pixels in a small area around the mouse click
            {
                for (int j = -10; j <= 10; j++)
                {
                    if (x + i >= 0 && x + i < maskTexture.width && y + j >= 0 && y + j < maskTexture.height)
                    {
                        maskTexture.SetPixel(x + i, y + j, Color.black);
                    }
                }
            }
            maskTexture.Apply();
        }
    }
}
