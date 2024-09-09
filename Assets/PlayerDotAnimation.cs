using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDotAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer walk1;
    [SerializeField] private SpriteRenderer walk2;
    [SerializeField] private SpriteRenderer walk3;
    private float oldXPosition;
    Coroutine right;
    Coroutine left;

    // Start is called before the first frame update
    void Start()
    {
        walk1.enabled = false;
        walk2.enabled = false;
        walk3.enabled = true;
        oldXPosition = transform.position.x;
        StartCoroutine(UpdateAnimation());
    }

    private IEnumerator UpdateAnimation()
    {
        while(true)
        {
            if ((transform.position.x - oldXPosition) >= 1) //moving to the right
            {
                oldXPosition = transform.position.x;
                right = StartCoroutine(RightWalking());
            }
            else if ((oldXPosition - transform.position.x) >= 1) //moving to the left
            {
                oldXPosition = transform.position.x;
                left = StartCoroutine(LeftWalking());
            }
            else  //not moving to the left or right 
            {
                oldXPosition = transform.position.x;
                StopAnimation();
            }
            yield return new WaitForSecondsRealtime(4);
        }
    }
    private IEnumerator RightWalking()
    {
        Debug.Log("RightWalking");
        if(right != null )
        {
            StopCoroutine(right);
        }
        if (left != null)
        {
            StopCoroutine(left);
        }


        walk1.flipX = false;
        walk2.flipX = false;
        walk3.flipX = false;

        while (true)
        {
            walk1.enabled = true;
            walk2.enabled = false;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = true;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = false;
            walk3.enabled = true;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = true;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private IEnumerator LeftWalking()
    {
        Debug.Log("leftwalking");
        if (right != null)
        {
            StopCoroutine(right);
        }
        if (left != null)
        {
            StopCoroutine(left);
        }


        walk1.flipX = true;
        walk2.flipX = true;
        walk3.flipX = true;

        while (true)
        {
            walk1.enabled = true;
            walk2.enabled = false;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = true;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = false;
            walk3.enabled = true;
            yield return new WaitForSecondsRealtime(0.5f);
            walk1.enabled = false;
            walk2.enabled = true;
            walk3.enabled = false;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void StopAnimation()
    {
        Debug.Log("stopAnimation");
        if (right != null)
        {
            StopCoroutine(right);
        }
        if(left != null)
        {
            StopCoroutine(left);
        }
            
        walk1.enabled = false;
        walk2.enabled = false;
        walk3.enabled = true;
    }
}