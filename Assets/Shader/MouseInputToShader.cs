using UnityEngine;

public class MouseInputToShader : MonoBehaviour
{
    public Material material;  // Assign the material using this shader in the inspector
    public float mouseRadius = 0.1f;

    void Update()
    {
        if (Input.GetMouseButton(0))  // Detect left mouse button
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 uvMousePos = new Vector2(1.0f - (mousePos.x / Screen.width), 1.0f - (mousePos.y / Screen.height)); // Invert X and Y-axis
            material.SetVector("_MousePos", new Vector4(uvMousePos.x, uvMousePos.y, 0, 0));
            material.SetFloat("_MouseRadius", mouseRadius);
        }
        else
        {
            // Set mouse position outside the screen if no input
            material.SetVector("_MousePos", new Vector4(-1, -1, 0, 0));
        }
    }
}
