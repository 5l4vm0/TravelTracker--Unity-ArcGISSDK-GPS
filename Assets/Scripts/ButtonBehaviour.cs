using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private Image DefaultIcon;
    [SerializeField] private Image SwapedIcon;

    // Start is called before the first frame update
    void Start()
    {
        DefaultIcon.enabled = true;
        SwapedIcon.enabled = false;
    }

    public void IconBackToDefault()
    {
        DefaultIcon.enabled = true;
        SwapedIcon.enabled = false;
        Debug.Log("button back to default");
    }

    public void SwapIcon()
    {
        DefaultIcon.enabled = false;
        SwapedIcon.enabled = true;
        Debug.Log("button swapped");
    }
}
